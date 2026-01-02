using RetirementCalculator.API.Models;

namespace RetirementCalculator.API.Services
{
    public class ReverseRetirementCalculationService : IReverseRetirementCalculationService
    {
        private readonly IWithdrawalCalculationService _withdrawalService;
        private readonly ITaxCalculationService _taxService;
        private readonly IEarlyRetirementService _earlyRetirementService;
        private readonly IRothConversionService _rothConversionService;
        private readonly IAllocationAnalysisService _allocationService;

        public ReverseRetirementCalculationService(
            IWithdrawalCalculationService withdrawalService,
            ITaxCalculationService taxService,
            IEarlyRetirementService earlyRetirementService,
            IRothConversionService rothConversionService,
            IAllocationAnalysisService allocationService)
        {
            _withdrawalService = withdrawalService;
            _taxService = taxService;
            _earlyRetirementService = earlyRetirementService;
            _rothConversionService = rothConversionService;
            _allocationService = allocationService;
        }

        public async Task<ReverseCalculationResult> CalculateRequiredPortfolioAsync(ReverseCalculationInput input)
        {
            var result = new ReverseCalculationResult();

            // Define risk profile configurations
            var riskProfiles = new List<(string Name, decimal MinSuccessRate, decimal PreferredAllocation)>
            {
                ("Conservative", 0.95m, 0.40m), // 40% stocks, target 95%+ success
                ("Moderate", 0.90m, 0.60m),     // 60% stocks, target 90%+ success
                ("Aggressive", 0.85m, 0.80m)    // 80% stocks, target 85%+ success
            };

            // If user specified a preferred risk profile, only calculate that one
            if (input.PreferredRiskProfile.HasValue)
            {
                var profileName = input.PreferredRiskProfile.Value.ToString();
                var profile = riskProfiles.FirstOrDefault(p => p.Name == profileName);
                if (profile != default)
                {
                    riskProfiles = new List<(string, decimal, decimal)> { profile };
                }
            }

            // Calculate required portfolio for each risk profile
            foreach (var (name, minSuccessRate, preferredAllocation) in riskProfiles)
            {
                var scenario = await CalculateScenarioAsync(input, name, minSuccessRate, preferredAllocation);
                if (scenario != null)
                {
                    result.Scenarios.Add(scenario);
                }
            }

            // Perform gap analysis if current savings provided
            if (input.CurrentRetirementAccountBalance.HasValue || input.CurrentTaxableAccountBalance.HasValue)
            {
                result.GapAnalysis = CalculateGapAnalysis(input, result.Scenarios);
            }

            // Generate summary
            result.Summary = GenerateSummary(input, result);

            return result;
        }

        private async Task<RiskProfileScenario?> CalculateScenarioAsync(
            ReverseCalculationInput input,
            string riskProfileName,
            decimal targetSuccessRate,
            decimal stockAllocation)
        {
            try
            {
                // Step 1: Use binary search to find required portfolio size
                var requiredPortfolio = await FindRequiredPortfolioSizeAsync(
                    input.DesiredAfterTaxIncome,
                    input.RetirementAge,
                    input.CurrentAge,
                    targetSuccessRate,
                    stockAllocation);

                if (requiredPortfolio <= 0)
                {
                    return null;
                }

                // Step 2: Calculate detailed results using the required portfolio
                var retirementYears = 95 - input.RetirementAge; // Assume life expectancy of 95
                var withdrawalResult = await _withdrawalService.FindOptimalWithdrawalRateAsync(
                    requiredPortfolio,
                    0, // All in retirement account for simplicity
                    retirementYears,
                    targetSuccessRate,
                    stockAllocation);

                // Step 3: Calculate taxes
                var grossWithdrawal = withdrawalResult.AnnualGrossWithdrawal;
                var taxes = _taxService.CalculateTotalTax(grossWithdrawal, 0);
                var afterTaxIncome = grossWithdrawal - taxes;

                // Step 4: Check for early retirement penalties
                decimal? earlyWithdrawalPenalty = null;
                int? yearsWithPenalty = null;
                string? penaltyWarning = null;
                decimal totalPenalty = 0;

                if (input.RetirementAge < 59)
                {
                    var (penalty, years) = _earlyRetirementService.CalculateTotalPenalties(
                        input.RetirementAge,
                        retirementYears,
                        withdrawalResult.TaxDeferredAccountWithdrawal);

                    totalPenalty = penalty;
                    earlyWithdrawalPenalty = penalty / years; // Annual penalty
                    yearsWithPenalty = years;
                    penaltyWarning = _earlyRetirementService.GetPenaltyWarning(input.RetirementAge);

                    // Adjust after-tax income for annual penalty
                    afterTaxIncome -= earlyWithdrawalPenalty.Value;
                }

                // Step 5: Analyze Roth conversion strategy for early retirement
                RothConversionAnalysis? rothAnalysis = null;
                if (input.RetirementAge < 59 && yearsWithPenalty > 0)
                {
                    rothAnalysis = _rothConversionService.EvaluateConversionStrategy(
                        input.RetirementAge,
                        grossWithdrawal,
                        withdrawalResult.TaxDeferredAccountWithdrawal,
                        withdrawalResult.OrdinaryIncomeTax,
                        totalPenalty,
                        yearsWithPenalty.Value);
                }

                // Step 6: Get portfolio statistics
                var stats = await _withdrawalService.CalculatePortfolioStatisticsAsync(
                    requiredPortfolio,
                    0,
                    retirementYears,
                    withdrawalResult.WithdrawalRate,
                    stockAllocation);

                // Step 7: Build scenario
                var scenario = new RiskProfileScenario
                {
                    RiskProfile = riskProfileName,
                    RequiredPortfolioSize = Math.Round(requiredPortfolio, 2),
                    WithdrawalRate = Math.Round(withdrawalResult.WithdrawalRate * 100, 2), // Convert to percentage
                    AnnualPreTaxWithdrawal = Math.Round(grossWithdrawal, 2),
                    AnnualAfterTaxIncome = Math.Round(afterTaxIncome, 2),
                    EstimatedAnnualTaxes = Math.Round(taxes, 2),
                    EffectiveTaxRate = grossWithdrawal > 0 ? Math.Round((taxes / grossWithdrawal) * 100, 2) : 0,
                    HistoricalSuccessRate = Math.Round(withdrawalResult.AchievedSuccessRate * 100, 2),
                    StockAllocationPercent = (int)(stockAllocation * 100),
                    BondAllocationPercent = (int)((1 - stockAllocation) * 100),
                    MedianFinalPortfolioValue = Math.Round(stats.MedianFinalValue, 2),
                    WorstCaseScenario = Math.Round(stats.WorstCaseValue, 2),
                    BestCaseScenario = Math.Round(stats.BestCaseValue, 2),
                    EarlyWithdrawalPenalty = earlyWithdrawalPenalty.HasValue ? Math.Round(earlyWithdrawalPenalty.Value, 2) : null,
                    YearsWithPenalty = yearsWithPenalty,
                    PenaltyWarning = penaltyWarning,
                    RothConversionAnalysis = rothAnalysis,
                    Recommendation = GenerateScenarioRecommendation(riskProfileName, withdrawalResult.AchievedSuccessRate, rothAnalysis)
                };

                return scenario;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating scenario for {riskProfileName}: {ex.Message}");
                return null;
            }
        }

        private async Task<decimal> FindRequiredPortfolioSizeAsync(
            decimal desiredAfterTaxIncome,
            int retirementAge,
            int currentAge,
            decimal targetSuccessRate,
            decimal stockAllocation)
        {
            // Binary search parameters
            decimal minPortfolio = 100000m; // Start at $100k
            decimal maxPortfolio = 50000000m; // Max $50M
            decimal tolerance = 1000m; // $1000 tolerance
            int maxIterations = 50;

            var retirementYears = 95 - retirementAge; // Assume life expectancy of 95

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                decimal testPortfolio = (minPortfolio + maxPortfolio) / 2;

                // Calculate withdrawal result for this portfolio size
                var withdrawalResult = await _withdrawalService.FindOptimalWithdrawalRateAsync(
                    testPortfolio,
                    0, // All in retirement account
                    retirementYears,
                    targetSuccessRate,
                    stockAllocation);

                // Calculate after-tax income
                var grossWithdrawal = withdrawalResult.AnnualGrossWithdrawal;
                var taxes = _taxService.CalculateTotalTax(grossWithdrawal, 0);
                var afterTaxIncome = grossWithdrawal - taxes;

                // Account for early withdrawal penalty if applicable
                if (retirementAge < 59)
                {
                    var taxDeferredWithdrawal = grossWithdrawal; // Assuming all from tax-deferred for simplicity
                    var (totalPenalty, years) = _earlyRetirementService.CalculateTotalPenalties(
                        retirementAge,
                        retirementYears,
                        taxDeferredWithdrawal);

                    if (years > 0)
                    {
                        var annualPenalty = totalPenalty / years;
                        afterTaxIncome -= annualPenalty;
                    }
                }

                // Check if we're close enough
                decimal incomeDifference = afterTaxIncome - desiredAfterTaxIncome;

                if (Math.Abs(incomeDifference) <= tolerance)
                {
                    return testPortfolio; // Found it!
                }

                // Adjust search range
                if (afterTaxIncome < desiredAfterTaxIncome)
                {
                    // Need more portfolio
                    minPortfolio = testPortfolio;
                }
                else
                {
                    // Have too much portfolio
                    maxPortfolio = testPortfolio;
                }

                // Check if search range is too small
                if (maxPortfolio - minPortfolio < tolerance)
                {
                    return (minPortfolio + maxPortfolio) / 2;
                }
            }

            // Return best estimate after max iterations
            return (minPortfolio + maxPortfolio) / 2;
        }

        private GapAnalysis CalculateGapAnalysis(ReverseCalculationInput input, List<RiskProfileScenario> scenarios)
        {
            decimal currentSavings = (input.CurrentRetirementAccountBalance ?? 0) +
                                    (input.CurrentTaxableAccountBalance ?? 0);

            var conservative = scenarios.FirstOrDefault(s => s.RiskProfile == "Conservative");
            var moderate = scenarios.FirstOrDefault(s => s.RiskProfile == "Moderate");
            var aggressive = scenarios.FirstOrDefault(s => s.RiskProfile == "Aggressive");

            var gapAnalysis = new GapAnalysis
            {
                CurrentTotalSavings = currentSavings,
                RequiredAmountConservative = conservative?.RequiredPortfolioSize ?? 0,
                RequiredAmountModerate = moderate?.RequiredPortfolioSize ?? 0,
                RequiredAmountAggressive = aggressive?.RequiredPortfolioSize ?? 0,
                GapConservative = (conservative?.RequiredPortfolioSize ?? 0) - currentSavings,
                GapModerate = (moderate?.RequiredPortfolioSize ?? 0) - currentSavings,
                GapAggressive = (aggressive?.RequiredPortfolioSize ?? 0) - currentSavings
            };

            // Calculate savings roadmap if annual savings provided
            if (input.AnnualSavings.HasValue && input.AnnualSavings.Value > 0)
            {
                gapAnalysis.SavingsRoadmap = CalculateSavingsRoadmap(
                    input.CurrentAge,
                    input.RetirementAge,
                    currentSavings,
                    input.AnnualSavings.Value,
                    gapAnalysis);
            }

            return gapAnalysis;
        }

        private SavingsRoadmap CalculateSavingsRoadmap(
            int currentAge,
            int retirementAge,
            decimal currentSavings,
            decimal annualSavings,
            GapAnalysis gapAnalysis)
        {
            int yearsUntilRetirement = retirementAge - currentAge;

            // Simple calculation: years = gap / annual savings
            // (Note: This doesn't account for investment growth, which would be more complex)
            int? yearsToConservative = null;
            int? yearsToModerate = null;
            int? yearsToAggressive = null;

            if (annualSavings > 0)
            {
                if (gapAnalysis.GapConservative > 0)
                    yearsToConservative = (int)Math.Ceiling(gapAnalysis.GapConservative / annualSavings);
                if (gapAnalysis.GapModerate > 0)
                    yearsToModerate = (int)Math.Ceiling(gapAnalysis.GapModerate / annualSavings);
                if (gapAnalysis.GapAggressive > 0)
                    yearsToAggressive = (int)Math.Ceiling(gapAnalysis.GapAggressive / annualSavings);
            }

            // Calculate required monthly savings to reach each goal by retirement
            decimal? requiredMonthlyConservative = null;
            decimal? requiredMonthlyModerate = null;
            decimal? requiredMonthlyAggressive = null;

            if (yearsUntilRetirement > 0)
            {
                if (gapAnalysis.GapConservative > 0)
                    requiredMonthlyConservative = gapAnalysis.GapConservative / (yearsUntilRetirement * 12);
                if (gapAnalysis.GapModerate > 0)
                    requiredMonthlyModerate = gapAnalysis.GapModerate / (yearsUntilRetirement * 12);
                if (gapAnalysis.GapAggressive > 0)
                    requiredMonthlyAggressive = gapAnalysis.GapAggressive / (yearsUntilRetirement * 12);
            }

            var roadmap = new SavingsRoadmap
            {
                AnnualSavingsAmount = annualSavings,
                YearsUntilRetirement = yearsUntilRetirement,
                YearsToReachConservativeGoal = yearsToConservative,
                YearsToReachModerateGoal = yearsToModerate,
                YearsToReachAggressiveGoal = yearsToAggressive,
                RequiredMonthlySavingsConservative = requiredMonthlyConservative.HasValue ? Math.Round(requiredMonthlyConservative.Value, 2) : null,
                RequiredMonthlySavingsModerate = requiredMonthlyModerate.HasValue ? Math.Round(requiredMonthlyModerate.Value, 2) : null,
                RequiredMonthlySavingsAggressive = requiredMonthlyAggressive.HasValue ? Math.Round(requiredMonthlyAggressive.Value, 2) : null,
                Recommendation = GenerateSavingsRecommendation(yearsUntilRetirement, yearsToModerate, requiredMonthlyModerate)
            };

            return roadmap;
        }

        private string GenerateScenarioRecommendation(string riskProfile, decimal successRate, RothConversionAnalysis? rothAnalysis)
        {
            var recommendation = $"{riskProfile} strategy has {successRate:F1}% historical success rate. ";

            if (rothAnalysis != null && rothAnalysis.IsRecommended)
            {
                recommendation += $"Consider Roth conversion ladder to save ${rothAnalysis.EstimatedSavings:N0} in penalties.";
            }

            return recommendation;
        }

        private string GenerateSavingsRecommendation(int yearsUntilRetirement, int? yearsToModerate, decimal? requiredMonthlyModerate)
        {
            if (yearsToModerate.HasValue && yearsToModerate.Value <= yearsUntilRetirement)
            {
                return $"At your current savings rate, you'll reach the Moderate goal in {yearsToModerate.Value} years, before your planned retirement.";
            }
            else if (requiredMonthlyModerate.HasValue)
            {
                return $"To reach the Moderate goal by retirement, save ${requiredMonthlyModerate.Value:N0} per month (${requiredMonthlyModerate.Value * 12:N0} annually).";
            }

            return "Increase your annual savings to reach your retirement goals on time.";
        }

        private string GenerateSummary(ReverseCalculationInput input, ReverseCalculationResult result)
        {
            if (!result.Scenarios.Any())
            {
                return "Unable to calculate required portfolio. Please adjust your inputs.";
            }

            var moderate = result.Scenarios.FirstOrDefault(s => s.RiskProfile == "Moderate");
            if (moderate == null)
            {
                moderate = result.Scenarios.First();
            }

            var summary = $"To generate ${input.DesiredAfterTaxIncome:N0} per year after taxes in retirement, " +
                         $"you need approximately ${moderate.RequiredPortfolioSize:N0} " +
                         $"(based on {moderate.RiskProfile} strategy with {moderate.HistoricalSuccessRate:F1}% success rate).";

            if (result.GapAnalysis != null && result.GapAnalysis.CurrentTotalSavings > 0)
            {
                var gap = result.GapAnalysis.GapModerate;
                if (gap > 0)
                {
                    summary += $" You currently have ${result.GapAnalysis.CurrentTotalSavings:N0} saved, " +
                              $"leaving a gap of ${gap:N0}.";
                }
                else
                {
                    summary += $" Good news: Your current savings of ${result.GapAnalysis.CurrentTotalSavings:N0} " +
                              $"exceeds this requirement!";
                }
            }

            return summary;
        }
    }
}
