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
}
