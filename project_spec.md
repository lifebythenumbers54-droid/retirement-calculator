# Retirement Calculator MVP – Project Specification

## 1. Project Overview

### 1.1 Purpose
A web-based retirement calculator that helps users determine safe withdrawal strategies from their investment portfolios based on historical market data and tax considerations.

### 1.2 Core Question
> "Given my age, assets, and taxes, what is a reasonable withdrawal strategy that historically does not run out of money?"

### 1.3 Target Users
- DIY investors
- Early retirees / FIRE-minded individuals
- Technically curious users who want transparency in calculations

---

## 2. Technical Architecture

### 2.1 Technology Stack

**Frontend:**
- React
- Form-based UI for input collection
- Results display with summary and basic charts

**Backend:**
- C# .NET Core Web API
- Deterministic calculation engine
- RESTful API endpoints

**Data:**
- Static historical market data (100 years)
- S&P 500 returns
- Bond returns
- Pre-loaded inflation data

### 2.2 System Design Principles
- Start simple, layer complexity over time
- Prefer explainable math over black-box models
- Historical realism over optimistic projections
- Stateless MVP (no login or data persistence required)

---

## 3. Functional Requirements

### 3.1 User Inputs (MVP)

**Required:**
- Current age (integer, 18-100)
- Retirement age (integer, must be >= current age)
- Retirement account balance (401k/IRA, USD)
- Taxable account balance (USD)
- Success rate threshold (dropdown: 90%, 95%, 98%)

**Future (Post-MVP):**
- Asset allocation (stock/bond split)
- Spending flexibility preferences
- Custom inflation assumptions

### 3.2 Outputs (MVP)

**Primary Results:**
- Recommended withdrawal rate (%)
- Annual gross withdrawal amount (USD)
- Estimated annual taxes (USD)
- Net annual income (USD)
- Historical success rate achieved (%)

**Supporting Information:**
- Number of historical scenarios tested
- Failure rate and what it means
- Basic visualization of withdrawal sustainability

### 3.3 Calculation Engine Requirements

#### 3.3.1 Historical Simulation
- Use rolling window analysis across 100 years of data
- Simulate retirement periods starting from each historical year
- Apply fixed portfolio allocation (default: configurable)
- Rebalance portfolio annually
- Adjust withdrawals for inflation yearly

#### 3.3.2 Withdrawal Logic
- Fixed real-dollar withdrawals (inflation-adjusted)
- Annual rebalancing
- Failure condition: portfolio value reaches $0 before end of retirement period
- Success threshold: user-defined (90%, 95%, or 98% of scenarios succeed)

#### 3.3.3 Tax Modeling
**Account Types:**
- Tax-deferred accounts (401k/IRA): taxed as ordinary income
- Taxable accounts: taxed as long-term capital gains

**Withdrawal Order Optimization:**
- Minimize total lifetime tax burden
- Consider marginal tax rates for each withdrawal source
- Balance current-year tax vs future tax obligations
- Account for tax rate differential (ordinary income vs LTCG)

**Tax Calculation:**
- Apply federal tax brackets (current year rates)
- Standard deduction application
- Separate ordinary income and capital gains taxation

#### 3.3.4 Early Retirement Handling (Milestone 4)
- Detect withdrawals before age 59½
- Apply 10% early withdrawal penalty to applicable withdrawals
- Adjust withdrawal order to minimize penalties
- Display penalty amounts and warnings to user

---

## 4. Data Requirements

### 4.1 Historical Market Data
- **Time Period:** Last 100 years
- **Data Points:**
  - Annual S&P 500 returns
  - Annual bond returns (government/corporate blend)
  - Annual inflation rates (CPI)
- **Source:** User-provided dataset
- **Format:** CSV or JSON (to be determined)

### 4.2 Tax Data
- Current federal income tax brackets
- Long-term capital gains tax rates
- Standard deduction amounts
- Early withdrawal penalty rules (10% before 59½)

---

## 5. Implementation Milestones

### Milestone 1: Basic UI & Data Flow
**Objective:** Establish end-to-end communication between UI and API

**Deliverables:**
- React form with all MVP input fields
- API endpoint to accept user inputs
- Response echoed back to UI
- Basic validation on inputs

**Success Criteria:**
- Data flows cleanly: UI → API → UI
- No data loss or corruption
- Input validation working

---

### Milestone 2: Historical Withdrawal Modeling
**Objective:** Calculate safe withdrawal rates using historical simulations

**Deliverables:**
- Historical data loader (100 years of returns)
- Portfolio allocation logic
- Rolling retirement period simulator
- Withdrawal rate calculator based on success threshold
- Success rate computation

**Outputs:**
- Withdrawal % (based on user's success preference)
- Annual income amount
- Success rate achieved

**Success Criteria:**
- Calculations are deterministic and reproducible
- Results align with known safe withdrawal rate studies (e.g., 4% rule baseline)

---

### Milestone 3: Tax Modeling (Post-Retirement)
**Objective:** Add realistic tax considerations

**Deliverables:**
- Separate taxable vs tax-deferred account handling
- Federal tax bracket implementation
- Tax-optimized withdrawal order logic
- Gross-to-net income conversion
- Tax breakdown display

**Withdrawal Order Logic:**
- Optimize between account types based on marginal rates
- Consider ordinary income vs LTCG taxation
- Minimize total lifetime tax burden

**Outputs Updated:**
- Annual taxes paid
- Net annual income (after taxes)
- Tax efficiency score (optional)

**Success Criteria:**
- Tax calculations match expected values for test scenarios
- Withdrawal order demonstrably reduces tax burden

---

### Milestone 4: Early Retirement & Penalties
**Objective:** Support users retiring before age 59½

**Deliverables:**
- Early withdrawal detection
- 10% penalty application
- Withdrawal order adjustment to minimize penalties
- Warning messages for penalty scenarios
- Penalty amount display

**Success Criteria:**
- Penalties correctly applied to affected withdrawals
- User is clearly informed of penalty implications

---

## 6. User Interface Requirements

### 6.1 Input Form
- Clean, single-page form layout
- Clear labels and input validation
- Tooltips/help text for technical terms
- Real-time validation feedback
- Submit button to trigger calculation

### 6.2 Results Display
- Summary card with key metrics
- Tax breakdown section
- Success rate visualization
- Disclaimer about historical data limitations
- Clear explanation of what results mean

### 6.3 Visualization
- Simple chart showing withdrawal sustainability
- Success rate indicator (progress bar or gauge)
- Optional: portfolio balance over time

---

## 7. Non-Functional Requirements

### 7.1 Performance
- Calculation results returned within 2 seconds
- Support for 100+ years of historical data
- Responsive UI on desktop browsers

### 7.2 Accuracy
- Calculations reproducible with same inputs
- Results within accepted ranges of academic research
- Clear mathematical documentation

### 7.3 Usability
- Intuitive input form (no financial expertise required)
- Results explained in plain language
- Appropriate disclaimers about limitations

### 7.4 Security
- Input validation to prevent injection attacks
- No sensitive data storage (MVP is stateless)
- HTTPS for production deployment

---

## 8. Assumptions & Constraints

### 8.1 Assumptions
- User is anonymous (no authentication)
- Single household calculations
- US-based tax rules only
- User provides all account balances in today's dollars
- No Social Security income (post-MVP feature)
- No Required Minimum Distributions (post-MVP)
- Fixed asset allocation throughout retirement

### 8.2 Constraints
- MVP does not save user data
- No state tax calculations
- No Monte Carlo simulation (deterministic only)
- Historical data only (no forward projections)

---

## 9. Edge Cases

### 9.1 Account Distribution Edge Cases
- All assets in taxable accounts only
- All assets in tax-deferred accounts only
- Very small balances (< $10,000)
- Very large balances (> $10M)

### 9.2 Age Edge Cases
- Very early retirement (age 30-40)
- Late retirement (age 70+)
- Long retirement periods (40+ years)

### 9.3 Calculation Edge Cases
- Success rate threshold cannot be met (portfolio too small)
- Zero inflation scenarios
- Extreme market conditions in historical data

---

## 10. Risks & Mitigation

### 10.1 Data Risks
**Risk:** Overfitting to historical data
**Mitigation:** Clear disclaimers; show historical success rate vs guarantee

**Risk:** Data quality issues
**Mitigation:** Validate data source; cross-check against known benchmarks

### 10.2 User Interpretation Risks
**Risk:** Users misinterpret results as guarantees
**Mitigation:** Prominent disclaimers; clear language about uncertainty

**Risk:** Tax complexity creep
**Mitigation:** Limit to federal taxes in MVP; document simplifications

### 10.3 Technical Risks
**Risk:** Performance degradation with large datasets
**Mitigation:** Optimize data structures; consider caching

---

## 11. Success Criteria

### 11.1 MVP Launch Criteria
- All four milestones completed
- Results are clear and understandable
- Calculations are reproducible
- Users can easily experiment with inputs
- Appropriate disclaimers in place
- Basic documentation available

### 11.2 Quality Metrics
- Calculation accuracy: ±0.5% vs manual verification
- Response time: < 2 seconds for all calculations
- User comprehension: Results are self-explanatory

---

## 12. Future Enhancements (Post-MVP)

### 12.1 Features
- Monte Carlo simulation mode
- Social Security income modeling
- Required Minimum Distributions (RMDs)
- State tax calculations
- Roth IRA account handling
- Spending flexibility strategies
- Healthcare cost modeling

### 12.2 User Features
- Save/load scenarios
- User authentication
- Historical scenario comparison
- PDF report generation
- Multiple portfolio allocation options

### 12.3 Technical Enhancements
- Database for user data persistence
- Advanced charting and visualization
- Mobile-responsive design
- API rate limiting
- Comprehensive test suite

---

## 13. Out of Scope (MVP)

- User authentication and profiles
- Data persistence
- Social Security calculations
- Required Minimum Distributions
- State taxes
- Healthcare costs
- Estate planning
- Multiple household members
- Scenario comparison tools
- Mobile app
- Email notifications

---

## 14. Appendix

### 14.1 Key Design Decisions

**Decision 1: User-Configurable Success Threshold**
- Rationale: Different users have different risk tolerances
- Implementation: Dropdown with preset options (90%, 95%, 98%)

**Decision 2: 100-Year Historical Dataset**
- Rationale: Captures multiple market cycles and economic conditions
- Source: User-provided data

**Decision 3: Tax-Optimized Withdrawal Order**
- Rationale: Maximize user's net income by minimizing lifetime taxes
- Implementation: Dynamic withdrawal sequencing based on marginal rates

### 14.2 Open Design Questions
1. What default portfolio allocation should be used? (60/40? 70/30?)
2. Should the MVP allow user-configurable allocation or use a fixed default?
3. Should spending be strictly fixed or allow for some flexibility?
4. What level of tax detail should be shown to users?

---

**Document Version:** 1.0
**Last Updated:** 2026-01-01
**Status:** Draft for Implementation
