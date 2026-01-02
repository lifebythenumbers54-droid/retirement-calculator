using RetirementCalculator.API.Models;

namespace RetirementCalculator.API.Services;

public interface IWithdrawalCalculationService
{
    /// <summary>
    /// Calculates the optimal withdrawal strategy based on historical market data.
    /// </summary>
    /// <param name="userInput">User's retirement planning parameters</param>
    /// <returns>Calculated withdrawal strategy and success metrics</returns>
    Task<CalculationResult> CalculateWithdrawalStrategy(UserInput userInput);

    /// <summary>
    /// Finds the optimal withdrawal rate for a given portfolio size and target success rate.
    /// </summary>
    Task<CalculationResult> FindOptimalWithdrawalRateAsync(
        decimal retirementAccountBalance,
        decimal taxableAccountBalance,
        int retirementYears,
        decimal targetSuccessRate,
        decimal stockAllocation);

    /// <summary>
    /// Calculates portfolio statistics for a given withdrawal rate and allocation.
    /// </summary>
    Task<PortfolioStatistics> CalculatePortfolioStatisticsAsync(
        decimal retirementAccountBalance,
        decimal taxableAccountBalance,
        int retirementYears,
        decimal withdrawalRate,
        decimal stockAllocation);
}

public class PortfolioStatistics
{
    public decimal MedianFinalValue { get; set; }
    public decimal WorstCaseValue { get; set; }
    public decimal BestCaseValue { get; set; }
}
