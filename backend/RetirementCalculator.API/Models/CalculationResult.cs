namespace RetirementCalculator.API.Models;

/// <summary>
/// Result of retirement withdrawal calculation
/// </summary>
public class CalculationResult
{
    /// <summary>
    /// Recommended safe withdrawal rate as a percentage (e.g., 4.0 for 4%)
    /// </summary>
    public decimal WithdrawalRate { get; set; }

    /// <summary>
    /// Annual gross withdrawal amount in USD before taxes
    /// </summary>
    public decimal AnnualGrossWithdrawal { get; set; }

    /// <summary>
    /// Estimated annual taxes on withdrawals in USD
    /// </summary>
    public decimal EstimatedAnnualTaxes { get; set; }

    /// <summary>
    /// Net annual income after taxes in USD
    /// </summary>
    public decimal NetAnnualIncome { get; set; }

    /// <summary>
    /// Achieved success rate as a percentage (e.g., 95.0 for 95%)
    /// </summary>
    public decimal AchievedSuccessRate { get; set; }

    /// <summary>
    /// Number of historical scenarios simulated in the calculation
    /// </summary>
    public int NumberOfScenariosSimulated { get; set; }
}
