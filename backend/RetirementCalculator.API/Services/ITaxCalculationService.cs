namespace RetirementCalculator.API.Services;

/// <summary>
/// Service for calculating federal income taxes on retirement account withdrawals.
/// </summary>
public interface ITaxCalculationService
{
    /// <summary>
    /// Calculates the total federal tax on ordinary income.
    /// </summary>
    /// <param name="ordinaryIncome">The amount of ordinary income subject to tax</param>
    /// <returns>The calculated tax amount</returns>
    decimal CalculateOrdinaryIncomeTax(decimal ordinaryIncome);

    /// <summary>
    /// Calculates the federal tax on long-term capital gains.
    /// </summary>
    /// <param name="capitalGains">The amount of long-term capital gains</param>
    /// <param name="ordinaryIncome">The ordinary income (used to determine LTCG bracket)</param>
    /// <returns>The calculated tax amount</returns>
    decimal CalculateLongTermCapitalGainsTax(decimal capitalGains, decimal ordinaryIncome);

    /// <summary>
    /// Calculates total tax liability for a given withdrawal amount.
    /// </summary>
    /// <param name="taxableAccountWithdrawal">Amount withdrawn from taxable accounts (subject to LTCG)</param>
    /// <param name="taxDeferredAccountWithdrawal">Amount withdrawn from tax-deferred accounts (ordinary income)</param>
    /// <returns>Total federal tax liability</returns>
    decimal CalculateTotalTax(decimal taxableAccountWithdrawal, decimal taxDeferredAccountWithdrawal);

    /// <summary>
    /// Gets the standard deduction for single filing status.
    /// </summary>
    decimal GetStandardDeduction();
}
