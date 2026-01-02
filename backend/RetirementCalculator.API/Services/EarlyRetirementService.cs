namespace RetirementCalculator.API.Services;

/// <summary>
/// Service for calculating early retirement penalties (10% penalty for withdrawals before age 59.5)
/// </summary>
public class EarlyRetirementService : IEarlyRetirementService
{
    private const decimal PENALTY_AGE_THRESHOLD = 59.5m;
    private const decimal PENALTY_RATE = 0.10m; // 10% penalty

    private readonly ILogger<EarlyRetirementService> _logger;

    public EarlyRetirementService(ILogger<EarlyRetirementService> logger)
    {
        _logger = logger;
    }

    public bool IsEarlyRetirement(int currentAge, int retirementAge)
    {
        bool isEarly = retirementAge < PENALTY_AGE_THRESHOLD;
        _logger.LogDebug("IsEarlyRetirement: RetirementAge={RetirementAge}, Threshold={Threshold}, IsEarly={IsEarly}",
            retirementAge, PENALTY_AGE_THRESHOLD, isEarly);
        return isEarly;
    }

    public decimal CalculateAgeInRetirementYear(int retirementAge, int yearIntoRetirement)
    {
        return retirementAge + yearIntoRetirement;
    }

    public bool IsPenaltyAge(decimal age)
    {
        return age < PENALTY_AGE_THRESHOLD;
    }

    public decimal CalculatePenalty(decimal taxDeferredWithdrawal)
    {
        if (taxDeferredWithdrawal <= 0)
        {
            return 0;
        }

        decimal penalty = taxDeferredWithdrawal * PENALTY_RATE;
        _logger.LogDebug("CalculatePenalty: Withdrawal={Withdrawal}, Penalty={Penalty}",
            taxDeferredWithdrawal, penalty);
        return penalty;
    }

    public (decimal totalPenalty, int yearsWithPenalty) CalculateTotalPenalties(
        int retirementAge,
        int retirementYears,
        decimal annualTaxDeferredWithdrawal)
    {
        decimal totalPenalty = 0;
        int yearsWithPenalty = 0;

        for (int year = 0; year < retirementYears; year++)
        {
            decimal ageInYear = CalculateAgeInRetirementYear(retirementAge, year);

            if (IsPenaltyAge(ageInYear))
            {
                totalPenalty += CalculatePenalty(annualTaxDeferredWithdrawal);
                yearsWithPenalty++;
            }
            else
            {
                // Once we pass the penalty age, no more penalties
                break;
            }
        }

        _logger.LogInformation(
            "CalculateTotalPenalties: RetirementAge={RetirementAge}, YearsWithPenalty={YearsWithPenalty}, TotalPenalty={TotalPenalty}",
            retirementAge, yearsWithPenalty, totalPenalty);

        return (totalPenalty, yearsWithPenalty);
    }

    public string GetPenaltyWarning(int retirementAge)
    {
        if (!IsEarlyRetirement(0, retirementAge))
        {
            return string.Empty;
        }

        decimal yearsUntilNoPenalty = PENALTY_AGE_THRESHOLD - retirementAge;

        return $"Warning: Retiring at age {retirementAge} means you'll face a 10% early withdrawal penalty " +
               $"on tax-deferred account withdrawals for approximately {Math.Ceiling(yearsUntilNoPenalty)} years " +
               $"(until age {PENALTY_AGE_THRESHOLD}).";
    }

    public string GetPenaltyExplanation(int yearsWithPenalty, decimal totalPenalty)
    {
        if (yearsWithPenalty == 0)
        {
            return "No early withdrawal penalties apply. You are withdrawing at or after age 59.5.";
        }

        return $"Early withdrawal penalty: 10% of tax-deferred withdrawals for {yearsWithPenalty} year{(yearsWithPenalty != 1 ? "s" : "")}. " +
               $"Total estimated penalties: ${totalPenalty:N2}. " +
               $"Strategy: Minimize penalties by withdrawing from taxable accounts first until age 59.5.";
    }
}
