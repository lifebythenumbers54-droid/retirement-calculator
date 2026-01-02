namespace RetirementCalculator.API.Models;

/// <summary>
/// Result of portfolio allocation analysis containing recommended strategies
/// </summary>
public class AllocationAnalysisResult
{
    /// <summary>
    /// Conservative strategy (highest success rate, lower volatility)
    /// </summary>
    public AllocationStrategy Conservative { get; set; } = new();

    /// <summary>
    /// Balanced strategy (optimal risk/reward ratio)
    /// </summary>
    public AllocationStrategy Balanced { get; set; } = new();

    /// <summary>
    /// Aggressive strategy (higher potential returns, accepts more risk)
    /// </summary>
    public AllocationStrategy Aggressive { get; set; } = new();

    /// <summary>
    /// User's retirement duration in years
    /// </summary>
    public int RetirementDuration { get; set; }

    /// <summary>
    /// User's target success rate threshold
    /// </summary>
    public decimal TargetSuccessRate { get; set; }

    /// <summary>
    /// Total portfolio balance
    /// </summary>
    public decimal TotalPortfolioBalance { get; set; }

    /// <summary>
    /// Number of different allocations analyzed
    /// </summary>
    public int AllocationsAnalyzed { get; set; }

    /// <summary>
    /// Explanation of methodology used
    /// </summary>
    public string Methodology { get; set; } = string.Empty;
}
