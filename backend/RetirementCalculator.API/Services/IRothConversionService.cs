namespace RetirementCalculator.API.Services;

/// <summary>
/// Service for calculating Roth IRA conversion ladder strategies for early retirement
/// </summary>
public interface IRothConversionService
{
    /// <summary>
    /// Evaluates whether a Roth conversion ladder strategy would be more tax-efficient
    /// than paying early withdrawal penalties
    /// </summary>
    /// <param name="retirementAge">Age at which user plans to retire</param>
    /// <param name="annualWithdrawalNeeded">Annual withdrawal amount needed in retirement</param>
    /// <param name="taxDeferredWithdrawal">Annual withdrawal from tax-deferred accounts</param>
    /// <param name="ordinaryIncomeTax">Annual ordinary income tax on withdrawals</param>
    /// <param name="earlyWithdrawalPenalty">Total early withdrawal penalty under standard approach</param>
    /// <param name="yearsWithPenalty">Number of years subject to penalty</param>
    /// <returns>Roth conversion strategy analysis</returns>
    RothConversionAnalysis EvaluateConversionStrategy(
        int retirementAge,
        decimal annualWithdrawalNeeded,
        decimal taxDeferredWithdrawal,
        decimal ordinaryIncomeTax,
        decimal earlyWithdrawalPenalty,
        int yearsWithPenalty);
}

/// <summary>
/// Analysis result for Roth IRA conversion ladder strategy
/// </summary>
public class RothConversionAnalysis
{
    /// <summary>
    /// Whether Roth conversion ladder is recommended (more tax-efficient)
    /// </summary>
    public bool IsRecommended { get; set; }

    /// <summary>
    /// Total tax cost using conversion ladder strategy
    /// </summary>
    public decimal TotalConversionTaxCost { get; set; }

    /// <summary>
    /// Total cost using standard penalty approach (penalties + taxes)
    /// </summary>
    public decimal TotalPenaltyCost { get; set; }

    /// <summary>
    /// Estimated savings by using conversion ladder vs penalties
    /// </summary>
    public decimal EstimatedSavings { get; set; }

    /// <summary>
    /// Year-by-year conversion schedule
    /// </summary>
    public List<ConversionYear> ConversionSchedule { get; set; } = new();

    /// <summary>
    /// Explanation of the strategy
    /// </summary>
    public string StrategyExplanation { get; set; } = string.Empty;

    /// <summary>
    /// Warning about first 5 years (transition period)
    /// </summary>
    public string TransitionWarning { get; set; } = string.Empty;

    /// <summary>
    /// Whether the user needs funds immediately (can't wait 5 years)
    /// </summary>
    public bool RequiresImmediateFunds { get; set; }
}

/// <summary>
/// Represents a single year in the Roth conversion schedule
/// </summary>
public class ConversionYear
{
    /// <summary>
    /// Year number (0 = first year of retirement)
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// User's age in this year
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Amount to convert from traditional to Roth IRA
    /// </summary>
    public decimal ConversionAmount { get; set; }

    /// <summary>
    /// Tax owed on conversion in this year
    /// </summary>
    public decimal ConversionTax { get; set; }

    /// <summary>
    /// Amount available for withdrawal from previous conversions (after 5-year seasoning)
    /// </summary>
    public decimal AvailableForWithdrawal { get; set; }

    /// <summary>
    /// Notes about this year's strategy
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}
