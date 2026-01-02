namespace RetirementCalculator.API.Models;

/// <summary>
/// Represents a recommended portfolio allocation strategy with historical performance data
/// </summary>
public class AllocationStrategy
{
    /// <summary>
    /// Name/Type of the strategy (e.g., "Conservative", "Balanced", "Aggressive")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Percentage allocated to stocks (0-100)
    /// </summary>
    public decimal StockAllocation { get; set; }

    /// <summary>
    /// Percentage allocated to bonds (0-100)
    /// </summary>
    public decimal BondAllocation { get; set; }

    /// <summary>
    /// Historical success rate as a percentage (e.g., 94.0 for 94%)
    /// </summary>
    public decimal HistoricalSuccessRate { get; set; }

    /// <summary>
    /// Median final portfolio value across all historical scenarios
    /// </summary>
    public decimal MedianFinalValue { get; set; }

    /// <summary>
    /// Best case final portfolio value
    /// </summary>
    public decimal BestCaseFinalValue { get; set; }

    /// <summary>
    /// Worst case final portfolio value
    /// </summary>
    public decimal WorstCaseFinalValue { get; set; }

    /// <summary>
    /// Average annual volatility (standard deviation of returns)
    /// </summary>
    public decimal AverageVolatility { get; set; }

    /// <summary>
    /// Number of historical scenarios that succeeded
    /// </summary>
    public int SuccessfulScenarios { get; set; }

    /// <summary>
    /// Total number of historical scenarios tested
    /// </summary>
    public int TotalScenarios { get; set; }

    /// <summary>
    /// Years/periods where this allocation failed (e.g., "1966-1995")
    /// </summary>
    public List<string> FailurePeriods { get; set; } = new();

    /// <summary>
    /// Recommended withdrawal rate for this allocation
    /// </summary>
    public decimal RecommendedWithdrawalRate { get; set; }

    /// <summary>
    /// Expected annual gross withdrawal amount
    /// </summary>
    public decimal ExpectedAnnualWithdrawal { get; set; }

    /// <summary>
    /// Expected net annual income after taxes
    /// </summary>
    public decimal ExpectedNetIncome { get; set; }

    /// <summary>
    /// Description of the strategy's trade-offs
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Overall score for ranking (0-100)
    /// </summary>
    public decimal Score { get; set; }
}
