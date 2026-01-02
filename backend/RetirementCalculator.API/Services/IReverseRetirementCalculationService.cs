using RetirementCalculator.API.Models;

namespace RetirementCalculator.API.Services
{
    public interface IReverseRetirementCalculationService
    {
        Task<ReverseCalculationResult> CalculateRequiredPortfolioAsync(ReverseCalculationInput input);
    }
}
