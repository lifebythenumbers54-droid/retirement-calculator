# Retirement Calculator MVP – Brainstorm

## Vision
Create a web-based retirement calculator that helps users understand **how much they can safely withdraw** from their investments over time, based on historical market data and basic tax assumptions.

The MVP should answer one core question:
> "Given my age, assets, and taxes, what is a reasonable withdrawal strategy that historically does not run out of money?"

---

## Guiding Principles
- Start simple, layer complexity
- Prefer explainable math over black-box models
- Historical realism > optimistic projections
- MVP should be useful even without login or saved data

---

## Target User
- DIY investors
- Early retirees / FIRE-minded users
- Technically curious users who want transparency

---

## Core Assumptions (Initial)
- User is anonymous
- Single household
- US-based tax assumptions
- Fixed stock/bond allocation (configurable later)
- Inflation-adjusted withdrawals

---

## Tech Stack

### Frontend
- React
- Simple form-based UI
- Results summary + basic charts

### Backend
- C# .NET Core Web API
- Deterministic calculation engine
- Static historical datasets (last 100 years of S&P 500 + bond returns)

---

## Key Inputs (MVP)
- Current age
- Retirement age
- Retirement account balance (401k / IRA)
- Taxable account balance
- Success rate threshold (user-selectable: 90%, 95%, 98%, etc.)

Optional later:
- Asset allocation
- Spending flexibility
- Inflation assumption

---

## Key Outputs (MVP)
- Suggested withdrawal rate (%)
- Annual gross withdrawal
- Estimated annual taxes
- Net annual income
- Historical success rate

---

## Milestones

### Milestone 1 – Basic UI & Data Flow
**Goal:** End-to-end data submission

- React form for user inputs
- API endpoint accepts data
- Echo response back to UI

Success = data flows cleanly from UI → API → UI

---

### Milestone 2 – Historical Withdrawal Modeling
**Goal:** Determine safe withdrawal rates

- Load historical S&P 500 + bond returns (100 years of data)
- Apply fixed portfolio allocation
- Run rolling retirement-period simulations
- Identify withdrawal rate based on user's selected success threshold

Outputs:
- Withdrawal % (based on user's success rate preference)
- Annual income
- Success rate achieved

---

### Milestone 3 – Tax Modeling (Post-Retirement)
**Goal:** Make results more realistic

- Separate taxable vs tax-deferred accounts
- Apply simplified federal tax brackets
- Model tax-optimized withdrawal order (minimize taxes today + future tax burden)
- Convert gross → net income

Withdrawal order logic:
- Optimize between taxable and tax-deferred accounts based on marginal tax rates
- Consider long-term capital gains vs ordinary income taxation
- Minimize total lifetime tax burden

Outputs updated to include taxes

---

### Milestone 4 – Early Retirement & Penalties
**Goal:** Support under-59½ scenarios

- Detect early withdrawals
- Apply 10% penalty where applicable
- Adjust withdrawal order
- Display warnings and penalties

---

## Withdrawal Logic Ideas
- Fixed real-dollar withdrawals
- Inflation-adjusted annually
- Portfolio rebalanced yearly
- Failure = portfolio hits $0 before end of retirement period

---

## Tax Logic Ideas (Simplified)
- Ordinary income tax for retirement accounts
- Long-term capital gains tax for taxable accounts
- Bracketed federal tax model
- Tax-optimized withdrawal sequencing:
  - Compare marginal tax impact of each withdrawal source
  - Balance current tax vs future tax obligations
  - Account for the tax rate differential (ordinary income vs LTCG)

---

## Edge Cases to Watch
- All assets in taxable accounts
- All assets in tax-deferred accounts
- Very early retirement ages
- Very low balances

---

## Risks / Unknowns
- Overfitting to historical data
- Users misinterpreting outputs as guarantees
- Tax complexity creep

---

## Success Criteria for MVP
- Clear, understandable output
- Results are reproducible
- User can experiment with inputs easily
- Calculator provides insight, not false certainty

---

## Future Enhancements (Post-MVP)
- Monte Carlo simulations
- Social Security modeling
- RMDs
- State taxes
- Scenario saving
- Auth + profiles

---

## Key Design Decisions

### ✅ Resolved
1. **Success rate threshold:** User-configurable (90%, 95%, 98%, etc.)
2. **Historical dataset:** Last 100 years of market data (user-provided)
3. **Withdrawal order:** Tax-optimized to minimize total lifetime tax burden

### Open Questions
- How configurable should allocation be in MVP?
- Should spending be fixed or flexible?
- What default portfolio allocation should we suggest? (60/40? 70/30?)

---

*This document is intentionally exploratory and may change as implementation begins.*

