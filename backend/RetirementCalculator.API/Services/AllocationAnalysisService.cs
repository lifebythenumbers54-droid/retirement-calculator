using RetirementCalculator.API.Models;

namespace RetirementCalculator.API.Services;

public class AllocationAnalysisService : IAllocationAnalysisService
{
    private readonly IHistoricalDataService _historicalDataService;
    private readonly ITaxCalculationService _taxCalculationService;
    private readonly ILogger<AllocationAnalysisService> _logger;

    // Different allocations to test (stock percentage)
    private static readonly decimal[] ALLOCATIONS_TO_TEST = new[]
    {
        0.30m, 0.40m, 0.50m, 0.60m, 0.70m, 0.80m, 0.90m, 1.00m
    };

    public AllocationAnalysisService(
        IHistoricalDataService historicalDataService,
        ITaxCalculationService taxCalculationService,
        ILogger<AllocationAnalysisService> logger)
    {
        _historicalDataService = historicalDataService;
        _taxCalculationService = taxCalculationService;
        _logger = logger;
    }

    public async Task<AllocationAnalysisResult> AnalyzeAllocations(UserInput userInput)
    {
        var totalBalance = userInput.RetirementAccountBalance + userInput.TaxableAccountBalance;
        var retirementYears = 95 - userInput.RetirementAge;
        var targetSuccessRate = userInput.SuccessRateThreshold * 100;

        _logger.LogInformation(
            "Analyzing allocations for retirement duration {Years} years, balance ${Balance}, target success rate {TargetRate}%",
            retirementYears, totalBalance, targetSuccessRate);

        // Analyze all allocations in parallel for performance
        var allocationResults = await Task.WhenAll(
            ALLOCATIONS_TO_TEST.Select(stockAlloc =>
                AnalyzeAllocation(stockAlloc, totalBalance, retirementYears, targetSuccessRate, userInput)));

        // Filter out null results and convert to list
        var validResults = allocationResults.Where(r => r != null).Cast<AllocationStrategy>().ToList();

        // Sort by score (descending)
        var sortedResults = validResults.OrderByDescending(r => r.Score).ToList();

        _logger.LogInformation("Analyzed {Count} allocations, found {Valid} valid results",
            ALLOCATIONS_TO_TEST.Length, validResults.Count);

        // Select top 3 with different risk profiles
        var conservative = SelectConservativeStrategy(sortedResults);
        var balanced = SelectBalancedStrategy(sortedResults);
        var aggressive = SelectAggressiveStrategy(sortedResults);

        return new AllocationAnalysisResult
        {
            Conservative = conservative,
            Balanced = balanced,
            Aggressive = aggressive,
            RetirementDuration = retirementYears,
            TargetSuccessRate = targetSuccessRate,
            TotalPortfolioBalance = totalBalance,
            AllocationsAnalyzed = ALLOCATIONS_TO_TEST.Length,
            Methodology = "Historical rolling period analysis using data from 1925-2024. " +
                         "Each allocation is tested against all historical retirement periods of the specified duration. " +
                         "Success is defined as maintaining a positive portfolio balance throughout retirement."
        };
    }

    public async Task<decimal> CalculateSuccessRateForAllocation(
        decimal stockAllocation,
        decimal totalBalance,
        decimal withdrawalRate,
        int retirementYears)
    {
        return await Task.Run(() =>
        {
            var historicalData = _historicalDataService.GetHistoricalData();
            var (minYear, maxYear) = _historicalDataService.GetYearRange();

            int successfulScenarios = 0;
            int totalScenarios = 0;

            // Rolling window analysis
            for (int startYear = minYear; startYear <= maxYear - retirementYears; startYear++)
            {
                if (SimulateRetirementWithAllocation(
                    totalBalance, withdrawalRate, startYear, retirementYears, stockAllocation))
                {
                    successfulScenarios++;
                }
                totalScenarios++;
            }

            return totalScenarios > 0 ? (decimal)successfulScenarios / totalScenarios * 100 : 0;
        });
    }

    private async Task<AllocationStrategy?> AnalyzeAllocation(
        decimal stockAllocation,
        decimal totalBalance,
        int retirementYears,
        decimal targetSuccessRate,
        UserInput userInput)
    {
        var bondAllocation = 1.0m - stockAllocation;

        _logger.LogDebug("Analyzing allocation: {Stock}% stocks / {Bond}% bonds",
            stockAllocation * 100, bondAllocation * 100);

        // Find optimal withdrawal rate for this allocation using binary search
        var optimalWithdrawalRate = await FindOptimalWithdrawalRateForAllocation(
            stockAllocation, totalBalance, retirementYears, targetSuccessRate);

        // Get detailed statistics for this allocation
        var stats = await CalculateAllocationStatistics(
            stockAllocation, totalBalance, optimalWithdrawalRate, retirementYears);

        if (stats == null)
        {
            return null;
        }

        // Calculate tax-optimized withdrawal
        var annualGrossWithdrawal = totalBalance * optimalWithdrawalRate;
        var (taxableWithdrawal, taxDeferredWithdrawal) = CalculateTaxOptimizedWithdrawal(
            annualGrossWithdrawal,
            userInput.TaxableAccountBalance,
            userInput.RetirementAccountBalance);

        var ordinaryIncomeTax = _taxCalculationService.CalculateOrdinaryIncomeTax(taxDeferredWithdrawal);
        var capitalGainsTax = _taxCalculationService.CalculateLongTermCapitalGainsTax(taxableWithdrawal, taxDeferredWithdrawal);
        var totalTax = ordinaryIncomeTax + capitalGainsTax;

        // Calculate score based on multiple factors
        var score = CalculateScore(stats.SuccessRate, stats.MedianFinalValue, stats.Volatility, totalBalance);

        var strategy = new AllocationStrategy
        {
            StockAllocation = stockAllocation * 100,
            BondAllocation = bondAllocation * 100,
            HistoricalSuccessRate = stats.SuccessRate,
            MedianFinalValue = stats.MedianFinalValue,
            BestCaseFinalValue = stats.BestCaseFinalValue,
            WorstCaseFinalValue = stats.WorstCaseFinalValue,
            AverageVolatility = stats.Volatility,
            SuccessfulScenarios = stats.SuccessfulScenarios,
            TotalScenarios = stats.TotalScenarios,
            FailurePeriods = stats.FailurePeriods,
            RecommendedWithdrawalRate = optimalWithdrawalRate * 100,
            ExpectedAnnualWithdrawal = annualGrossWithdrawal,
            ExpectedNetIncome = annualGrossWithdrawal - totalTax,
            Score = score
        };

        return strategy;
    }

    private async Task<decimal> FindOptimalWithdrawalRateForAllocation(
        decimal stockAllocation,
        decimal totalBalance,
        int retirementYears,
        decimal targetSuccessRate)
    {
        return await Task.Run(() =>
        {
            decimal minRate = 0.01m; // 1%
            decimal maxRate = 0.15m; // 15%
            decimal tolerance = 0.0001m; // 0.01%
            decimal bestRate = minRate;

            while (maxRate - minRate > tolerance)
            {
                decimal midRate = (minRate + maxRate) / 2;
                decimal successRate = CalculateSuccessRateSync(
                    stockAllocation, totalBalance, midRate, retirementYears);

                if (successRate >= targetSuccessRate)
                {
                    bestRate = midRate;
                    minRate = midRate;
                }
                else
                {
                    maxRate = midRate;
                }
            }

            return bestRate;
        });
    }

    private decimal CalculateSuccessRateSync(
        decimal stockAllocation,
        decimal totalBalance,
        decimal withdrawalRate,
        int retirementYears)
    {
        var (minYear, maxYear) = _historicalDataService.GetYearRange();

        int successfulScenarios = 0;
        int totalScenarios = 0;

        for (int startYear = minYear; startYear <= maxYear - retirementYears; startYear++)
        {
            if (SimulateRetirementWithAllocation(
                totalBalance, withdrawalRate, startYear, retirementYears, stockAllocation))
            {
                successfulScenarios++;
            }
            totalScenarios++;
        }

        return totalScenarios > 0 ? (decimal)successfulScenarios / totalScenarios * 100 : 0;
    }

    private async Task<AllocationStatistics?> CalculateAllocationStatistics(
        decimal stockAllocation,
        decimal totalBalance,
        decimal withdrawalRate,
        int retirementYears)
    {
        return await Task.Run(() =>
        {
            var (minYear, maxYear) = _historicalDataService.GetYearRange();
            var finalValues = new List<decimal>();
            var yearlyReturns = new List<decimal>();
            var failurePeriods = new List<string>();

            int successfulScenarios = 0;
            int totalScenarios = 0;

            for (int startYear = minYear; startYear <= maxYear - retirementYears; startYear++)
            {
                var (success, finalValue, avgReturn) = SimulateRetirementDetailed(
                    totalBalance, withdrawalRate, startYear, retirementYears, stockAllocation);

                if (success)
                {
                    successfulScenarios++;
                    finalValues.Add(finalValue);
                }
                else
                {
                    var endYear = startYear + retirementYears - 1;
                    failurePeriods.Add($"{startYear}-{endYear}");
                }

                yearlyReturns.Add(avgReturn);
                totalScenarios++;
            }

            if (finalValues.Count == 0)
            {
                return null; // No successful scenarios
            }

            finalValues.Sort();
            var medianIndex = finalValues.Count / 2;
            var medianFinalValue = finalValues.Count % 2 == 0
                ? (finalValues[medianIndex - 1] + finalValues[medianIndex]) / 2
                : finalValues[medianIndex];

            // Calculate volatility (standard deviation of returns)
            var avgYearlyReturn = yearlyReturns.Average();
            var variance = yearlyReturns.Sum(r => (r - avgYearlyReturn) * (r - avgYearlyReturn)) / yearlyReturns.Count;
            var volatility = (decimal)Math.Sqrt((double)variance) * 100;

            return new AllocationStatistics
            {
                SuccessRate = (decimal)successfulScenarios / totalScenarios * 100,
                MedianFinalValue = medianFinalValue,
                BestCaseFinalValue = finalValues.Last(),
                WorstCaseFinalValue = finalValues.First(),
                Volatility = volatility,
                SuccessfulScenarios = successfulScenarios,
                TotalScenarios = totalScenarios,
                FailurePeriods = failurePeriods
            };
        });
    }

    private bool SimulateRetirementWithAllocation(
        decimal initialBalance,
        decimal withdrawalRate,
        int startYear,
        int retirementYears,
        decimal stockAllocation)
    {
        decimal balance = initialBalance;
        decimal annualWithdrawal = initialBalance * withdrawalRate;
        decimal bondAllocation = 1.0m - stockAllocation;

        for (int year = 0; year < retirementYears; year++)
        {
            int historicalYear = startYear + year;
            var marketData = _historicalDataService.GetDataForYear(historicalYear);

            if (marketData == null)
            {
                return false;
            }

            balance -= annualWithdrawal;

            if (balance <= 0)
            {
                return false;
            }

            decimal portfolioReturn =
                (stockAllocation * marketData.Sp500Return) +
                (bondAllocation * marketData.BondReturn);

            balance *= (1 + portfolioReturn);
            annualWithdrawal *= (1 + marketData.Inflation);
        }

        return balance > 0;
    }

    private (bool success, decimal finalValue, decimal avgReturn) SimulateRetirementDetailed(
        decimal initialBalance,
        decimal withdrawalRate,
        int startYear,
        int retirementYears,
        decimal stockAllocation)
    {
        decimal balance = initialBalance;
        decimal annualWithdrawal = initialBalance * withdrawalRate;
        decimal bondAllocation = 1.0m - stockAllocation;
        var returns = new List<decimal>();

        for (int year = 0; year < retirementYears; year++)
        {
            int historicalYear = startYear + year;
            var marketData = _historicalDataService.GetDataForYear(historicalYear);

            if (marketData == null)
            {
                return (false, 0, 0);
            }

            balance -= annualWithdrawal;

            if (balance <= 0)
            {
                return (false, 0, returns.Count > 0 ? returns.Average() : 0);
            }

            decimal portfolioReturn =
                (stockAllocation * marketData.Sp500Return) +
                (bondAllocation * marketData.BondReturn);

            returns.Add(portfolioReturn);
            balance *= (1 + portfolioReturn);
            annualWithdrawal *= (1 + marketData.Inflation);
        }

        return (balance > 0, balance, returns.Average());
    }

    private decimal CalculateScore(decimal successRate, decimal medianFinalValue, decimal volatility, decimal initialBalance)
    {
        // Score based on multiple factors (0-100 scale)
        // Success rate: 60% weight
        // Final value relative to initial: 25% weight
        // Low volatility: 15% weight

        var successScore = successRate * 0.60m;

        var finalValueRatio = medianFinalValue / initialBalance;
        var finalValueScore = Math.Min(finalValueRatio * 10, 25); // Cap at 25 points

        var volatilityScore = Math.Max(0, 15 - (volatility * 0.5m)); // Lower volatility = higher score

        return successScore + finalValueScore + volatilityScore;
    }

    private AllocationStrategy SelectConservativeStrategy(List<AllocationStrategy> strategies)
    {
        // Conservative: Highest success rate and lowest volatility
        var conservative = strategies
            .OrderByDescending(s => s.HistoricalSuccessRate)
            .ThenBy(s => s.AverageVolatility)
            .First();

        conservative.Name = "Conservative";
        conservative.Description = "Prioritizes portfolio survival with highest success rate and lowest volatility. " +
            "Best for those who prioritize security over growth.";

        return conservative;
    }

    private AllocationStrategy SelectBalancedStrategy(List<AllocationStrategy> strategies)
    {
        // Balanced: Best overall score (balance of success, returns, and volatility)
        var balanced = strategies
            .OrderByDescending(s => s.Score)
            .First();

        balanced.Name = "Balanced";
        balanced.Description = "Optimal risk/reward ratio based on historical performance. " +
            "Balances growth potential with portfolio stability.";

        return balanced;
    }

    private AllocationStrategy SelectAggressiveStrategy(List<AllocationStrategy> strategies)
    {
        // Aggressive: Highest median final value while maintaining reasonable success rate
        var aggressive = strategies
            .Where(s => s.HistoricalSuccessRate >= 85) // Must have at least 85% success rate
            .OrderByDescending(s => s.MedianFinalValue)
            .ThenByDescending(s => s.StockAllocation)
            .FirstOrDefault() ?? strategies.OrderByDescending(s => s.MedianFinalValue).First();

        aggressive.Name = "Aggressive";
        aggressive.Description = "Maximizes growth potential while maintaining acceptable success rate. " +
            "Best for those comfortable with higher volatility for potentially greater returns.";

        return aggressive;
    }

    private (decimal taxableWithdrawal, decimal taxDeferredWithdrawal) CalculateTaxOptimizedWithdrawal(
        decimal totalWithdrawalNeeded,
        decimal taxableAccountBalance,
        decimal taxDeferredAccountBalance)
    {
        if (taxDeferredAccountBalance <= 0)
        {
            return (totalWithdrawalNeeded, 0m);
        }
        if (taxableAccountBalance <= 0)
        {
            return (0m, totalWithdrawalNeeded);
        }

        decimal standardDeduction = _taxCalculationService.GetStandardDeduction();
        decimal optimalTaxDeferredWithdrawal = Math.Min(
            standardDeduction + 48475m,
            Math.Min(totalWithdrawalNeeded, taxDeferredAccountBalance));

        decimal taxableWithdrawal = Math.Min(
            totalWithdrawalNeeded - optimalTaxDeferredWithdrawal,
            taxableAccountBalance);

        decimal taxDeferredWithdrawal = totalWithdrawalNeeded - taxableWithdrawal;

        return (taxableWithdrawal, taxDeferredWithdrawal);
    }

    private class AllocationStatistics
    {
        public decimal SuccessRate { get; set; }
        public decimal MedianFinalValue { get; set; }
        public decimal BestCaseFinalValue { get; set; }
        public decimal WorstCaseFinalValue { get; set; }
        public decimal Volatility { get; set; }
        public int SuccessfulScenarios { get; set; }
        public int TotalScenarios { get; set; }
        public List<string> FailurePeriods { get; set; } = new();
    }
}
