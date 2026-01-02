using RetirementCalculator.API.Services;

namespace RetirementCalculator.API.Models
{
    public class ReverseCalculationResult
    {
        public List<RiskProfileScenario> Scenarios { get; set; } = new();
        public GapAnalysis? GapAnalysis { get; set; }
        public string Summary { get; set; } = string.Empty;
    }

    public class RiskProfileScenario
    {
        public string RiskProfile { get; set; } = string.Empty;
        public decimal RequiredPortfolioSize { get; set; }
        public decimal WithdrawalRate { get; set; }
        public decimal AnnualPreTaxWithdrawal { get; set; }
        public decimal AnnualAfterTaxIncome { get; set; }
        public decimal EstimatedAnnualTaxes { get; set; }
        public decimal EffectiveTaxRate { get; set; }
        public decimal HistoricalSuccessRate { get; set; }
        public int StockAllocationPercent { get; set; }
        public int BondAllocationPercent { get; set; }
        public decimal MedianFinalPortfolioValue { get; set; }
        public decimal WorstCaseScenario { get; set; }
        public decimal BestCaseScenario { get; set; }

        // Early retirement specific
        public decimal? EarlyWithdrawalPenalty { get; set; }
        public int? YearsWithPenalty { get; set; }
        public string? PenaltyWarning { get; set; }

        // Roth conversion analysis
        public RothConversionAnalysis? RothConversionAnalysis { get; set; }

        public string Recommendation { get; set; } = string.Empty;
    }

    public class GapAnalysis
    {
        public decimal CurrentTotalSavings { get; set; }
        public decimal RequiredAmountConservative { get; set; }
        public decimal RequiredAmountModerate { get; set; }
        public decimal RequiredAmountAggressive { get; set; }

        public decimal GapConservative { get; set; }
        public decimal GapModerate { get; set; }
        public decimal GapAggressive { get; set; }

        public SavingsRoadmap? SavingsRoadmap { get; set; }
    }

    public class SavingsRoadmap
    {
        public decimal AnnualSavingsAmount { get; set; }
        public int YearsUntilRetirement { get; set; }

        // Years to reach target at current savings rate
        public int? YearsToReachConservativeGoal { get; set; }
        public int? YearsToReachModerateGoal { get; set; }
        public int? YearsToReachAggressiveGoal { get; set; }

        // Required monthly savings to reach each goal by retirement age
        public decimal? RequiredMonthlySavingsConservative { get; set; }
        public decimal? RequiredMonthlySavingsModerate { get; set; }
        public decimal? RequiredMonthlySavingsAggressive { get; set; }

        public string Recommendation { get; set; } = string.Empty;
    }
}
