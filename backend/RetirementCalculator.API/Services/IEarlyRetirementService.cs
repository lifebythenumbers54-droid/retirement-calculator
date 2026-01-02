namespace RetirementCalculator.API.Services;

/// <summary>
/// Service for calculating early retirement penalties
/// </summary>
public interface IEarlyRetirementService
{
    /// <summary>
    /// Determines if the user is subject to early withdrawal penalties (under age 59.5)
    /// </summary>
    /// <param name="currentAge">Current age of the user</param>
    /// <param name="retirementAge">Age at which user plans to retire</param>
    /// <returns>True if user will face early withdrawal penalties</returns>
    bool IsEarlyRetirement(int currentAge, int retirementAge);

    /// <summary>
    /// Calculates the age at a given year of retirement
    /// </summary>
    /// <param name="retirementAge">Age at retirement</param>
    /// <param name="yearIntoRetirement">Year into retirement (0 for first year)</param>
    /// <returns>Age during that retirement year</returns>
    decimal CalculateAgeInRetirementYear(int retirementAge, int yearIntoRetirement);

    /// <summary>
    /// Determines if early withdrawal penalty applies at a given age
    /// </summary>
    /// <param name="age">Age to check (can include months as decimal)</param>
    /// <returns>True if penalty applies (age < 59.5)</returns>
    bool IsPenaltyAge(decimal age);

    /// <summary>
    /// Calculates the 10% early withdrawal penalty on tax-deferred withdrawals
    /// </summary>
    /// <param name="taxDeferredWithdrawal">Amount withdrawn from tax-deferred accounts</param>
    /// <returns>Penalty amount (10% of withdrawal)</returns>
    decimal CalculatePenalty(decimal taxDeferredWithdrawal);

    /// <summary>
    /// Calculates total penalties over the retirement period
    /// </summary>
    /// <param name="retirementAge">Age at retirement</param>
    /// <param name="retirementYears">Total years in retirement</param>
    /// <param name="annualTaxDeferredWithdrawal">Annual withdrawal from tax-deferred accounts</param>
    /// <returns>Tuple of (total penalty, years with penalty)</returns>
    (decimal totalPenalty, int yearsWithPenalty) CalculateTotalPenalties(
        int retirementAge,
        int retirementYears,
        decimal annualTaxDeferredWithdrawal);

    /// <summary>
    /// Generates a warning message for early retirement
    /// </summary>
    /// <param name="retirementAge">Age at retirement</param>
    /// <returns>Warning message</returns>
    string GetPenaltyWarning(int retirementAge);

    /// <summary>
    /// Generates an explanation of the penalty
    /// </summary>
    /// <param name="yearsWithPenalty">Number of years subject to penalty</param>
    /// <param name="totalPenalty">Total penalty amount</param>
    /// <returns>Explanation text</returns>
    string GetPenaltyExplanation(int yearsWithPenalty, decimal totalPenalty);
}
