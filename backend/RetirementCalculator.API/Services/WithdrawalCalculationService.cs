using RetirementCalculator.API.Models;

namespace RetirementCalculator.API.Services;

public class WithdrawalCalculationService : IWithdrawalCalculationService
{
    private readonly IHistoricalDataService _historicalDataService;
    private readonly ITaxCalculationService _taxCalculationService;
    private readonly ILogger<WithdrawalCalculationService> _logger;

    // Default portfolio allocation
    private const decimal STOCK_ALLOCATION = 0.60m;
    private const decimal BOND_ALLOCATION = 0.40m;

    // Store stock allocation for custom allocations
    private decimal _currentStockAllocation = STOCK_ALLOCATION;

    public WithdrawalCalculationService(
        IHistoricalDataService historicalDataService,
        ITaxCalculationService taxCalculationService,
        ILogger<WithdrawalCalculationService> logger)
    {
        _historicalDataService = historicalDataService;
        _taxCalculationService = taxCalculationService;
        _logger = logger;
    }

    public async Task<CalculationResult> CalculateWithdrawalStrategy(UserInput userInput)
    {
        _logger.LogInformation(
            "Calculating withdrawal strategy for retirement age {RetirementAge}, balance ${TotalBalance}, threshold {Threshold}%",
            userInput.RetirementAge,
            userInput.RetirementAccountBalance + userInput.TaxableAccountBalance,
            userInput.SuccessRateThreshold * 100);

        var totalBalance = userInput.RetirementAccountBalance + userInput.TaxableAccountBalance;
        var retirementYearsExpected = 95 - userInput.RetirementAge; // Assume living to 95

        // Find optimal withdrawal rate using binary search
        var optimalWithdrawalRate = await FindOptimalWithdrawalRate(
            totalBalance,
            retirementYearsExpected,
            userInput.SuccessRateThreshold * 100); // Convert to percentage

        var annualGrossWithdrawal = totalBalance * optimalWithdrawalRate;

        // Calculate tax-optimized withdrawal amounts from each account
        var (taxableWithdrawal, taxDeferredWithdrawal) = CalculateTaxOptimizedWithdrawal(
            annualGrossWithdrawal,
            userInput.TaxableAccountBalance,
            userInput.RetirementAccountBalance);

        // Calculate detailed tax breakdown
        var ordinaryIncomeTax = _taxCalculationService.CalculateOrdinaryIncomeTax(taxDeferredWithdrawal);
        var capitalGainsTax = _taxCalculationService.CalculateLongTermCapitalGainsTax(taxableWithdrawal, taxDeferredWithdrawal);
        var totalTax = ordinaryIncomeTax + capitalGainsTax;
        var effectiveTaxRate = annualGrossWithdrawal > 0 ? (totalTax / annualGrossWithdrawal) * 100 : 0;

        var result = new CalculationResult
        {
            WithdrawalRate = optimalWithdrawalRate * 100, // Convert to percentage
            AnnualGrossWithdrawal = annualGrossWithdrawal,
            EstimatedAnnualTaxes = totalTax,
            NetAnnualIncome = annualGrossWithdrawal - totalTax,
            AchievedSuccessRate = await CalculateSuccessRate(totalBalance, retirementYearsExpected, optimalWithdrawalRate),
            NumberOfScenariosSimulated = GetNumberOfScenarios(retirementYearsExpected),
            TaxableAccountWithdrawal = taxableWithdrawal,
            TaxDeferredAccountWithdrawal = taxDeferredWithdrawal,
            OrdinaryIncomeTax = ordinaryIncomeTax,
            CapitalGainsTax = capitalGainsTax,
            EffectiveTaxRate = effectiveTaxRate
        };

        _logger.LogInformation(
            "Calculation complete: {WithdrawalRate}% withdrawal rate, {SuccessRate}% success rate over {Scenarios} scenarios, " +
            "TaxableWithdrawal=${TaxableWithdrawal}, TaxDeferredWithdrawal=${TaxDeferredWithdrawal}, EffectiveTaxRate={EffectiveTaxRate}%",
            result.WithdrawalRate,
            result.AchievedSuccessRate,
            result.NumberOfScenariosSimulated,
            taxableWithdrawal,
            taxDeferredWithdrawal,
            effectiveTaxRate);

        return result;
    }

    private async Task<decimal> FindOptimalWithdrawalRate(
        decimal totalBalance,
        int retirementYears,
        decimal targetSuccessRate)
    {
        // Binary search for optimal withdrawal rate
        decimal minRate = 0.01m; // 1%
        decimal maxRate = 0.15m; // 15%
        decimal tolerance = 0.0001m; // 0.01%
        decimal bestRate = minRate;

        while (maxRate - minRate > tolerance)
        {
            decimal midRate = (minRate + maxRate) / 2;
            decimal successRate = await CalculateSuccessRate(totalBalance, retirementYears, midRate);

            _logger.LogInformation("Binary search: Testing withdrawal rate {Rate}% - Success rate: {SuccessRate}% (target: {Target}%)",
                midRate * 100, successRate, targetSuccessRate);

            if (successRate >= targetSuccessRate)
            {
                // Success rate is high enough, try withdrawing more
                bestRate = midRate;
                minRate = midRate;
                _logger.LogInformation("Success rate acceptable, trying higher withdrawal rate");
            }
            else
            {
                // Success rate too low, need to withdraw less
                maxRate = midRate;
                _logger.LogInformation("Success rate too low, trying lower withdrawal rate");
            }
        }

        _logger.LogInformation("Binary search complete: Best rate = {Rate}%", bestRate * 100);

        return bestRate;
    }

    private async Task<decimal> CalculateSuccessRate(
        decimal initialBalance,
        int retirementYears,
        decimal withdrawalRate)
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
                if (SimulateRetirement(initialBalance, withdrawalRate, startYear, retirementYears))
                {
                    successfulScenarios++;
                }
                totalScenarios++;
            }

            return totalScenarios > 0 ? (decimal)successfulScenarios / totalScenarios * 100 : 0;
        });
    }

    private bool SimulateRetirement(
        decimal initialBalance,
        decimal withdrawalRate,
        int startYear,
        int retirementYears)
    {
        decimal balance = initialBalance;
        decimal annualWithdrawal = initialBalance * withdrawalRate;

        for (int year = 0; year < retirementYears; year++)
        {
            int historicalYear = startYear + year;
            var marketData = _historicalDataService.GetDataForYear(historicalYear);

            if (marketData == null)
            {
                // If we run out of historical data, consider it a failure
                return false;
            }

            // Take withdrawal at beginning of year
            balance -= annualWithdrawal;

            // Check if portfolio is depleted
            if (balance <= 0)
            {
                return false;
            }

            // Calculate portfolio return based on allocation
            decimal portfolioReturn =
                (STOCK_ALLOCATION * marketData.Sp500Return) +
                (BOND_ALLOCATION * marketData.BondReturn);

            // Apply returns
            balance *= (1 + portfolioReturn);

            // Adjust withdrawal for inflation
            annualWithdrawal *= (1 + marketData.Inflation);
        }

        // Success if we still have money left
        return balance > 0;
    }

    private int GetNumberOfScenarios(int retirementYears)
    {
        var (minYear, maxYear) = _historicalDataService.GetYearRange();
        return Math.Max(0, maxYear - minYear - retirementYears + 1);
    }

    /// <summary>
    /// Calculates the tax-optimized withdrawal amounts from taxable and tax-deferred accounts.
    /// Strategy: Fill lower tax brackets with tax-deferred income first, then use taxable account
    /// to benefit from lower capital gains rates.
    /// </summary>
    private (decimal taxableWithdrawal, decimal taxDeferredWithdrawal) CalculateTaxOptimizedWithdrawal(
        decimal totalWithdrawalNeeded,
        decimal taxableAccountBalance,
        decimal taxDeferredAccountBalance)
    {
        // If one account is empty, withdraw from the other
        if (taxDeferredAccountBalance <= 0)
        {
            return (totalWithdrawalNeeded, 0m);
        }
        if (taxableAccountBalance <= 0)
        {
            return (0m, totalWithdrawalNeeded);
        }

        // Strategy: Withdraw from tax-deferred account up to the top of the 12% bracket
        // (which is effectively ~0% after standard deduction), then use taxable account for the rest
        // to benefit from 0% or 15% LTCG rates

        decimal standardDeduction = _taxCalculationService.GetStandardDeduction();

        // Optimal amount to withdraw from tax-deferred: enough to fill up to ~12% bracket
        // This is approximately the standard deduction + first two tax brackets
        decimal optimalTaxDeferredWithdrawal = Math.Min(
            standardDeduction + 48475m, // Standard deduction + 12% bracket limit
            Math.Min(totalWithdrawalNeeded, taxDeferredAccountBalance));

        // Remaining withdrawal comes from taxable account
        decimal taxableWithdrawal = Math.Min(
            totalWithdrawalNeeded - optimalTaxDeferredWithdrawal,
            taxableAccountBalance);

        // If taxable account can't cover the rest, take more from tax-deferred
        decimal taxDeferredWithdrawal = totalWithdrawalNeeded - taxableWithdrawal;

        _logger.LogDebug(
            "Tax-optimized withdrawal: Total={Total}, Taxable={Taxable}, TaxDeferred={TaxDeferred}",
            totalWithdrawalNeeded, taxableWithdrawal, taxDeferredWithdrawal);

        return (taxableWithdrawal, taxDeferredWithdrawal);
    }

    public async Task<CalculationResult> FindOptimalWithdrawalRateAsync(
        decimal retirementAccountBalance,
        decimal taxableAccountBalance,
        int retirementYears,
        decimal targetSuccessRate,
        decimal stockAllocation)
    {
        // Set the stock allocation for this calculation
        _currentStockAllocation = stockAllocation;

        var totalBalance = retirementAccountBalance + taxableAccountBalance;

        // Find optimal withdrawal rate using binary search
        var optimalWithdrawalRate = await FindOptimalWithdrawalRateWithAllocation(
            totalBalance,
            retirementYears,
            targetSuccessRate * 100, // Convert to percentage
            stockAllocation);

        var annualGrossWithdrawal = totalBalance * optimalWithdrawalRate;

        // Calculate tax-optimized withdrawal amounts from each account
        var (taxableWithdrawal, taxDeferredWithdrawal) = CalculateTaxOptimizedWithdrawal(
            annualGrossWithdrawal,
            taxableAccountBalance,
            retirementAccountBalance);

        // Calculate detailed tax breakdown
        var ordinaryIncomeTax = _taxCalculationService.CalculateOrdinaryIncomeTax(taxDeferredWithdrawal);
        var capitalGainsTax = _taxCalculationService.CalculateLongTermCapitalGainsTax(taxableWithdrawal, taxDeferredWithdrawal);
        var totalTax = ordinaryIncomeTax + capitalGainsTax;
        var effectiveTaxRate = annualGrossWithdrawal > 0 ? (totalTax / annualGrossWithdrawal) * 100 : 0;

        var result = new CalculationResult
        {
            WithdrawalRate = optimalWithdrawalRate * 100, // Convert to percentage
            AnnualGrossWithdrawal = annualGrossWithdrawal,
            EstimatedAnnualTaxes = totalTax,
            NetAnnualIncome = annualGrossWithdrawal - totalTax,
            AchievedSuccessRate = await CalculateSuccessRateWithAllocation(totalBalance, retirementYears, optimalWithdrawalRate, stockAllocation),
            NumberOfScenariosSimulated = GetNumberOfScenarios(retirementYears),
            TaxableAccountWithdrawal = taxableWithdrawal,
            TaxDeferredAccountWithdrawal = taxDeferredWithdrawal,
            OrdinaryIncomeTax = ordinaryIncomeTax,
            CapitalGainsTax = capitalGainsTax,
            EffectiveTaxRate = effectiveTaxRate
        };

        // Reset to default allocation
        _currentStockAllocation = STOCK_ALLOCATION;

        return result;
    }

    public async Task<PortfolioStatistics> CalculatePortfolioStatisticsAsync(
        decimal retirementAccountBalance,
        decimal taxableAccountBalance,
        int retirementYears,
        decimal withdrawalRate,
        decimal stockAllocation)
    {
        return await Task.Run(() =>
        {
            var totalBalance = retirementAccountBalance + taxableAccountBalance;
            var historicalData = _historicalDataService.GetHistoricalData();
            var (minYear, maxYear) = _historicalDataService.GetYearRange();

            var finalValues = new List<decimal>();

            // Rolling window analysis
            for (int startYear = minYear; startYear <= maxYear - retirementYears; startYear++)
            {
                var finalValue = SimulateRetirementWithFinalValue(totalBalance, withdrawalRate, startYear, retirementYears, stockAllocation);
                finalValues.Add(finalValue);
            }

            finalValues.Sort();

            return new PortfolioStatistics
            {
                MedianFinalValue = finalValues.Count > 0 ? finalValues[finalValues.Count / 2] : 0,
                WorstCaseValue = finalValues.Count > 0 ? finalValues.First() : 0,
                BestCaseValue = finalValues.Count > 0 ? finalValues.Last() : 0
            };
        });
    }

    private async Task<decimal> FindOptimalWithdrawalRateWithAllocation(
        decimal totalBalance,
        int retirementYears,
        decimal targetSuccessRate,
        decimal stockAllocation)
    {
        // Binary search for optimal withdrawal rate
        decimal minRate = 0.01m; // 1%
        decimal maxRate = 0.15m; // 15%
        decimal tolerance = 0.0001m; // 0.01%
        decimal bestRate = minRate;

        while (maxRate - minRate > tolerance)
        {
            decimal midRate = (minRate + maxRate) / 2;
            decimal successRate = await CalculateSuccessRateWithAllocation(totalBalance, retirementYears, midRate, stockAllocation);

            if (successRate >= targetSuccessRate)
            {
                // Success rate is high enough, try withdrawing more
                bestRate = midRate;
                minRate = midRate;
            }
            else
            {
                // Success rate too low, need to withdraw less
                maxRate = midRate;
            }
        }

        return bestRate;
    }

    private async Task<decimal> CalculateSuccessRateWithAllocation(
        decimal initialBalance,
        int retirementYears,
        decimal withdrawalRate,
        decimal stockAllocation)
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
                if (SimulateRetirementWithAllocation(initialBalance, withdrawalRate, startYear, retirementYears, stockAllocation))
                {
                    successfulScenarios++;
                }
                totalScenarios++;
            }

            return totalScenarios > 0 ? (decimal)successfulScenarios / totalScenarios * 100 : 0;
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
        decimal bondAllocation = 1 - stockAllocation;

        for (int year = 0; year < retirementYears; year++)
        {
            int historicalYear = startYear + year;
            var marketData = _historicalDataService.GetDataForYear(historicalYear);

            if (marketData == null)
            {
                return false;
            }

            // Take withdrawal at beginning of year
            balance -= annualWithdrawal;

            // Check if portfolio is depleted
            if (balance <= 0)
            {
                return false;
            }

            // Calculate portfolio return based on allocation
            decimal portfolioReturn =
                (stockAllocation * marketData.Sp500Return) +
                (bondAllocation * marketData.BondReturn);

            // Apply returns
            balance *= (1 + portfolioReturn);

            // Adjust withdrawal for inflation
            annualWithdrawal *= (1 + marketData.Inflation);
        }

        // Success if we still have money left
        return balance > 0;
    }

    private decimal SimulateRetirementWithFinalValue(
        decimal initialBalance,
        decimal withdrawalRate,
        int startYear,
        int retirementYears,
        decimal stockAllocation)
    {
        decimal balance = initialBalance;
        decimal annualWithdrawal = initialBalance * withdrawalRate;
        decimal bondAllocation = 1 - stockAllocation;

        for (int year = 0; year < retirementYears; year++)
        {
            int historicalYear = startYear + year;
            var marketData = _historicalDataService.GetDataForYear(historicalYear);

            if (marketData == null)
            {
                return 0;
            }

            // Take withdrawal at beginning of year
            balance -= annualWithdrawal;

            // Check if portfolio is depleted
            if (balance <= 0)
            {
                return 0;
            }

            // Calculate portfolio return based on allocation
            decimal portfolioReturn =
                (stockAllocation * marketData.Sp500Return) +
                (bondAllocation * marketData.BondReturn);

            // Apply returns
            balance *= (1 + portfolioReturn);

            // Adjust withdrawal for inflation
            annualWithdrawal *= (1 + marketData.Inflation);
        }

        return balance;
    }
}
