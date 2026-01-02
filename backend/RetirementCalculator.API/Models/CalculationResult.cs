using RetirementCalculator.API.Services;

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

    /// <summary>
    /// Annual withdrawal amount from taxable accounts (subject to capital gains tax)
    /// </summary>
    public decimal TaxableAccountWithdrawal { get; set; }

    /// <summary>
    /// Annual withdrawal amount from tax-deferred accounts (subject to ordinary income tax)
    /// </summary>
    public decimal TaxDeferredAccountWithdrawal { get; set; }

    /// <summary>
    /// Ordinary income tax on tax-deferred withdrawals
    /// </summary>
    public decimal OrdinaryIncomeTax { get; set; }

    /// <summary>
    /// Capital gains tax on taxable account withdrawals
    /// </summary>
    public decimal CapitalGainsTax { get; set; }

    /// <summary>
    /// Effective tax rate as a percentage of gross withdrawal
    /// </summary>
    public decimal EffectiveTaxRate { get; set; }

    /// <summary>
    /// Total early withdrawal penalty for retiring before age 59.5
    /// </summary>
    public decimal EarlyWithdrawalPenalty { get; set; }

    /// <summary>
    /// Number of years subject to early withdrawal penalty
    /// </summary>
    public int YearsWithPenalty { get; set; }

    /// <summary>
    /// Warning message for early retirement (if applicable)
    /// </summary>
    public string? PenaltyWarning { get; set; }

    /// <summary>
    /// Explanation of early withdrawal penalty
    /// </summary>
    public string PenaltyExplanation { get; set; } = string.Empty;

    /// <summary>
    /// Roth IRA conversion ladder strategy analysis (if applicable for early retirement)
    /// </summary>
    public RothConversionAnalysis? RothConversionAnalysis { get; set; }
}
