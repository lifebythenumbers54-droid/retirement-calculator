using Microsoft.AspNetCore.Mvc;
using RetirementCalculator.API.Models;
using RetirementCalculator.API.Services;

namespace RetirementCalculator.API.Controllers;

/// <summary>
/// Controller for retirement calculation endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CalculationController : ControllerBase
{
    private readonly ILogger<CalculationController> _logger;
    private readonly IWithdrawalCalculationService _calculationService;

    public CalculationController(
        ILogger<CalculationController> logger,
        IWithdrawalCalculationService calculationService)
    {
        _logger = logger;
        _calculationService = calculationService;
    }

    /// <summary>
    /// Calculate retirement withdrawal strategy based on user input
    /// </summary>
    /// <param name="userInput">User input data including age, balances, and success rate threshold</param>
    /// <returns>Calculation result with withdrawal rates and income projections</returns>
    /// <response code="200">Returns the calculation result</response>
    /// <response code="400">If the input data is invalid</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [ProducesResponseType(typeof(CalculationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Calculate([FromBody] UserInput userInput)
    {
        var startTime = DateTime.UtcNow;

        try
        {
            _logger.LogInformation(
                "Received calculation request - CurrentAge: {CurrentAge}, RetirementAge: {RetirementAge}, " +
                "RetirementAccountBalance: {RetirementAccountBalance}, TaxableAccountBalance: {TaxableAccountBalance}, " +
                "SuccessRateThreshold: {SuccessRateThreshold}",
                userInput.CurrentAge, userInput.RetirementAge, userInput.RetirementAccountBalance,
                userInput.TaxableAccountBalance, userInput.SuccessRateThreshold
            );

            // Validate model state
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for calculation request");
                return BadRequest(ModelState);
            }

            // Custom validation: retirement age must be >= current age
            if (userInput.RetirementAge < userInput.CurrentAge)
            {
                ModelState.AddModelError(nameof(userInput.RetirementAge),
                    "Retirement age must be greater than or equal to current age");
                _logger.LogWarning("Validation failed: Retirement age ({RetirementAge}) is less than current age ({CurrentAge})",
                    userInput.RetirementAge, userInput.CurrentAge);
                return BadRequest(ModelState);
            }

            // Perform actual calculation using historical data
            var result = await _calculationService.CalculateWithdrawalStrategy(userInput);

            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

            _logger.LogInformation(
                "Calculation completed successfully in {Duration}ms - WithdrawalRate: {WithdrawalRate}%, " +
                "AnnualGrossWithdrawal: {AnnualGrossWithdrawal}, NetAnnualIncome: {NetAnnualIncome}, " +
                "AchievedSuccessRate: {AchievedSuccessRate}%",
                duration, result.WithdrawalRate, result.AnnualGrossWithdrawal,
                result.NetAnnualIncome, result.AchievedSuccessRate
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogError(ex, "An error occurred while processing the calculation request after {Duration}ms", duration);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An error occurred while processing your request. Please try again later.",
                error = ex.Message
            });
        }
    }
}
