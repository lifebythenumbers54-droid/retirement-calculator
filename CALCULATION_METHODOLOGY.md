# Calculation Methodology

## Overview

This document explains the mathematical and financial methodologies used by the Retirement Calculator to determine safe withdrawal rates and retirement income projections.

## Core Calculation Process

### 1. Historical Simulation Method

The calculator uses a **historical simulation** approach rather than Monte Carlo simulation with random distributions. This method:

- Analyzes actual historical market returns from 1925-2024
- Tests every possible retirement start year in the historical data
- Simulates a retirement period lasting from retirement age to age 95
- Uses real sequence-of-returns data to account for sequence risk

**Why Historical Simulation?**
- More realistic than purely random simulations
- Captures actual market behavior including crashes and recoveries
- Accounts for real-world correlations between stocks and bonds
- Includes actual inflation patterns

### 2. Withdrawal Rate Optimization

The calculator uses a binary search algorithm to find the optimal withdrawal rate:

```
1. Start with a range (e.g., 0% to 10%)
2. Test the midpoint withdrawal rate
3. Run historical simulations for all starting years
4. Calculate success rate (% of scenarios where money lasted)
5. If success rate > threshold: try a higher rate
6. If success rate < threshold: try a lower rate
7. Repeat until finding the highest rate that meets the threshold
```

This process typically completes in under 2 seconds by efficiently narrowing the search space.

### 3. Portfolio Assumptions

**Asset Allocation**: 60% stocks / 40% bonds
- Stocks: S&P 500 or equivalent broad market index
- Bonds: U.S. Government bonds or high-grade corporate bonds

**Rebalancing**: Annual rebalancing back to 60/40 allocation

**Returns**: Historical total returns (price appreciation + dividends/interest)

**Inflation**: Built into historical returns data

## Tax Calculation Methodology

### Federal Tax Framework

The calculator implements the 2024 federal income tax system with current rates and brackets.

#### Tax Brackets (Single Filer, 2024)
- 10%: $0 - $11,600
- 12%: $11,601 - $47,150
- 22%: $47,151 - $100,525
- 24%: $100,526 - $191,950
- 32%: $191,951 - $243,725
- 35%: $243,726 - $609,350
- 37%: $609,351+

#### Capital Gains Rates (2024)
- 0%: Income up to $47,025
- 15%: Income $47,026 - $518,900
- 20%: Income over $518,900

#### Standard Deduction
- $14,600 (2024 single filer)

### Tax-Optimized Withdrawal Strategy

The calculator implements a tax-efficient withdrawal strategy to minimize tax burden:

#### Step 1: Tax-Deferred Account Withdrawals
1. Calculate the amount that can be withdrawn from tax-deferred accounts (401k/IRA) while staying in the 12% tax bracket
2. This amount = (Top of 12% bracket + Standard Deduction) - Current taxable income
3. Withdraw up to this amount from tax-deferred accounts

**Rationale**: The 12% bracket is considered optimal as it's low enough to minimize taxes while utilizing available bracket space.

#### Step 2: Taxable Account Withdrawals
1. Any remaining withdrawal needed comes from taxable accounts
2. These withdrawals are taxed at long-term capital gains rates (typically lower)
3. Assumes 50% cost basis (conservative assumption for capital gains calculation)

**Rationale**: Capital gains rates are lower than ordinary income rates for most retirees, making taxable accounts more tax-efficient for amounts exceeding the 12% bracket.

### Tax Calculations

#### Ordinary Income Tax (Tax-Deferred Withdrawals)
```
1. Subtract standard deduction from withdrawal amount
2. Apply progressive tax brackets to remaining amount
3. Sum tax from each bracket
```

Example (simplified):
- Withdrawal: $40,000
- Less standard deduction: $40,000 - $14,600 = $25,400
- Tax on first $11,600 @ 10%: $1,160
- Tax on remaining $13,800 @ 12%: $1,656
- Total ordinary income tax: $2,816

#### Capital Gains Tax (Taxable Account Withdrawals)
```
1. Calculate capital gains = Withdrawal × (1 - Cost Basis %)
2. Add to ordinary income to determine tax bracket
3. Apply appropriate capital gains rate
```

Example:
- Withdrawal: $30,000
- Capital gains (50% cost basis): $15,000
- Return of principal: $15,000 (not taxed)
- If total income puts you in 15% capital gains bracket: $15,000 × 15% = $2,250

#### Effective Tax Rate
```
Effective Tax Rate = (Total Taxes / Gross Withdrawal) × 100
```

## Early Withdrawal Penalty Calculation

### IRS 10% Early Withdrawal Penalty

**Applicability**:
- Applies to tax-deferred account withdrawals (401k, IRA)
- Only for withdrawals before age 59.5
- Does NOT apply to taxable account withdrawals

### Calculation Process

```
For each year of retirement:
  If current_age < 59.5:
    penalty = tax_deferred_withdrawal × 0.10
    total_penalty += penalty
    years_with_penalty += 1
  Else:
    penalty = 0
```

### Penalty Impact

The penalty significantly reduces early retirement income:

Example:
- Tax-deferred withdrawal: $20,000/year
- Years before 59.5: 5 years
- Annual penalty: $2,000
- Total penalty over period: $10,000

**Note**: The calculator displays this prominently to help users make informed decisions about early retirement.

### Exceptions Not Modeled

The calculator does NOT account for penalty exceptions such as:
- 72(t) Substantially Equal Periodic Payments
- First-time home purchase
- Higher education expenses
- Medical expenses exceeding 7.5% of AGI
- Disability
- Death

These exceptions require personalized professional advice.

## Success Rate Calculation

### Definition

Success Rate = (Number of successful scenarios / Total scenarios tested) × 100

**Successful scenario**: Portfolio balance remains positive through age 95

### Historical Scenarios

For a retirement age of 65:
- Start years: Every year from 1925 to (2024 - retirement duration)
- Each scenario simulates retiring in that year and living 30 years to age 95
- Uses actual historical returns for stocks and bonds
- Withdraws inflation-adjusted amounts annually

### Example

Retirement age 65, current year 2024:
- Maximum start year: 2024 - 30 = 1994
- Total scenarios: 1994 - 1925 + 1 = 70 scenarios
- If portfolio survives in 67 scenarios: Success Rate = 95.7%

### Threshold Options

**90% Success Rate**:
- More aggressive
- 1 in 10 chance of running out of money
- Higher withdrawal rates

**95% Success Rate** (Recommended):
- Balanced approach
- 1 in 20 chance of running out of money
- Moderate withdrawal rates

**98% Success Rate**:
- Conservative
- 1 in 50 chance of running out of money
- Lower withdrawal rates

## Portfolio Simulation Process

### Annual Simulation Steps

For each historical scenario:

```
1. Initialize portfolio with user's starting balances
2. For each year from retirement age to 95:
   a. Calculate required withdrawal (optimized between accounts)
   b. Apply tax-deferred withdrawal penalty if age < 59.5
   c. Calculate taxes (ordinary income + capital gains)
   d. Determine net income
   e. Deduct withdrawal from portfolio
   f. Apply historical returns for that year
   g. Rebalance to 60/40
   h. Check if portfolio depleted
3. Record success/failure
```

### Withdrawal Sequence

1. **Determine total needed**: Based on withdrawal rate and total portfolio
2. **Optimize withdrawal source**: Use tax-efficient strategy
3. **Calculate taxes**: Apply federal tax code
4. **Net to user**: Gross withdrawal - taxes = net income

### Portfolio Depletion Check

A scenario is considered **failed** if:
- Portfolio balance drops to $0 before age 95
- Insufficient funds to support withdrawal in any year

## Limitations and Assumptions

### Assumptions Made

1. **Life Expectancy**: Always assumes living to age 95
2. **No Other Income**: Withdrawals are the only income source
3. **No Expenses Beyond Withdrawal**: Doesn't model specific expense categories
4. **Fixed Allocation**: 60/40 maintained throughout retirement
5. **Annual Withdrawals**: Assumes one withdrawal per year
6. **No Fees**: Investment fees and advisor costs not included
7. **Perfect Rebalancing**: No transaction costs for rebalancing
8. **Federal Taxes Only**: No state, local, or payroll taxes
9. **Current Tax Code**: Uses 2024 tax rates and brackets
10. **Full Liquidity**: Assumes all assets can be sold instantly

### What's NOT Included

- Social Security benefits
- Pension income
- Part-time work income
- Required Minimum Distributions (RMDs) starting at 73
- Healthcare costs (including Medicare premiums)
- Long-term care expenses
- Inflation adjustments to tax brackets
- Changes to tax law
- Estate planning considerations
- Market timing or tactical allocation
- Behavioral factors (panic selling, etc.)

## Scientific Basis

### Trinity Study (1998)

The methodology is inspired by the **Trinity Study** (Cooley, Hubbard, and Walz):
- Analyzed safe withdrawal rates using historical data
- Tested various portfolio allocations and time periods
- Established the foundation for the "4% rule"

**Reference**: Cooley, P. L., Hubbard, C. M., & Walz, D. T. (1998). "Retirement Savings: Choosing a Withdrawal Rate That Is Sustainable."

### Bengen's Research (1994)

William Bengen's original research on safe withdrawal rates:
- Analyzed historical market data from 1926-1992
- Determined 4% as a historically safe withdrawal rate
- Considered sequence-of-returns risk

**Reference**: Bengen, W. P. (1994). "Determining Withdrawal Rates Using Historical Data."

### Modern Extensions

This calculator extends classic research by:
- Using more recent data (through 2024)
- Incorporating tax optimization
- Modeling early withdrawal penalties
- Providing multiple success rate thresholds
- Offering detailed tax breakdowns

## Performance Considerations

### Optimization Techniques

1. **Binary Search**: Efficiently finds optimal withdrawal rate
2. **Data Caching**: Historical market data loaded once at startup
3. **Parallel Processing**: Could be added for even faster calculations
4. **Response Compression**: Reduces network transfer time

### Typical Performance

- API calculation time: < 1 second
- Frontend rendering: < 0.5 seconds
- Total user experience: < 2 seconds

## Validation and Testing

### Calculation Accuracy

Results have been validated against:
- Published safe withdrawal rate research
- Online retirement calculators
- Manual spreadsheet calculations
- Financial planning software

### Edge Cases Tested

- Very early retirement (age 40)
- Very late retirement (age 70)
- Minimal balances ($50,000)
- Large balances ($5,000,000+)
- All success rate thresholds
- Various combinations of account types

## Future Enhancements

Potential improvements to methodology:

1. **Variable Portfolio Allocations**: Allow custom stock/bond mixes
2. **Glide Path Strategies**: Adjust allocation as age increases
3. **Social Security Integration**: Include SS benefits in calculations
4. **RMD Compliance**: Model Required Minimum Distributions
5. **State Tax Modeling**: Add state and local taxes
6. **Healthcare Costs**: Include Medicare and supplemental insurance
7. **Inflation Scenarios**: Test different inflation assumptions
8. **Dynamic Withdrawal Rules**: Implement variable percentage rules
9. **Roth Account Optimization**: Add Roth IRA withdrawal strategies
10. **Monte Carlo Options**: Offer probabilistic scenarios beyond historical

## Conclusion

The Retirement Calculator provides a scientifically-grounded, tax-aware estimate of safe withdrawal rates based on historical market data. While it makes simplifying assumptions, it offers valuable insights for retirement planning when used as part of a comprehensive financial planning process.

**Remember**: This is a tool for estimation and education. Always consult with qualified financial and tax professionals for personalized advice.
