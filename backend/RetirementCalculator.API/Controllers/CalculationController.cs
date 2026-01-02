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
    private readonly IEarlyRetirementService _earlyRetirementService;
    private readonly IRothConversionService _rothConversionService;
    private readonly IAllocationAnalysisService _allocationAnalysisService;
    private readonly IReverseRetirementCalculationService _reverseCalculationService;

    public CalculationController(
        ILogger<CalculationController> logger,
        IWithdrawalCalculationService calculationService,
        IEarlyRetirementService earlyRetirementService,
        IRothConversionService rothConversionService,
        IAllocationAnalysisService allocationAnalysisService,
        IReverseRetirementCalculationService reverseCalculationService)
    {
        _logger = logger;
        _calculationService = calculationService;
        _earlyRetirementService = earlyRetirementService;
        _rothConversionService = rothConversionService;
        _allocationAnalysisService = allocationAnalysisService;
        _reverseCalculationService = reverseCalculationService;
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

            // Calculate early retirement penalties
            var retirementYears = 95 - userInput.RetirementAge; // Assume living to 95
            var (totalPenalty, yearsWithPenalty) = _earlyRetirementService.CalculateTotalPenalties(
                userInput.RetirementAge,
                retirementYears,
                result.TaxDeferredAccountWithdrawal);

            result.EarlyWithdrawalPenalty = totalPenalty;
            result.YearsWithPenalty = yearsWithPenalty;
            result.PenaltyWarning = _earlyRetirementService.GetPenaltyWarning(userInput.RetirementAge);
            result.PenaltyExplanation = _earlyRetirementService.GetPenaltyExplanation(yearsWithPenalty, totalPenalty);

            // Evaluate Roth conversion ladder strategy if early retirement detected
            if (yearsWithPenalty > 0)
            {
                result.RothConversionAnalysis = _rothConversionService.EvaluateConversionStrategy(
                    userInput.RetirementAge,
                    result.AnnualGrossWithdrawal,
                    result.TaxDeferredAccountWithdrawal,
                    result.OrdinaryIncomeTax,
                    totalPenalty,
                    yearsWithPenalty);
            }

            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

            _logger.LogInformation(
                "Calculation completed successfully in {Duration}ms - WithdrawalRate: {WithdrawalRate}%, " +
                "AnnualGrossWithdrawal: {AnnualGrossWithdrawal}, NetAnnualIncome: {NetAnnualIncome}, " +
                "AchievedSuccessRate: {AchievedSuccessRate}%, EarlyWithdrawalPenalty: {EarlyWithdrawalPenalty}",
                duration, result.WithdrawalRate, result.AnnualGrossWithdrawal,
                result.NetAnnualIncome, result.AchievedSuccessRate, result.EarlyWithdrawalPenalty
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

    /// <summary>
    /// Analyze portfolio allocations and return top 3 recommended strategies
    /// </summary>
    /// <param name="userInput">User input data including age, balances, and success rate threshold</param>
    /// <returns>Analysis result with Conservative, Balanced, and Aggressive allocation strategies</returns>
    /// <response code="200">Returns the allocation analysis result</response>
    /// <response code="400">If the input data is invalid</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("analyze-allocations")]
    [ProducesResponseType(typeof(AllocationAnalysisResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AnalyzeAllocations([FromBody] UserInput userInput)
    {
        var startTime = DateTime.UtcNow;

        try
        {
            _logger.LogInformation(
                "Received allocation analysis request - CurrentAge: {CurrentAge}, RetirementAge: {RetirementAge}, " +
                "RetirementAccountBalance: {RetirementAccountBalance}, TaxableAccountBalance: {TaxableAccountBalance}, " +
                "SuccessRateThreshold: {SuccessRateThreshold}",
                userInput.CurrentAge, userInput.RetirementAge, userInput.RetirementAccountBalance,
                userInput.TaxableAccountBalance, userInput.SuccessRateThreshold
            );

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for allocation analysis request");
                return BadRequest(ModelState);
            }

            if (userInput.RetirementAge < userInput.CurrentAge)
            {
                ModelState.AddModelError(nameof(userInput.RetirementAge),
                    "Retirement age must be greater than or equal to current age");
                _logger.LogWarning("Validation failed: Retirement age ({RetirementAge}) is less than current age ({CurrentAge})",
                    userInput.RetirementAge, userInput.CurrentAge);
                return BadRequest(ModelState);
            }

            var result = await _allocationAnalysisService.AnalyzeAllocations(userInput);

            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

            _logger.LogInformation(
                "Allocation analysis completed successfully in {Duration}ms - " +
                "Conservative: {ConservativeAlloc}% stocks ({ConservativeSuccess}% success), " +
                "Balanced: {BalancedAlloc}% stocks ({BalancedSuccess}% success), " +
                "Aggressive: {AggressiveAlloc}% stocks ({AggressiveSuccess}% success)",
                duration,
                result.Conservative.StockAllocation, result.Conservative.HistoricalSuccessRate,
                result.Balanced.StockAllocation, result.Balanced.HistoricalSuccessRate,
                result.Aggressive.StockAllocation, result.Aggressive.HistoricalSuccessRate
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogError(ex, "An error occurred while processing the allocation analysis request after {Duration}ms", duration);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An error occurred while processing your request. Please try again later.",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Reverse calculator: Calculate required portfolio size from desired after-tax retirement income
    /// </summary>
    /// <param name="input">Reverse calculation input including desired income and risk profile</param>
    /// <returns>Required portfolio sizes for different risk profiles</returns>
    /// <response code="200">Returns the reverse calculation result</response>
    /// <response code="400">If the input data is invalid</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("reverse")]
    [ProducesResponseType(typeof(ReverseCalculationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReverseCalculate([FromBody] ReverseCalculationInput input)
    {
        var startTime = DateTime.UtcNow;

        try
        {
            _logger.LogInformation(
                "Reverse calculation requested: Desired after-tax income ${Income}, Age {CurrentAge} -> {RetirementAge}, Success rate {SuccessRate}%",
                input.DesiredAfterTaxIncome,
                input.CurrentAge,
                input.RetirementAge,
                input.SuccessRateThreshold * 100);

            // Validate retirement age is after current age
            if (input.RetirementAge <= input.CurrentAge)
            {
                return BadRequest(new { error = "Retirement age must be greater than current age" });
            }

            var result = await _reverseCalculationService.CalculateRequiredPortfolioAsync(input);

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation(
                "Reverse calculation completed in {Duration}ms. Scenarios calculated: {ScenarioCount}",
                duration.TotalMilliseconds,
                result.Scenarios.Count);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid input for reverse calculation: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during reverse calculation");
            return StatusCode(500, new { error = "An error occurred while calculating required portfolio. Please try again later." });
        }
    }
}
