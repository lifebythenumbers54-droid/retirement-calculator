using System.ComponentModel.DataAnnotations;

namespace RetirementCalculator.API.Models
{
    public class ReverseCalculationInput
    {
        [Required]
        [Range(10000, 10000000, ErrorMessage = "Desired after-tax annual income must be between $10,000 and $10,000,000")]
        public decimal DesiredAfterTaxIncome { get; set; }

        [Required]
        [Range(18, 100, ErrorMessage = "Current age must be between 18 and 100")]
        public int CurrentAge { get; set; }

        [Required]
        [Range(18, 100, ErrorMessage = "Retirement age must be between 18 and 100")]
        public int RetirementAge { get; set; }

        [Required]
        [Range(0.85, 0.98, ErrorMessage = "Success rate threshold must be between 0.85 (85%) and 0.98 (98%)")]
        public decimal SuccessRateThreshold { get; set; }

        // Optional: Current savings for gap analysis
        [Range(0, double.MaxValue, ErrorMessage = "Current retirement account balance cannot be negative")]
        public decimal? CurrentRetirementAccountBalance { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Current taxable account balance cannot be negative")]
        public decimal? CurrentTaxableAccountBalance { get; set; }

        // Optional: For savings roadmap
        [Range(0, double.MaxValue, ErrorMessage = "Annual savings cannot be negative")]
        public decimal? AnnualSavings { get; set; }

        // Risk profile preference (optional - if not specified, show all three)
        public RiskProfile? PreferredRiskProfile { get; set; }
    }

    public enum RiskProfile
    {
        Conservative,
        Moderate,
        Aggressive
    }
}
