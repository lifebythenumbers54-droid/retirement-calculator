namespace RetirementCalculator.API.Services;

/// <summary>
/// Service for calculating Roth IRA conversion ladder strategies for early retirement
/// </summary>
public class RothConversionService : IRothConversionService
{
    private const decimal PENALTY_AGE_THRESHOLD = 59.5m;
    private const int ROTH_CONVERSION_SEASONING_YEARS = 5;
    private const decimal PENALTY_RATE = 0.10m;

    private readonly ILogger<RothConversionService> _logger;
    private readonly ITaxCalculationService _taxCalculationService;

    public RothConversionService(
        ILogger<RothConversionService> logger,
        ITaxCalculationService taxCalculationService)
    {
        _logger = logger;
        _taxCalculationService = taxCalculationService;
    }

    public RothConversionAnalysis EvaluateConversionStrategy(
        int retirementAge,
        decimal annualWithdrawalNeeded,
        decimal taxDeferredWithdrawal,
        decimal ordinaryIncomeTax,
        decimal earlyWithdrawalPenalty,
        int yearsWithPenalty)
    {
        _logger.LogInformation(
            "Evaluating Roth conversion strategy - RetirementAge: {RetirementAge}, AnnualNeed: {AnnualNeed}, YearsWithPenalty: {YearsWithPenalty}",
            retirementAge, annualWithdrawalNeeded, yearsWithPenalty);

        var analysis = new RothConversionAnalysis();

        // If no early retirement, no need for conversion ladder
        if (retirementAge >= PENALTY_AGE_THRESHOLD)
        {
            analysis.IsRecommended = false;
            analysis.StrategyExplanation = "Roth conversion ladder is not necessary. You are retiring at or after age 59.5, so no early withdrawal penalties apply.";
            return analysis;
        }

        // Calculate total years in early retirement (until 59.5)
        int yearsUntil595 = (int)Math.Ceiling(PENALTY_AGE_THRESHOLD - retirementAge);

        // Build conversion schedule
        analysis.ConversionSchedule = BuildConversionSchedule(
            retirementAge,
            annualWithdrawalNeeded,
            taxDeferredWithdrawal,
            yearsUntil595);

        // Calculate total conversion tax cost
        analysis.TotalConversionTaxCost = analysis.ConversionSchedule.Sum(y => y.ConversionTax);

        // Calculate total penalty approach cost (penalties + taxes over penalty years)
        analysis.TotalPenaltyCost = earlyWithdrawalPenalty + (ordinaryIncomeTax * yearsWithPenalty);

        // Calculate savings
        analysis.EstimatedSavings = analysis.TotalPenaltyCost - analysis.TotalConversionTaxCost;

        // Determine if recommended
        analysis.IsRecommended = analysis.EstimatedSavings > 0;

        // Check if user needs funds in first 5 years (before first conversion seasons)
        analysis.RequiresImmediateFunds = yearsUntil595 >= ROTH_CONVERSION_SEASONING_YEARS;

        // Generate explanations
        analysis.StrategyExplanation = GenerateStrategyExplanation(analysis, yearsUntil595);
        analysis.TransitionWarning = GenerateTransitionWarning(retirementAge, yearsUntil595);

        _logger.LogInformation(
            "Roth conversion analysis complete - IsRecommended: {IsRecommended}, Savings: {Savings}, ConversionTaxCost: {ConversionTaxCost}, PenaltyCost: {PenaltyCost}",
            analysis.IsRecommended, analysis.EstimatedSavings, analysis.TotalConversionTaxCost, analysis.TotalPenaltyCost);

        return analysis;
    }

    private List<ConversionYear> BuildConversionSchedule(
        int retirementAge,
        decimal annualWithdrawalNeeded,
        decimal taxDeferredWithdrawal,
        int yearsUntil595)
    {
        var schedule = new List<ConversionYear>();

        // We need to start converting 5 years before we need the funds
        // Model conversions for the full early retirement period plus 5 years lead time
        int totalYearsToModel = yearsUntil595 + ROTH_CONVERSION_SEASONING_YEARS;

        for (int year = 0; year < totalYearsToModel; year++)
        {
            int currentAge = retirementAge + year;
            var conversionYear = new ConversionYear
            {
                Year = year,
                Age = currentAge
            };

            // Determine conversion amount for this year
            if (currentAge < PENALTY_AGE_THRESHOLD)
            {
                // Convert enough to cover annual needs after 5-year seasoning
                conversionYear.ConversionAmount = taxDeferredWithdrawal;

                // Calculate tax on conversion (conversions are taxable as ordinary income)
                conversionYear.ConversionTax = _taxCalculationService.CalculateOrdinaryIncomeTax(
                    conversionYear.ConversionAmount);

                _logger.LogDebug(
                    "Year {Year} (Age {Age}): Converting {Amount}, Tax: {Tax}",
                    year, currentAge, conversionYear.ConversionAmount, conversionYear.ConversionTax);
            }
            else
            {
                // After 59.5, no need to convert (can withdraw directly without penalty)
                conversionYear.ConversionAmount = 0;
                conversionYear.ConversionTax = 0;
                conversionYear.Notes = "No conversion needed - age 59.5+ allows penalty-free withdrawals";
            }

            // Calculate what's available for withdrawal (conversions from 5 years ago)
            if (year >= ROTH_CONVERSION_SEASONING_YEARS)
            {
                int sourceYear = year - ROTH_CONVERSION_SEASONING_YEARS;
                if (sourceYear < schedule.Count)
                {
                    conversionYear.AvailableForWithdrawal = schedule[sourceYear].ConversionAmount;
                    conversionYear.Notes = $"Can withdraw ${conversionYear.AvailableForWithdrawal:N2} from Year {sourceYear} conversion (5-year seasoning complete)";
                }
            }
            else
            {
                // First 5 years: need alternative funding (taxable accounts, penalty withdrawals, etc.)
                conversionYear.AvailableForWithdrawal = 0;
                conversionYear.Notes = "Transition year - use taxable accounts or accept penalties if needed";
            }

            schedule.Add(conversionYear);
        }

        return schedule;
    }

    private string GenerateStrategyExplanation(RothConversionAnalysis analysis, int yearsUntil595)
    {
        if (!analysis.IsRecommended)
        {
            return $"The Roth conversion ladder strategy would cost ${analysis.TotalConversionTaxCost:N2} in conversion taxes, " +
                   $"compared to ${analysis.TotalPenaltyCost:N2} using the standard penalty approach. " +
                   $"The penalty approach is more cost-effective by ${Math.Abs(analysis.EstimatedSavings):N2}.";
        }

        return $"✓ Roth Conversion Ladder Recommended: By converting traditional IRA/401(k) funds to a Roth IRA each year " +
               $"and waiting 5 years for each conversion to season, you can avoid the 10% early withdrawal penalty. " +
               $"\n\nEstimated savings: ${analysis.EstimatedSavings:N2} " +
               $"(${analysis.TotalPenaltyCost:N2} penalty approach vs ${analysis.TotalConversionTaxCost:N2} conversion approach). " +
               $"\n\nYou would need to perform Roth conversions for {yearsUntil595} years (until age 59.5), " +
               $"paying ordinary income tax on each conversion in the year it occurs.";
    }

    private string GenerateTransitionWarning(int retirementAge, int yearsUntil595)
    {
        if (yearsUntil595 < ROTH_CONVERSION_SEASONING_YEARS)
        {
            return $"Since you only have {yearsUntil595} years until age 59.5, " +
                   $"you can start conversions immediately and they'll be available before you reach the penalty-free age.";
        }

        int transitionYears = ROTH_CONVERSION_SEASONING_YEARS;
        decimal transitionEndAge = retirementAge + transitionYears;

        return $"⚠ First {transitionYears} Years Transition Period: Roth conversions require a 5-year seasoning period before " +
               $"you can withdraw the converted principal penalty-free. " +
               $"\n\nFor the first 5 years of retirement (age {retirementAge} to {transitionEndAge}), you'll need to fund " +
               $"your expenses using:\n" +
               $"• Taxable account withdrawals (recommended)\n" +
               $"• Roth IRA contributions (if you made any - always penalty-free)\n" +
               $"• A mix of small penalty withdrawals if necessary\n\n" +
               $"Start converting to Roth IRA immediately upon retirement to begin the 5-year clock.";
    }
}
