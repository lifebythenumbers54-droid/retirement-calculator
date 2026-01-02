using RetirementCalculator.API.Models;

namespace RetirementCalculator.API.Services;

public class WithdrawalCalculationService : IWithdrawalCalculationService
{
    private readonly IHistoricalDataService _historicalDataService;
    private readonly ILogger<WithdrawalCalculationService> _logger;

    // Default portfolio allocation
    private const decimal STOCK_ALLOCATION = 0.60m;
    private const decimal BOND_ALLOCATION = 0.40m;

    // Tax calculation constants (simplified federal tax brackets for 2024)
    private const decimal STANDARD_DEDUCTION_SINGLE = 14600m;
    private const decimal TAX_BRACKET_10_LIMIT = 11600m;
    private const decimal TAX_BRACKET_12_LIMIT = 47150m;
    private const decimal TAX_BRACKET_22_LIMIT = 100525m;

    public WithdrawalCalculationService(
        IHistoricalDataService historicalDataService,
        ILogger<WithdrawalCalculationService> logger)
    {
        _historicalDataService = historicalDataService;
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
        var estimatedTaxes = CalculateEstimatedTaxes(
            annualGrossWithdrawal,
            userInput.RetirementAccountBalance,
            totalBalance);

        var result = new CalculationResult
        {
            WithdrawalRate = optimalWithdrawalRate * 100, // Convert to percentage
            AnnualGrossWithdrawal = annualGrossWithdrawal,
            EstimatedAnnualTaxes = estimatedTaxes,
            NetAnnualIncome = annualGrossWithdrawal - estimatedTaxes,
            AchievedSuccessRate = await CalculateSuccessRate(totalBalance, retirementYearsExpected, optimalWithdrawalRate),
            NumberOfScenariosSimulated = GetNumberOfScenarios(retirementYearsExpected)
        };

        _logger.LogInformation(
            "Calculation complete: {WithdrawalRate}% withdrawal rate, {SuccessRate}% success rate over {Scenarios} scenarios",
            result.WithdrawalRate,
            result.AchievedSuccessRate,
            result.NumberOfScenariosSimulated);

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

    private decimal CalculateEstimatedTaxes(
        decimal annualWithdrawal,
        decimal retirementAccountBalance,
        decimal totalBalance)
    {
        // Calculate proportion of withdrawal from tax-deferred accounts
        decimal taxDeferredProportion = totalBalance > 0
            ? retirementAccountBalance / totalBalance
            : 0;

        decimal taxableIncome = annualWithdrawal * taxDeferredProportion;

        // Subtract standard deduction
        decimal taxableIncomeAfterDeduction = Math.Max(0, taxableIncome - STANDARD_DEDUCTION_SINGLE);

        decimal tax = 0;

        // Calculate tax using simplified federal brackets
        if (taxableIncomeAfterDeduction <= TAX_BRACKET_10_LIMIT)
        {
            tax = taxableIncomeAfterDeduction * 0.10m;
        }
        else if (taxableIncomeAfterDeduction <= TAX_BRACKET_12_LIMIT)
        {
            tax = TAX_BRACKET_10_LIMIT * 0.10m +
                  (taxableIncomeAfterDeduction - TAX_BRACKET_10_LIMIT) * 0.12m;
        }
        else if (taxableIncomeAfterDeduction <= TAX_BRACKET_22_LIMIT)
        {
            tax = TAX_BRACKET_10_LIMIT * 0.10m +
                  (TAX_BRACKET_12_LIMIT - TAX_BRACKET_10_LIMIT) * 0.12m +
                  (taxableIncomeAfterDeduction - TAX_BRACKET_12_LIMIT) * 0.22m;
        }
        else
        {
            tax = TAX_BRACKET_10_LIMIT * 0.10m +
                  (TAX_BRACKET_12_LIMIT - TAX_BRACKET_10_LIMIT) * 0.12m +
                  (TAX_BRACKET_22_LIMIT - TAX_BRACKET_12_LIMIT) * 0.22m +
                  (taxableIncomeAfterDeduction - TAX_BRACKET_22_LIMIT) * 0.24m;
        }

        return tax;
    }
}
