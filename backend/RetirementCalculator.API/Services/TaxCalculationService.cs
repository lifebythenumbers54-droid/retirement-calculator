namespace RetirementCalculator.API.Services;

/// <summary>
/// Service for calculating federal income taxes on retirement account withdrawals.
/// Implements 2025 federal tax brackets and long-term capital gains rates.
/// </summary>
public class TaxCalculationService : ITaxCalculationService
{
    // 2025 Federal Tax Brackets for Single Filers (Ordinary Income)
    private const decimal STANDARD_DEDUCTION = 15000m;

    // Tax bracket limits
    private const decimal BRACKET_10_LIMIT = 11925m;
    private const decimal BRACKET_12_LIMIT = 48475m;
    private const decimal BRACKET_22_LIMIT = 103350m;
    private const decimal BRACKET_24_LIMIT = 197300m;
    private const decimal BRACKET_32_LIMIT = 250525m;
    private const decimal BRACKET_35_LIMIT = 626350m;
    // Above this is 37%

    // Tax rates
    private const decimal RATE_10 = 0.10m;
    private const decimal RATE_12 = 0.12m;
    private const decimal RATE_22 = 0.22m;
    private const decimal RATE_24 = 0.24m;
    private const decimal RATE_32 = 0.32m;
    private const decimal RATE_35 = 0.35m;
    private const decimal RATE_37 = 0.37m;

    // 2025 Long-Term Capital Gains Tax Brackets for Single Filers
    private const decimal LTCG_0_PERCENT_LIMIT = 48350m;
    private const decimal LTCG_15_PERCENT_LIMIT = 533400m;
    // Above this is 20%

    private const decimal LTCG_RATE_0 = 0.00m;
    private const decimal LTCG_RATE_15 = 0.15m;
    private const decimal LTCG_RATE_20 = 0.20m;

    private readonly ILogger<TaxCalculationService> _logger;

    public TaxCalculationService(ILogger<TaxCalculationService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the standard deduction for single filing status.
    /// </summary>
    public decimal GetStandardDeduction()
    {
        return STANDARD_DEDUCTION;
    }

    /// <summary>
    /// Calculates the total federal tax on ordinary income using progressive tax brackets.
    /// </summary>
    /// <param name="ordinaryIncome">The amount of ordinary income subject to tax</param>
    /// <returns>The calculated tax amount</returns>
    public decimal CalculateOrdinaryIncomeTax(decimal ordinaryIncome)
    {
        // Apply standard deduction
        decimal taxableIncome = Math.Max(0, ordinaryIncome - STANDARD_DEDUCTION);

        if (taxableIncome <= 0)
        {
            return 0m;
        }

        decimal tax = 0m;

        // 10% bracket
        if (taxableIncome <= BRACKET_10_LIMIT)
        {
            tax = taxableIncome * RATE_10;
        }
        // 12% bracket
        else if (taxableIncome <= BRACKET_12_LIMIT)
        {
            tax = BRACKET_10_LIMIT * RATE_10 +
                  (taxableIncome - BRACKET_10_LIMIT) * RATE_12;
        }
        // 22% bracket
        else if (taxableIncome <= BRACKET_22_LIMIT)
        {
            tax = BRACKET_10_LIMIT * RATE_10 +
                  (BRACKET_12_LIMIT - BRACKET_10_LIMIT) * RATE_12 +
                  (taxableIncome - BRACKET_12_LIMIT) * RATE_22;
        }
        // 24% bracket
        else if (taxableIncome <= BRACKET_24_LIMIT)
        {
            tax = BRACKET_10_LIMIT * RATE_10 +
                  (BRACKET_12_LIMIT - BRACKET_10_LIMIT) * RATE_12 +
                  (BRACKET_22_LIMIT - BRACKET_12_LIMIT) * RATE_22 +
                  (taxableIncome - BRACKET_22_LIMIT) * RATE_24;
        }
        // 32% bracket
        else if (taxableIncome <= BRACKET_32_LIMIT)
        {
            tax = BRACKET_10_LIMIT * RATE_10 +
                  (BRACKET_12_LIMIT - BRACKET_10_LIMIT) * RATE_12 +
                  (BRACKET_22_LIMIT - BRACKET_12_LIMIT) * RATE_22 +
                  (BRACKET_24_LIMIT - BRACKET_22_LIMIT) * RATE_24 +
                  (taxableIncome - BRACKET_24_LIMIT) * RATE_32;
        }
        // 35% bracket
        else if (taxableIncome <= BRACKET_35_LIMIT)
        {
            tax = BRACKET_10_LIMIT * RATE_10 +
                  (BRACKET_12_LIMIT - BRACKET_10_LIMIT) * RATE_12 +
                  (BRACKET_22_LIMIT - BRACKET_12_LIMIT) * RATE_22 +
                  (BRACKET_24_LIMIT - BRACKET_22_LIMIT) * RATE_24 +
                  (BRACKET_32_LIMIT - BRACKET_24_LIMIT) * RATE_32 +
                  (taxableIncome - BRACKET_32_LIMIT) * RATE_35;
        }
        // 37% bracket
        else
        {
            tax = BRACKET_10_LIMIT * RATE_10 +
                  (BRACKET_12_LIMIT - BRACKET_10_LIMIT) * RATE_12 +
                  (BRACKET_22_LIMIT - BRACKET_12_LIMIT) * RATE_22 +
                  (BRACKET_24_LIMIT - BRACKET_22_LIMIT) * RATE_24 +
                  (BRACKET_32_LIMIT - BRACKET_24_LIMIT) * RATE_32 +
                  (BRACKET_35_LIMIT - BRACKET_32_LIMIT) * RATE_35 +
                  (taxableIncome - BRACKET_35_LIMIT) * RATE_37;
        }

        _logger.LogDebug("Ordinary income tax: Income={Income}, TaxableIncome={TaxableIncome}, Tax={Tax}",
            ordinaryIncome, taxableIncome, tax);

        return Math.Round(tax, 2);
    }

    /// <summary>
    /// Calculates the federal tax on long-term capital gains.
    /// The LTCG bracket is determined by the combination of ordinary income and capital gains.
    /// </summary>
    /// <param name="capitalGains">The amount of long-term capital gains</param>
    /// <param name="ordinaryIncome">The ordinary income (used to determine LTCG bracket)</param>
    /// <returns>The calculated tax amount</returns>
    public decimal CalculateLongTermCapitalGainsTax(decimal capitalGains, decimal ordinaryIncome)
    {
        if (capitalGains <= 0)
        {
            return 0m;
        }

        // For LTCG, we stack gains on top of ordinary income (after standard deduction)
        decimal taxableOrdinaryIncome = Math.Max(0, ordinaryIncome - STANDARD_DEDUCTION);
        decimal totalIncome = taxableOrdinaryIncome + capitalGains;

        decimal tax = 0m;

        // Determine which portion falls into each LTCG bracket
        // 0% bracket
        if (totalIncome <= LTCG_0_PERCENT_LIMIT)
        {
            // All gains are taxed at 0%
            tax = 0m;
        }
        else if (taxableOrdinaryIncome >= LTCG_0_PERCENT_LIMIT)
        {
            // All ordinary income already filled the 0% bracket, so all gains are at 15% or higher
            if (totalIncome <= LTCG_15_PERCENT_LIMIT)
            {
                tax = capitalGains * LTCG_RATE_15;
            }
            else if (taxableOrdinaryIncome >= LTCG_15_PERCENT_LIMIT)
            {
                // All gains are in the 20% bracket
                tax = capitalGains * LTCG_RATE_20;
            }
            else
            {
                // Some gains at 15%, some at 20%
                decimal gainsAt15 = LTCG_15_PERCENT_LIMIT - taxableOrdinaryIncome;
                decimal gainsAt20 = capitalGains - gainsAt15;
                tax = gainsAt15 * LTCG_RATE_15 + gainsAt20 * LTCG_RATE_20;
            }
        }
        else
        {
            // Ordinary income doesn't fill 0% bracket, so some gains are at 0%
            decimal gainsAt0 = LTCG_0_PERCENT_LIMIT - taxableOrdinaryIncome;
            decimal remainingGains = capitalGains - gainsAt0;

            if (remainingGains <= 0)
            {
                // All gains are in 0% bracket
                tax = 0m;
            }
            else if (totalIncome <= LTCG_15_PERCENT_LIMIT)
            {
                // Remaining gains at 15%
                tax = remainingGains * LTCG_RATE_15;
            }
            else
            {
                // Some gains at 15%, some at 20%
                decimal gainsAt15 = LTCG_15_PERCENT_LIMIT - LTCG_0_PERCENT_LIMIT;
                decimal gainsAt20 = totalIncome - LTCG_15_PERCENT_LIMIT;
                tax = gainsAt15 * LTCG_RATE_15 + gainsAt20 * LTCG_RATE_20;
            }
        }

        _logger.LogDebug("LTCG tax: CapitalGains={CapitalGains}, OrdinaryIncome={OrdinaryIncome}, Tax={Tax}",
            capitalGains, ordinaryIncome, tax);

        return Math.Round(tax, 2);
    }

    /// <summary>
    /// Calculates total tax liability for a given withdrawal amount.
    /// Assumes taxable account withdrawals represent capital gains and tax-deferred withdrawals are ordinary income.
    /// </summary>
    /// <param name="taxableAccountWithdrawal">Amount withdrawn from taxable accounts (subject to LTCG)</param>
    /// <param name="taxDeferredAccountWithdrawal">Amount withdrawn from tax-deferred accounts (ordinary income)</param>
    /// <returns>Total federal tax liability</returns>
    public decimal CalculateTotalTax(decimal taxableAccountWithdrawal, decimal taxDeferredAccountWithdrawal)
    {
        decimal ordinaryIncomeTax = CalculateOrdinaryIncomeTax(taxDeferredAccountWithdrawal);
        decimal capitalGainsTax = CalculateLongTermCapitalGainsTax(taxableAccountWithdrawal, taxDeferredAccountWithdrawal);

        decimal totalTax = ordinaryIncomeTax + capitalGainsTax;

        _logger.LogInformation(
            "Total tax calculated: TaxableWithdrawal={TaxableWithdrawal}, TaxDeferredWithdrawal={TaxDeferredWithdrawal}, " +
            "OrdinaryIncomeTax={OrdinaryIncomeTax}, CapitalGainsTax={CapitalGainsTax}, TotalTax={TotalTax}",
            taxableAccountWithdrawal, taxDeferredAccountWithdrawal, ordinaryIncomeTax, capitalGainsTax, totalTax);

        return totalTax;
    }
}
