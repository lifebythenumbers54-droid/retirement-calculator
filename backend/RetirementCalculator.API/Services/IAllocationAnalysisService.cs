using RetirementCalculator.API.Models;

namespace RetirementCalculator.API.Services;

/// <summary>
/// Service for analyzing and recommending optimal portfolio allocations
/// </summary>
public interface IAllocationAnalysisService
{
    /// <summary>
    /// Analyzes multiple stock/bond allocations and returns top 3 recommended strategies
    /// </summary>
    /// <param name="userInput">User input with retirement parameters</param>
    /// <returns>Analysis result with Conservative, Balanced, and Aggressive strategies</returns>
    Task<AllocationAnalysisResult> AnalyzeAllocations(UserInput userInput);

    /// <summary>
    /// Calculates success rate for a specific allocation strategy
    /// </summary>
    /// <param name="stockAllocation">Stock allocation percentage (0-1)</param>
    /// <param name="totalBalance">Total portfolio balance</param>
    /// <param name="withdrawalRate">Annual withdrawal rate (0-1)</param>
    /// <param name="retirementYears">Number of retirement years</param>
    /// <returns>Success rate as percentage</returns>
    Task<decimal> CalculateSuccessRateForAllocation(
        decimal stockAllocation,
        decimal totalBalance,
        decimal withdrawalRate,
        int retirementYears);
}
