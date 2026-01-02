using System.ComponentModel.DataAnnotations;

namespace RetirementCalculator.API.Models;

/// <summary>
/// User input data for retirement calculation
/// </summary>
public class UserInput
{
    /// <summary>
    /// Current age of the user (must be between 18 and 100)
    /// </summary>
    [Required(ErrorMessage = "Current age is required")]
    [Range(18, 100, ErrorMessage = "Current age must be between 18 and 100")]
    public int CurrentAge { get; set; }

    /// <summary>
    /// Age at which the user plans to retire (must be greater than or equal to current age)
    /// </summary>
    [Required(ErrorMessage = "Retirement age is required")]
    [Range(18, 100, ErrorMessage = "Retirement age must be between 18 and 100")]
    public int RetirementAge { get; set; }

    /// <summary>
    /// Total balance in retirement accounts (401k, IRA, etc.)
    /// </summary>
    [Required(ErrorMessage = "Retirement account balance is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Retirement account balance must be a positive number")]
    public decimal RetirementAccountBalance { get; set; }

    /// <summary>
    /// Total balance in taxable investment accounts
    /// </summary>
    [Required(ErrorMessage = "Taxable account balance is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Taxable account balance must be a positive number")]
    public decimal TaxableAccountBalance { get; set; }

    /// <summary>
    /// Desired success rate threshold (0.90 for 90%, 0.95 for 95%, 0.98 for 98%)
    /// </summary>
    [Required(ErrorMessage = "Success rate threshold is required")]
    [Range(0.90, 0.98, ErrorMessage = "Success rate threshold must be 0.90, 0.95, or 0.98")]
    public decimal SuccessRateThreshold { get; set; }

    /// <summary>
    /// Custom validation to ensure retirement age is not less than current age
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (RetirementAge < CurrentAge)
        {
            yield return new ValidationResult(
                "Retirement age must be greater than or equal to current age",
                new[] { nameof(RetirementAge) }
            );
        }
    }
}
