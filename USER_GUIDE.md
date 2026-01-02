# Retirement Calculator - User Guide

## Overview

The Retirement Calculator helps you determine a safe withdrawal rate for your retirement savings based on historical market data. It uses advanced Monte Carlo simulations analyzing over 100 years of market history to calculate how much you can safely withdraw each year while minimizing the risk of running out of money.

## How to Use the Calculator

### Step 1: Enter Your Information

Fill out the following fields in the calculator:

1. **Current Age** (18-100)
   - Your age today
   - Used to calculate early withdrawal penalties if retiring before 59.5

2. **Retirement Age** (must be ≥ current age)
   - The age at which you plan to retire
   - Must be at least your current age

3. **Retirement Account Balance** (401k/IRA)
   - Total balance in tax-deferred accounts
   - These withdrawals will be taxed as ordinary income
   - Subject to 10% early withdrawal penalty if under 59.5

4. **Taxable Account Balance**
   - Total balance in regular brokerage/investment accounts
   - Withdrawals taxed at lower capital gains rates
   - No early withdrawal penalties

5. **Success Rate Threshold**
   - 90%: More aggressive withdrawal rate with moderate risk
   - 95%: Balanced approach (recommended)
   - 98%: Conservative approach with highest safety margin

### Step 2: Review Your Results

After calculation, you'll see:

#### Main Metrics
- **Recommended Withdrawal Rate**: The percentage of your total savings you can withdraw annually
- **Annual Gross Withdrawal**: Total amount to withdraw before taxes
- **Net Annual Income**: Your actual after-tax income
- **Success Rate Achieved**: Percentage of historical scenarios where your money lasted

#### Visual Analysis
- **Success Rate Gauge**: Visual representation of your plan's probability of success
- **Tax Breakdown Chart**: Shows how your withdrawal is split between net income and taxes
- **Withdrawal Sources Chart**: Displays the tax-optimized mix of account withdrawals
- **Key Metrics Summary**: Quick reference for important numbers

#### Tax Information
- **Withdrawal Sources**: How much to withdraw from each account type
- **Tax Details**: Breakdown of ordinary income tax, capital gains tax, and any penalties
- **Effective Tax Rate**: Your overall tax rate on withdrawals

#### Early Retirement Warnings
If retiring before age 59.5, you'll see:
- Warning notification
- Years affected by penalties
- Total penalty cost
- Explanation of the penalty impact

## Understanding Your Results

### Success Rate

The success rate represents the percentage of historical scenarios where your portfolio lasted through retirement:

- **95%+ (Excellent)**: Very high probability of success. Your portfolio would have lasted in 95 out of 100 historical scenarios.
- **90-95% (Good)**: Good probability of success with acceptable risk.
- **Below 90% (Moderate)**: Higher risk. Consider reducing withdrawal rate or adjusting retirement plans.

### Withdrawal Rate

The calculator recommends a withdrawal rate that achieves your chosen success threshold. This is based on:
- Historical market returns from 1925-2024
- 60/40 stock/bond portfolio allocation
- Tax-optimized withdrawal strategy
- Inflation adjustments

### Tax Optimization

The calculator automatically optimizes your tax burden by:
1. Withdrawing from tax-deferred accounts up to the 12% federal tax bracket
2. Using taxable accounts for the remainder (taxed at lower capital gains rates)
3. This strategy minimizes your overall tax bill

### Early Withdrawal Penalties

If you retire before age 59.5:
- Tax-deferred account withdrawals incur a 10% IRS penalty
- This penalty applies until you reach 59.5
- The calculator shows the total penalty cost
- Taxable accounts are not subject to this penalty

## Important Assumptions

The calculator makes the following assumptions:

- **Portfolio Allocation**: 60% stocks, 40% bonds
- **Historical Data**: Market returns from 1925-2024
- **Retirement Duration**: Assumes living to age 95
- **Inflation**: Included in historical data analysis
- **Tax Filing Status**: Single filer for tax calculations
- **Standard Deduction**: Current federal standard deduction applied
- **No Other Income**: Calculations assume retirement withdrawals are your only income
- **No State Taxes**: Only federal taxes are calculated

## Disclaimers and Limitations

### Important Warnings

1. **Not Financial Advice**: This tool provides estimates only. It is NOT a substitute for professional financial advice.

2. **Past Performance**: Historical market data does not guarantee future results. Actual market returns may differ significantly.

3. **Tax Complexity**: Real tax situations are often more complex. Consult a tax professional for your specific situation.

4. **Oversimplifications**: The calculator makes assumptions that may not match your exact circumstances:
   - No consideration of Social Security benefits
   - No state or local taxes
   - No Medicare premiums or healthcare costs
   - No Required Minimum Distributions (RMDs)
   - Fixed portfolio allocation
   - No rebalancing costs

5. **Early Retirement**: If retiring before 59.5, there are strategies (like 72(t) distributions) to avoid penalties that this calculator doesn't model.

### When to Seek Professional Help

Consult with qualified professionals if you:
- Have complex tax situations
- Own a business
- Have significant assets outside retirement accounts
- Are considering early retirement
- Have health concerns affecting life expectancy
- Need estate planning
- Want to optimize Social Security claiming strategies

## Tips for Using the Calculator

1. **Try Different Scenarios**: Test various retirement ages and success thresholds to see how they affect your withdrawal rate.

2. **Be Conservative**: It's often wise to use a 95% or 98% success threshold for peace of mind.

3. **Account for Other Income**: If you'll have Social Security, pensions, or part-time income, you can reduce your needed withdrawal.

4. **Consider Healthcare**: Remember that healthcare costs in early retirement (before Medicare) can be substantial.

5. **Review Regularly**: Recalculate annually as your situation changes and market conditions evolve.

6. **Understand Your Risk Tolerance**: A 90% success rate means a 10% chance of running out of money. Only you can decide if that's acceptable.

## Frequently Asked Questions

### Why might I see a penalty warning?

If you plan to retire before age 59.5, withdrawals from tax-deferred accounts (401k, IRA) are subject to a 10% early withdrawal penalty. The calculator shows this so you can make an informed decision.

### Can I adjust the portfolio allocation?

Currently, the calculator assumes a 60/40 stock/bond allocation. This is a common moderate-risk retirement portfolio. Future versions may support custom allocations.

### What if I want to leave money to heirs?

The calculator assumes you'll spend down your portfolio through age 95. If you want to leave an inheritance, you should withdraw less than the recommended rate.

### Why is my withdrawal rate different from the "4% rule"?

The 4% rule is a simplified guideline. Your actual safe withdrawal rate depends on:
- Your chosen success threshold
- Retirement duration (age)
- Tax optimization
- Current market conditions
- Early withdrawal penalties if applicable

### How often should I recalculate?

Recalculate annually or whenever:
- Your account balances change significantly
- You're nearing retirement
- Your retirement age plans change
- You want to update your success threshold

## Technical Details

### Calculation Methodology

The calculator uses a historical simulation approach:

1. **Historical Scenarios**: Analyzes every possible retirement start year from 1925-2024
2. **Sequence Risk**: Accounts for the sequence of returns, not just average returns
3. **Tax Modeling**: Applies current federal tax rates and brackets
4. **Withdrawal Optimization**: Tests multiple withdrawal rates to find the highest rate that meets your success threshold
5. **Portfolio Performance**: Uses actual historical returns for a 60/40 portfolio

### Data Sources

- Historical market data: 1925-2024
- Federal tax rates: Current 2024 rates and brackets
- Early withdrawal penalty: IRS 10% penalty for distributions before 59.5

## Getting Help

If you encounter issues or have questions:

1. Check this user guide
2. Review the disclaimers and assumptions
3. Consult with a financial advisor for personalized guidance

## Version Information

This is the MVP (Minimum Viable Product) version of the Retirement Calculator. Future enhancements may include:
- Social Security integration
- State tax calculations
- Custom portfolio allocations
- Healthcare cost modeling
- RMD calculations
- Roth conversion analysis
