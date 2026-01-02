using Microsoft.AspNetCore.Mvc;
using RetirementCalculator.API.Models;

namespace RetirementCalculator.API.Controllers;

/// <summary>
/// Controller for retirement calculation endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CalculationController : ControllerBase
{
    private readonly ILogger<CalculationController> _logger;

    public CalculationController(ILogger<CalculationController> logger)
    {
        _logger = logger;
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
    public IActionResult Calculate([FromBody] UserInput userInput)
    {
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

            // For Milestone 1: Return mock calculation results
            // This will be replaced with actual calculation logic in future milestones
            var totalBalance = userInput.RetirementAccountBalance + userInput.TaxableAccountBalance;
            var withdrawalRate = 4.0m; // Mock 4% withdrawal rate
            var annualGrossWithdrawal = totalBalance * (withdrawalRate / 100);
            var estimatedTaxes = annualGrossWithdrawal * 0.15m; // Mock 15% tax rate
            var netIncome = annualGrossWithdrawal - estimatedTaxes;

            var result = new CalculationResult
            {
                WithdrawalRate = withdrawalRate,
                AnnualGrossWithdrawal = annualGrossWithdrawal,
                EstimatedAnnualTaxes = estimatedTaxes,
                NetAnnualIncome = netIncome,
                AchievedSuccessRate = userInput.SuccessRateThreshold * 100, // Convert to percentage
                NumberOfScenariosSimulated = 100 // Mock value
            };

            _logger.LogInformation(
                "Calculation completed successfully - WithdrawalRate: {WithdrawalRate}%, " +
                "AnnualGrossWithdrawal: {AnnualGrossWithdrawal}, NetAnnualIncome: {NetAnnualIncome}",
                result.WithdrawalRate, result.AnnualGrossWithdrawal, result.NetAnnualIncome
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the calculation request");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An error occurred while processing your request. Please try again later.",
                error = ex.Message
            });
        }
    }
}
