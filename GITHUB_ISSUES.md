# GitHub Issues for Retirement Calculator MVP

This document contains all the GitHub issues ready to be created. Copy each section and create as a new issue on GitHub.

---

## Milestone 1: Basic UI & Data Flow

### Issue #1: Create Frontend Input Form Component

**Title:** `[MILESTONE-1] Create Frontend Input Form Component`

**Labels:** `enhancement`, `frontend`, `milestone-1`

**Description:**

Create a React component for the retirement calculator input form with all MVP fields and client-side validation.

**User Story:**
As a user, I want to enter my retirement information through an intuitive form so that I can calculate my retirement withdrawal strategy.

**Acceptance Criteria:**
- [ ] Create `InputForm.jsx` component in `frontend/src/components/`
- [ ] Implement all MVP input fields:
  - [ ] Current age (18-100)
  - [ ] Retirement age (>= current age)
  - [ ] Retirement account balance (401k/IRA)
  - [ ] Taxable account balance
  - [ ] Success rate threshold (dropdown: 90%, 95%, 98%)
- [ ] Add client-side validation:
  - [ ] Age must be 18-100
  - [ ] Retirement age must be >= current age
  - [ ] Balance fields must be positive numbers
  - [ ] All fields are required
- [ ] Display validation errors in real-time
- [ ] Add tooltips/help text for technical terms
- [ ] Create form submission handler
- [ ] Add loading state during API call
- [ ] Style the form with consistent UI design

**Technical Details:**
- Use React hooks (useState, useEffect)
- Use controlled components for all inputs
- Integrate with `apiClient.js` for form submission
- Handle errors gracefully with error messages
- Format currency inputs with proper separators

**Files to Create/Modify:**
- `frontend/src/components/InputForm.jsx` (new)
- `frontend/src/components/InputForm.css` (new, optional)
- `frontend/src/App.jsx` (modify to include InputForm)

**Dependencies:**
- None (can be developed independently)

**Testing:**
- Test all validation rules
- Test form submission with valid data
- Test error handling with invalid data
- Test loading states

---

### Issue #2: Create Backend API Models

**Title:** `[MILESTONE-1] Create Backend API Models for User Input and Results`

**Labels:** `enhancement`, `backend`, `milestone-1`

**Description:**

Create C# models for user input and calculation results to establish the API contract between frontend and backend.

**User Story:**
As a backend developer, I want well-defined data models so that the API can validate inputs and structure responses consistently.

**Acceptance Criteria:**
- [ ] Create `UserInput.cs` model in `backend/RetirementCalculator.API/Models/`
  - [ ] CurrentAge (int, range 18-100)
  - [ ] RetirementAge (int, >= CurrentAge)
  - [ ] RetirementAccountBalance (decimal, >= 0)
  - [ ] TaxableAccountBalance (decimal, >= 0)
  - [ ] SuccessRateThreshold (decimal, 0.90/0.95/0.98)
- [ ] Create `CalculationResult.cs` model in `backend/RetirementCalculator.API/Models/`
  - [ ] WithdrawalRate (decimal, percentage)
  - [ ] AnnualGrossWithdrawal (decimal, USD)
  - [ ] EstimatedAnnualTaxes (decimal, USD)
  - [ ] NetAnnualIncome (decimal, USD)
  - [ ] AchievedSuccessRate (decimal, percentage)
  - [ ] NumberOfScenariosSimulated (int)
- [ ] Add data annotations for validation
- [ ] Add XML documentation comments
- [ ] Create unit tests for model validation

**Technical Details:**
- Use C# data annotations (`[Range]`, `[Required]`, etc.)
- Implement custom validation attributes if needed
- Follow .NET naming conventions
- Ensure models are serializable to JSON

**Files to Create:**
- `backend/RetirementCalculator.API/Models/UserInput.cs`
- `backend/RetirementCalculator.API/Models/CalculationResult.cs`

**Dependencies:**
- None (can be developed independently)

**Testing:**
- Unit tests for validation rules
- Test JSON serialization/deserialization

---

### Issue #3: Create Calculation API Endpoint

**Title:** `[MILESTONE-1] Create Calculation API Endpoint (Echo Implementation)`

**Labels:** `enhancement`, `backend`, `milestone-1`

**Description:**

Create the `/api/calculate` endpoint that accepts user input and returns a result. For Milestone 1, this will echo back the input with mock calculation results to verify end-to-end data flow.

**User Story:**
As a frontend developer, I want a working API endpoint so that I can test the complete data flow from UI to API and back.

**Acceptance Criteria:**
- [ ] Create `CalculationController.cs` in `backend/RetirementCalculator.API/Controllers/`
- [ ] Implement POST `/api/calculate` endpoint
- [ ] Accept `UserInput` model as request body
- [ ] Validate input using model validation
- [ ] Return `CalculationResult` with mock data (for now):
  - [ ] Echo back age and balance information
  - [ ] Return placeholder withdrawal rate (4%)
  - [ ] Return placeholder success rate (95%)
  - [ ] Calculate basic annual withdrawal (balance * 0.04)
- [ ] Return appropriate HTTP status codes:
  - [ ] 200 OK for valid requests
  - [ ] 400 Bad Request for validation errors
  - [ ] 500 Internal Server Error for exceptions
- [ ] Add Swagger documentation for the endpoint
- [ ] Add logging for requests and responses
- [ ] Implement error handling

**Technical Details:**
- Inherit from `ControllerBase`
- Use `[ApiController]` and `[Route("api/[controller]")]` attributes
- Use dependency injection (prepare for future services)
- Follow REST API best practices
- Return consistent error response format

**Files to Create:**
- `backend/RetirementCalculator.API/Controllers/CalculationController.cs`

**Dependencies:**
- Issue #2 (Backend API Models) must be completed first

**Testing:**
- Test with valid input data
- Test with invalid input data (validation errors)
- Test error handling
- Test via Swagger UI
- Test via curl or Postman

---

### Issue #4: Integrate Frontend Form with Backend API

**Title:** `[MILESTONE-1] Integrate Frontend Form with Backend API`

**Labels:** `enhancement`, `frontend`, `backend`, `milestone-1`, `integration`

**Description:**

Connect the frontend input form to the backend API endpoint and display the calculation results.

**User Story:**
As a user, I want to submit my retirement information and see calculation results so that I can understand my withdrawal strategy.

**Acceptance Criteria:**
- [ ] Update `apiClient.js` to call `/api/calculate` endpoint
- [ ] Handle form submission in `InputForm.jsx`
- [ ] Show loading indicator during API call
- [ ] Handle API responses:
  - [ ] Success: Display results
  - [ ] Validation errors: Show field-specific error messages
  - [ ] Server errors: Show generic error message
- [ ] Create basic `ResultsDisplay.jsx` component to show:
  - [ ] Recommended withdrawal rate
  - [ ] Annual gross withdrawal amount
  - [ ] Estimated annual taxes
  - [ ] Net annual income
  - [ ] Success rate achieved
- [ ] Add error boundary for unexpected errors
- [ ] Test complete end-to-end flow

**Technical Details:**
- Use async/await for API calls
- Handle network errors gracefully
- Update App.jsx state to manage form vs results view
- Add "Calculate Again" button to reset form
- Format currency values with proper separators

**Files to Modify:**
- `frontend/src/services/apiClient.js`
- `frontend/src/components/InputForm.jsx`
- `frontend/src/App.jsx`

**Files to Create:**
- `frontend/src/components/ResultsDisplay.jsx`

**Dependencies:**
- Issue #1 (Frontend Input Form) must be completed
- Issue #3 (Calculation API Endpoint) must be completed

**Testing:**
- Test successful calculation flow
- Test validation error handling
- Test network error handling
- Test loading states
- Test "Calculate Again" functionality

---

### Issue #5: End-to-End Testing for Milestone 1

**Title:** `[MILESTONE-1] End-to-End Testing and Validation`

**Labels:** `testing`, `milestone-1`

**Description:**

Comprehensive testing of the complete Milestone 1 implementation to ensure data flows correctly from UI to API and back without data loss or corruption.

**Acceptance Criteria:**
- [ ] Test complete user flow:
  - [ ] Enter valid data
  - [ ] Submit form
  - [ ] Verify API receives correct data
  - [ ] Verify response contains expected fields
  - [ ] Verify UI displays results correctly
- [ ] Test edge cases:
  - [ ] Minimum age (18)
  - [ ] Maximum age (100)
  - [ ] Retirement age = current age
  - [ ] Zero account balances
  - [ ] Very large account balances
  - [ ] All success rate threshold options
- [ ] Test validation:
  - [ ] Invalid age ranges
  - [ ] Retirement age < current age
  - [ ] Negative balances
  - [ ] Missing required fields
- [ ] Test error scenarios:
  - [ ] Backend not running
  - [ ] Network timeout
  - [ ] Server error response
- [ ] Performance test:
  - [ ] Response time < 2 seconds
- [ ] Cross-browser testing (Chrome, Firefox, Edge)
- [ ] Document any issues found

**Technical Details:**
- Create manual test checklist
- Consider adding automated E2E tests (Playwright/Cypress) in future
- Document test results

**Dependencies:**
- Issue #4 (Frontend-Backend Integration) must be completed

**Testing:**
- Manual testing with test scenarios
- Document results in issue comments

---

## Milestone 2: Historical Withdrawal Modeling

### Issue #6: Create Historical Data Service

**Title:** `[MILESTONE-2] Create Historical Data Service`

**Labels:** `enhancement`, `backend`, `milestone-2`

**Description:**

Create a service to load, parse, and validate historical market data from the JSON file.

**User Story:**
As the calculation engine, I need access to historical market data so that I can perform accurate retirement simulations based on past market performance.

**Acceptance Criteria:**
- [ ] Create `IHistoricalDataService.cs` interface in `backend/RetirementCalculator.API/Services/`
- [ ] Create `HistoricalDataService.cs` implementation
- [ ] Load data from `data/historical_market_data.json`
- [ ] Parse JSON into strongly-typed models
- [ ] Create `MarketDataPoint.cs` model:
  - [ ] Year (int)
  - [ ] SP500Return (decimal)
  - [ ] BondReturn (decimal)
  - [ ] Inflation (decimal)
- [ ] Validate data:
  - [ ] All years present (1925-2024)
  - [ ] No missing or null values
  - [ ] Returns are within reasonable ranges
- [ ] Cache data in memory (singleton service)
- [ ] Add error handling for file I/O
- [ ] Register service in Program.cs

**Technical Details:**
- Use `System.Text.Json` for JSON parsing
- Implement as singleton (load once at startup)
- Add logging for data loading
- Handle file not found gracefully

**Files to Create:**
- `backend/RetirementCalculator.API/Models/MarketDataPoint.cs`
- `backend/RetirementCalculator.API/Services/IHistoricalDataService.cs`
- `backend/RetirementCalculator.API/Services/HistoricalDataService.cs`

**Files to Modify:**
- `backend/RetirementCalculator.API/Program.cs` (register service)

**Dependencies:**
- Historical market data file already exists in `data/historical_market_data.json`

**Testing:**
- Unit tests for data loading
- Unit tests for validation
- Test with valid data file
- Test with missing file
- Test with corrupted data

---

### Issue #7: Create Withdrawal Calculation Service

**Title:** `[MILESTONE-2] Create Withdrawal Calculation Service with Historical Simulation`

**Labels:** `enhancement`, `backend`, `milestone-2`

**Description:**

Implement the core retirement calculation logic using historical simulation to determine safe withdrawal rates.

**User Story:**
As a user, I want my withdrawal strategy calculated based on historical market data so that I can have confidence in the sustainability of my retirement plan.

**Acceptance Criteria:**
- [ ] Create `IWithdrawalCalculationService.cs` interface
- [ ] Create `WithdrawalCalculationService.cs` implementation
- [ ] Implement portfolio allocation logic:
  - [ ] Default: 60% stocks, 40% bonds
  - [ ] Apply user's total balance across allocation
- [ ] Implement rolling window analysis:
  - [ ] For each historical start year (1925-2024)
  - [ ] Simulate retirement period based on user's age
  - [ ] Calculate retirement length in years
- [ ] Implement annual simulation logic:
  - [ ] Apply portfolio returns (stocks + bonds)
  - [ ] Annual rebalancing to maintain 60/40 allocation
  - [ ] Withdraw fixed real-dollar amount (inflation-adjusted)
  - [ ] Check for portfolio depletion (balance reaches $0)
- [ ] Implement withdrawal rate optimizer:
  - [ ] Binary search or iterative approach
  - [ ] Find maximum withdrawal rate that meets success threshold
  - [ ] Success threshold: user-defined (90%, 95%, or 98%)
- [ ] Calculate success rate:
  - [ ] Count successful scenarios vs total scenarios
  - [ ] Return achieved success rate
- [ ] Inject `IHistoricalDataService` dependency
- [ ] Add comprehensive logging
- [ ] Handle edge cases (e.g., very short retirement periods)

**Technical Details:**
- Use dependency injection for HistoricalDataService
- Optimize for performance (consider parallel processing)
- Ensure calculations are deterministic
- Use decimal for financial calculations
- Add XML documentation comments

**Files to Create:**
- `backend/RetirementCalculator.API/Services/IWithdrawalCalculationService.cs`
- `backend/RetirementCalculator.API/Services/WithdrawalCalculationService.cs`

**Dependencies:**
- Issue #6 (Historical Data Service) must be completed

**Testing:**
- Unit tests for portfolio allocation
- Unit tests for simulation logic
- Test with known scenarios (validate against 4% rule)
- Test all success thresholds (90%, 95%, 98%)
- Test edge cases (young retirement, old retirement)
- Performance testing (should complete in < 2 seconds)

---

### Issue #8: Integrate Calculation Service with API Endpoint

**Title:** `[MILESTONE-2] Integrate Withdrawal Calculation Service with API`

**Labels:** `enhancement`, `backend`, `milestone-2`, `integration`

**Description:**

Replace the mock calculation in the API endpoint with the actual withdrawal calculation service.

**User Story:**
As a user, I want to receive real withdrawal calculations based on historical data so that I can make informed retirement decisions.

**Acceptance Criteria:**
- [ ] Inject `IWithdrawalCalculationService` into `CalculationController`
- [ ] Replace mock calculation with actual service call
- [ ] Update `CalculationResult` model if needed:
  - [ ] Add NumberOfScenariosSimulated
  - [ ] Add any additional metrics
- [ ] Handle calculation errors gracefully
- [ ] Add logging for calculation requests
- [ ] Update API response time monitoring
- [ ] Update Swagger documentation

**Technical Details:**
- Use constructor injection
- Add try-catch for calculation errors
- Return detailed error messages for debugging
- Ensure thread-safety if needed

**Files to Modify:**
- `backend/RetirementCalculator.API/Controllers/CalculationController.cs`
- `backend/RetirementCalculator.API/Program.cs` (register calculation service)
- `backend/RetirementCalculator.API/Models/CalculationResult.cs` (if needed)

**Dependencies:**
- Issue #7 (Withdrawal Calculation Service) must be completed

**Testing:**
- Test with various input scenarios
- Verify results are deterministic
- Verify results align with expected ranges
- Test performance (< 2 seconds)
- Compare results with known safe withdrawal rate studies

---

### Issue #9: Update Frontend to Display Real Calculation Results

**Title:** `[MILESTONE-2] Update Frontend Results Display for Real Calculations`

**Labels:** `enhancement`, `frontend`, `milestone-2`

**Description:**

Update the results display component to show the real calculation results from the historical simulation.

**User Story:**
As a user, I want to see my personalized withdrawal strategy based on historical simulations so that I understand my sustainable retirement income.

**Acceptance Criteria:**
- [ ] Update `ResultsDisplay.jsx` to show:
  - [ ] Recommended withdrawal rate (formatted as percentage)
  - [ ] Annual gross withdrawal amount (formatted as currency)
  - [ ] Number of historical scenarios tested
  - [ ] Achieved success rate (formatted as percentage)
  - [ ] Clear explanation of what the results mean
- [ ] Add disclaimer about historical data limitations
- [ ] Format all numbers appropriately:
  - [ ] Currency with $ and thousand separators
  - [ ] Percentages with % symbol
- [ ] Add visual indicators:
  - [ ] Success rate progress bar or gauge
  - [ ] Color coding (green for high success, yellow for medium, red for low)
- [ ] Add "What does this mean?" section with explanations
- [ ] Make results easy to read and understand

**Technical Details:**
- Use number formatting libraries (e.g., Intl.NumberFormat)
- Create reusable formatting utility functions
- Ensure accessibility (ARIA labels, semantic HTML)

**Files to Modify:**
- `frontend/src/components/ResultsDisplay.jsx`
- `frontend/src/components/ResultsDisplay.css` (if needed)

**Files to Create:**
- `frontend/src/utils/formatters.js` (optional, for number formatting)

**Dependencies:**
- Issue #8 (Integrate Calculation Service) must be completed

**Testing:**
- Test with various calculation results
- Test number formatting edge cases
- Test responsiveness on different screen sizes
- Verify accessibility

---

### Issue #10: Milestone 2 Integration Testing

**Title:** `[MILESTONE-2] Integration Testing and Validation`

**Labels:** `testing`, `milestone-2`

**Description:**

Comprehensive testing of Milestone 2 to ensure calculations are accurate, deterministic, and performant.

**Acceptance Criteria:**
- [ ] Test calculation accuracy:
  - [ ] Compare results with known safe withdrawal rate studies
  - [ ] Verify 4% rule baseline (should appear in certain scenarios)
  - [ ] Test all success thresholds (90%, 95%, 98%)
- [ ] Test determinism:
  - [ ] Same inputs always produce same outputs
  - [ ] No random variation in calculations
- [ ] Test performance:
  - [ ] API response time < 2 seconds
  - [ ] Frontend renders results quickly
- [ ] Test edge cases:
  - [ ] Very young retirement age (30)
  - [ ] Very old retirement age (70)
  - [ ] Very long retirement periods (50+ years)
  - [ ] Small portfolio balances
  - [ ] Large portfolio balances
  - [ ] Different portfolio allocations (60/40)
- [ ] Test data quality:
  - [ ] All 100 years of historical data being used
  - [ ] Returns are applied correctly
  - [ ] Inflation adjustments are correct
- [ ] Document test results
- [ ] Create test report

**Dependencies:**
- Issue #9 (Frontend Updates) must be completed

**Testing:**
- Manual testing with comprehensive test scenarios
- Document expected vs actual results
- Validate against external sources (Trinity Study, Bengen research)

---

## Milestone 3: Tax Modeling

### Issue #11: Create Tax Calculation Service

**Title:** `[MILESTONE-3] Create Tax Calculation Service`

**Labels:** `enhancement`, `backend`, `milestone-3`

**Description:**

Implement federal tax calculation logic including tax brackets, standard deduction, and separate handling for ordinary income vs long-term capital gains.

**User Story:**
As a user, I want accurate tax calculations so that I understand my real after-tax retirement income.

**Acceptance Criteria:**
- [ ] Create `ITaxCalculationService.cs` interface
- [ ] Create `TaxCalculationService.cs` implementation
- [ ] Implement 2025 federal tax brackets for ordinary income
- [ ] Implement 2025 long-term capital gains tax rates
- [ ] Implement standard deduction (2025 amount)
- [ ] Create method to calculate ordinary income tax
- [ ] Create method to calculate LTCG tax
- [ ] Create method to apply standard deduction
- [ ] Handle married vs single filing status (start with single)
- [ ] Add comprehensive tax calculation logic
- [ ] Add unit tests for all tax scenarios

**Technical Details:**
- Use tax bracket tables as constants or configuration
- Handle progressive tax brackets correctly
- Consider 0% LTCG bracket for low income
- Use decimal for all calculations
- Add XML documentation

**Files to Create:**
- `backend/RetirementCalculator.API/Services/ITaxCalculationService.cs`
- `backend/RetirementCalculator.API/Services/TaxCalculationService.cs`
- `backend/RetirementCalculator.API/Models/TaxBracket.cs` (if needed)

**Dependencies:**
- None (can be developed independently)

**Testing:**
- Unit tests for each tax bracket
- Unit tests for LTCG calculations
- Unit tests for standard deduction
- Test with known tax scenarios
- Validate against IRS tax tables

---

### Issue #12: Implement Tax-Optimized Withdrawal Order Logic

**Title:** `[MILESTONE-3] Implement Tax-Optimized Withdrawal Order Strategy`

**Labels:** `enhancement`, `backend`, `milestone-3`

**Description:**

Implement logic to determine the optimal withdrawal order from taxable and tax-deferred accounts to minimize lifetime tax burden.

**User Story:**
As a user, I want my withdrawals optimized for taxes so that I keep more of my retirement savings.

**Acceptance Criteria:**
- [ ] Create withdrawal order optimization logic
- [ ] Separate handling for:
  - [ ] Taxable account withdrawals (LTCG tax)
  - [ ] Tax-deferred account withdrawals (ordinary income tax)
- [ ] Implement optimization strategy:
  - [ ] Consider marginal tax rates for each account type
  - [ ] Balance current-year tax vs future tax obligations
  - [ ] Account for tax rate differential (ordinary income vs LTCG)
  - [ ] Minimize total lifetime tax burden
- [ ] Track account balances separately during simulation
- [ ] Apply appropriate tax rate to each withdrawal source
- [ ] Update simulation to use tax-optimized withdrawals
- [ ] Add comprehensive documentation of strategy

**Technical Details:**
- Consider filling lower tax brackets with tax-deferred withdrawals
- Consider using taxable accounts for higher amounts
- Handle case where one account depletes before the other
- Ensure strategy is explainable to users

**Files to Modify:**
- `backend/RetirementCalculator.API/Services/WithdrawalCalculationService.cs`

**Dependencies:**
- Issue #11 (Tax Calculation Service) must be completed
- Issue #7 (Withdrawal Calculation Service) must exist

**Testing:**
- Unit tests for withdrawal order logic
- Test scenarios with different account balance ratios
- Verify tax minimization vs naive withdrawal strategy
- Test edge cases (one account empty, etc.)

---

### Issue #13: Update Calculation Models and API for Tax Information

**Title:** `[MILESTONE-3] Update API Models to Include Tax Information`

**Labels:** `enhancement`, `backend`, `milestone-3`

**Description:**

Update the API models and endpoint to include tax calculation results in the response.

**User Story:**
As a user, I want to see the tax breakdown of my retirement withdrawals so that I understand my tax obligations.

**Acceptance Criteria:**
- [ ] Update `UserInput.cs` model (if needed):
  - [ ] Add filing status (optional, default to single)
- [ ] Update `CalculationResult.cs` model:
  - [ ] EstimatedAnnualTaxes (decimal)
  - [ ] NetAnnualIncome (decimal)
  - [ ] TaxableAccountWithdrawal (decimal)
  - [ ] TaxDeferredAccountWithdrawal (decimal)
  - [ ] OrdinaryIncomeTax (decimal)
  - [ ] CapitalGainsTax (decimal)
  - [ ] EffectiveTaxRate (decimal, percentage)
- [ ] Update `CalculationController` to include tax calculations
- [ ] Inject `ITaxCalculationService` into controller
- [ ] Update Swagger documentation
- [ ] Update API response format

**Technical Details:**
- Ensure backward compatibility if possible
- Add versioning if breaking changes are needed
- Format tax amounts as currency in response

**Files to Modify:**
- `backend/RetirementCalculator.API/Models/UserInput.cs`
- `backend/RetirementCalculator.API/Models/CalculationResult.cs`
- `backend/RetirementCalculator.API/Controllers/CalculationController.cs`
- `backend/RetirementCalculator.API/Program.cs` (register tax service)

**Dependencies:**
- Issue #11 (Tax Calculation Service) must be completed
- Issue #12 (Withdrawal Order Logic) must be completed

**Testing:**
- Test API with tax calculations
- Verify all tax fields are populated correctly
- Test with different account balance scenarios

---

### Issue #14: Update Frontend to Display Tax Breakdown

**Title:** `[MILESTONE-3] Update Frontend to Display Tax Breakdown and Net Income`

**Labels:** `enhancement`, `frontend`, `milestone-3`

**Description:**

Update the frontend results display to show comprehensive tax information and net after-tax income.

**User Story:**
As a user, I want to see my tax breakdown so that I understand how taxes affect my retirement income.

**Acceptance Criteria:**
- [ ] Update `ResultsDisplay.jsx` to show:
  - [ ] Annual gross withdrawal amount
  - [ ] Tax breakdown section:
    - [ ] Taxable account withdrawal
    - [ ] Tax-deferred account withdrawal
    - [ ] Ordinary income tax
    - [ ] Capital gains tax
    - [ ] Total annual taxes
    - [ ] Effective tax rate
  - [ ] Net annual income (after taxes) - prominently displayed
- [ ] Add visual tax breakdown (pie chart or bar chart optional)
- [ ] Format all currency values
- [ ] Add tooltips explaining tax calculations
- [ ] Add "What affects my taxes?" explanation section
- [ ] Update styling to accommodate new information

**Technical Details:**
- Use consistent formatting for currency
- Consider using a chart library (Recharts) for tax breakdown
- Ensure information hierarchy is clear
- Make the display responsive

**Files to Modify:**
- `frontend/src/components/ResultsDisplay.jsx`
- `frontend/src/components/ResultsDisplay.css`

**Files to Create:**
- `frontend/src/components/TaxBreakdown.jsx` (optional, for tax details)

**Dependencies:**
- Issue #13 (API Models Update) must be completed

**Testing:**
- Test with various tax scenarios
- Test formatting of tax amounts
- Test responsiveness
- Verify calculations displayed match API response

---

### Issue #15: Milestone 3 Integration Testing

**Title:** `[MILESTONE-3] Tax Modeling Integration Testing`

**Labels:** `testing`, `milestone-3`

**Description:**

Comprehensive testing of tax calculations and optimization logic.

**Acceptance Criteria:**
- [ ] Test tax calculation accuracy:
  - [ ] Verify against manual tax calculations
  - [ ] Test all tax brackets
  - [ ] Test LTCG rates (0%, 15%, 20%)
  - [ ] Test standard deduction application
- [ ] Test withdrawal order optimization:
  - [ ] Verify withdrawals are optimized for taxes
  - [ ] Compare optimized vs non-optimized scenarios
  - [ ] Demonstrate tax savings
- [ ] Test edge cases:
  - [ ] All assets in taxable account
  - [ ] All assets in tax-deferred account
  - [ ] 50/50 split
  - [ ] Small vs large balances
- [ ] Test net income calculations:
  - [ ] Gross - taxes = net (verify math)
  - [ ] Effective tax rate calculation
- [ ] Document test scenarios and results
- [ ] Create comparison showing tax optimization benefits

**Dependencies:**
- Issue #14 (Frontend Tax Display) must be completed

**Testing:**
- Manual testing with comprehensive scenarios
- Compare with online tax calculators
- Document tax savings from optimization

---

## Milestone 4: Early Retirement & Penalties

### Issue #16: Implement Early Retirement Penalty Logic

**Title:** `[MILESTONE-4] Implement Early Retirement Penalty Detection and Calculation`

**Labels:** `enhancement`, `backend`, `milestone-4`

**Description:**

Add logic to detect early withdrawals (before age 59½) from tax-deferred accounts and apply the 10% penalty.

**User Story:**
As an early retiree, I want to understand the tax penalties I'll face before age 59½ so that I can plan accordingly.

**Acceptance Criteria:**
- [ ] Create `IEarlyRetirementService.cs` interface
- [ ] Create `EarlyRetirementService.cs` implementation
- [ ] Implement age 59.5 detection:
  - [ ] For each simulation year, calculate user's age
  - [ ] Determine if user is under 59.5
- [ ] Apply 10% penalty to tax-deferred withdrawals when age < 59.5
- [ ] Adjust withdrawal order to minimize penalties:
  - [ ] Prefer taxable account withdrawals before 59.5
  - [ ] Only use tax-deferred if necessary
  - [ ] Document withdrawal strategy
- [ ] Track total penalties paid
- [ ] Update simulation to include penalty calculations

**Technical Details:**
- Age 59.5 = 59 years and 6 months
- Only tax-deferred accounts (401k/IRA) subject to penalty
- Penalty applies to withdrawal amount, not taxes
- Consider Rule 72(t) exceptions (out of scope for MVP)

**Files to Create:**
- `backend/RetirementCalculator.API/Services/IEarlyRetirementService.cs`
- `backend/RetirementCalculator.API/Services/EarlyRetirementService.cs`

**Dependencies:**
- Milestone 3 (Tax Modeling) should be completed

**Testing:**
- Unit tests for age 59.5 detection
- Unit tests for penalty calculation
- Test scenarios with early retirement
- Test withdrawal order adjustments
- Verify penalty amounts

---

### Issue #17: Update Models and API for Early Retirement Information

**Title:** `[MILESTONE-4] Update API Models to Include Early Retirement Penalties`

**Labels:** `enhancement`, `backend`, `milestone-4`

**Description:**

Update API models and response to include early retirement penalty information.

**Acceptance Criteria:**
- [ ] Update `CalculationResult.cs` model:
  - [ ] EarlyWithdrawalPenalty (decimal)
  - [ ] YearsWithPenalty (int)
  - [ ] PenaltyWarning (string, optional)
  - [ ] PenaltyExplanation (string)
- [ ] Update calculation controller to:
  - [ ] Inject `IEarlyRetirementService`
  - [ ] Include penalty calculations
  - [ ] Add warning if retirement age < 59.5
- [ ] Update Swagger documentation
- [ ] Register service in Program.cs

**Technical Details:**
- Penalties are in addition to taxes
- Warning should be clear and prominent
- Provide explanation of penalty and how to minimize it

**Files to Modify:**
- `backend/RetirementCalculator.API/Models/CalculationResult.cs`
- `backend/RetirementCalculator.API/Controllers/CalculationController.cs`
- `backend/RetirementCalculator.API/Program.cs`

**Dependencies:**
- Issue #16 (Early Retirement Penalty Logic) must be completed

**Testing:**
- Test API response with early retirement scenarios
- Test API response without early retirement
- Verify penalty amounts are correct
- Verify warnings appear when appropriate

---

### Issue #18: Update Frontend to Display Penalty Information

**Title:** `[MILESTONE-4] Update Frontend to Display Early Retirement Penalty Information`

**Labels:** `enhancement`, `frontend`, `milestone-4`

**Description:**

Update the frontend to show early retirement penalty information and warnings.

**User Story:**
As an early retiree, I want to clearly see the penalties I'll face so that I can make informed decisions about my retirement timing.

**Acceptance Criteria:**
- [ ] Update `ResultsDisplay.jsx` to show:
  - [ ] Early withdrawal penalty amount (if applicable)
  - [ ] Number of years subject to penalty
  - [ ] Total cost of penalties over retirement
- [ ] Add prominent warning banner if retirement age < 59.5:
  - [ ] "Early Retirement Detected"
  - [ ] Explanation of 10% penalty
  - [ ] Years affected
  - [ ] Total penalty cost
- [ ] Add information section:
  - [ ] What are early withdrawal penalties?
  - [ ] How can I minimize them?
  - [ ] Consider delaying retirement to 59.5?
- [ ] Update tax breakdown to show penalties separately
- [ ] Use warning colors (orange/yellow) for penalty information

**Technical Details:**
- Make warnings visually prominent
- Format penalty amounts as currency
- Consider adding a penalty calculator tool (optional)
- Ensure mobile responsiveness

**Files to Modify:**
- `frontend/src/components/ResultsDisplay.jsx`
- `frontend/src/components/ResultsDisplay.css`

**Files to Create:**
- `frontend/src/components/EarlyRetirementWarning.jsx` (optional)

**Dependencies:**
- Issue #17 (API Models Update) must be completed

**Testing:**
- Test with early retirement scenarios (age < 59.5)
- Test with normal retirement scenarios (age >= 59.5)
- Test edge case (retirement at exactly 59.5)
- Verify warnings appear correctly
- Test penalty amount display

---

### Issue #19: Milestone 4 Integration Testing

**Title:** `[MILESTONE-4] Early Retirement Integration Testing`

**Labels:** `testing`, `milestone-4`

**Description:**

Test early retirement penalty calculations and display.

**Acceptance Criteria:**
- [ ] Test early retirement scenarios:
  - [ ] Retirement at age 30, 40, 50, 59
  - [ ] Verify 10% penalty applied correctly
  - [ ] Verify penalties stop at age 59.5
- [ ] Test withdrawal order optimization:
  - [ ] Verify taxable accounts used first
  - [ ] Verify penalties are minimized
- [ ] Test edge cases:
  - [ ] Retirement at exactly 59.5
  - [ ] Retirement at 60 (no penalty)
  - [ ] All assets in taxable (no penalty possible)
  - [ ] All assets in tax-deferred (penalty unavoidable)
- [ ] Test warning displays:
  - [ ] Warnings appear for early retirement
  - [ ] Warnings don't appear for normal retirement
  - [ ] Penalty amounts are correct
- [ ] Calculate total cost of early retirement
- [ ] Document findings

**Dependencies:**
- Issue #18 (Frontend Penalty Display) must be completed

**Testing:**
- Manual testing with various ages
- Verify penalty math
- Compare early vs delayed retirement scenarios

---

## Final Integration & Polish

### Issue #20: Create Visualization Component with Recharts

**Title:** `[FINAL] Create Visualization Component for Results`

**Labels:** `enhancement`, `frontend`, `visualization`

**Description:**

Create data visualizations to help users understand their retirement withdrawal strategy.

**User Story:**
As a user, I want visual charts so that I can easily understand my retirement plan's sustainability and tax breakdown.

**Acceptance Criteria:**
- [ ] Create `Visualization.jsx` component
- [ ] Implement success rate gauge or progress bar:
  - [ ] Show achieved success rate (90%, 95%, 98%)
  - [ ] Color coding (green for high, yellow for medium, red for low)
- [ ] Implement tax breakdown pie chart:
  - [ ] Ordinary income tax
  - [ ] Capital gains tax
  - [ ] Early withdrawal penalty (if applicable)
  - [ ] Net income
- [ ] Optional: Portfolio sustainability chart (line chart):
  - [ ] Show potential portfolio balance over time
  - [ ] Multiple scenarios (best, worst, median)
- [ ] Add interactive tooltips
- [ ] Make charts responsive
- [ ] Add legends and labels

**Technical Details:**
- Use Recharts library (already installed)
- Follow consistent color scheme
- Ensure accessibility (ARIA labels)
- Handle missing/null data gracefully

**Files to Create:**
- `frontend/src/components/Visualization.jsx`
- `frontend/src/components/Visualization.css`

**Dependencies:**
- All previous milestones should be completed

**Testing:**
- Test with various data scenarios
- Test responsiveness
- Test on different browsers
- Verify accessibility

---

### Issue #21: UI/UX Polish and Styling

**Title:** `[FINAL] UI/UX Polish and Styling Improvements`

**Labels:** `enhancement`, `frontend`, `design`

**Description:**

Polish the user interface for a professional, user-friendly experience.

**Acceptance Criteria:**
- [ ] Consistent design system:
  - [ ] Color palette
  - [ ] Typography
  - [ ] Spacing
  - [ ] Button styles
- [ ] Responsive design:
  - [ ] Mobile (320px+)
  - [ ] Tablet (768px+)
  - [ ] Desktop (1024px+)
- [ ] Loading states:
  - [ ] Skeleton loaders
  - [ ] Spinners
  - [ ] Progress indicators
- [ ] Error states:
  - [ ] User-friendly error messages
  - [ ] Error boundaries
  - [ ] Retry options
- [ ] Transitions and animations (subtle):
  - [ ] Form transitions
  - [ ] Results fade-in
  - [ ] Smooth scrolling
- [ ] Accessibility:
  - [ ] ARIA labels
  - [ ] Keyboard navigation
  - [ ] Screen reader support
  - [ ] Color contrast (WCAG AA)
- [ ] Add favicon and page title
- [ ] Add meta tags for SEO

**Technical Details:**
- Consider using CSS-in-JS or CSS modules
- Use CSS variables for theme consistency
- Test with accessibility tools (Lighthouse, axe)

**Files to Modify:**
- All frontend component CSS files
- `frontend/public/index.html`

**Dependencies:**
- All frontend components should exist

**Testing:**
- Visual regression testing
- Accessibility testing
- Cross-browser testing
- Mobile device testing

---

### Issue #22: Add Comprehensive Error Handling and Logging

**Title:** `[FINAL] Implement Comprehensive Error Handling and Logging`

**Labels:** `enhancement`, `backend`, `frontend`

**Description:**

Implement robust error handling and logging across the application.

**User Story:**
As a developer, I want comprehensive error handling and logging so that I can debug issues and maintain the application effectively.

**Acceptance Criteria:**

**Backend:**
- [ ] Add structured logging (Serilog or built-in)
- [ ] Log all API requests and responses
- [ ] Log calculation performance metrics
- [ ] Log errors with stack traces
- [ ] Add correlation IDs for request tracking
- [ ] Configure different log levels (Debug, Info, Warning, Error)
- [ ] Add global exception handler middleware

**Frontend:**
- [ ] Add error boundary components
- [ ] Log errors to console (development)
- [ ] Consider error reporting service (production)
- [ ] Add user-friendly error messages
- [ ] Add retry mechanisms for failed requests
- [ ] Handle network errors gracefully

**Technical Details:**
- Configure logging in appsettings.json
- Don't log sensitive user data
- Use appropriate log levels
- Consider log aggregation for production

**Files to Modify:**
- `backend/RetirementCalculator.API/Program.cs`
- Multiple backend service files
- `frontend/src/App.jsx` (error boundary)
- `frontend/src/services/apiClient.js` (error handling)

**Dependencies:**
- None (can be done anytime)

**Testing:**
- Trigger various error scenarios
- Verify errors are logged
- Verify user sees friendly messages
- Test retry mechanisms

---

### Issue #23: Performance Optimization

**Title:** `[FINAL] Performance Optimization and Benchmarking`

**Labels:** `enhancement`, `performance`

**Description:**

Optimize application performance to meet the < 2 second response time requirement.

**User Story:**
As a user, I want fast calculation results so that I can iterate quickly on my retirement planning scenarios.

**Acceptance Criteria:**
- [ ] Backend optimizations:
  - [ ] Profile calculation performance
  - [ ] Optimize historical simulation loops
  - [ ] Consider parallel processing for scenarios
  - [ ] Add response time logging
  - [ ] Cache historical data (already done)
  - [ ] Optimize withdrawal rate search algorithm
- [ ] Frontend optimizations:
  - [ ] Code splitting
  - [ ] Lazy loading of components
  - [ ] Optimize re-renders (React.memo, useMemo)
  - [ ] Compress and optimize images
  - [ ] Minimize bundle size
- [ ] API optimizations:
  - [ ] Enable response compression
  - [ ] Add caching headers (if applicable)
- [ ] Performance testing:
  - [ ] Measure API response time
  - [ ] Measure frontend render time
  - [ ] Test with various input scenarios
  - [ ] Ensure < 2 second total time
- [ ] Document performance metrics

**Technical Details:**
- Use profiling tools (dotnet-trace, Chrome DevTools)
- Consider algorithm improvements
- Balance accuracy vs performance

**Files to Modify:**
- Multiple backend service files
- Frontend build configuration
- `backend/RetirementCalculator.API/Program.cs` (compression)

**Dependencies:**
- All features should be implemented first

**Testing:**
- Benchmark various scenarios
- Load testing (if needed)
- Document performance improvements

---

### Issue #24: Documentation and Code Comments

**Title:** `[FINAL] Add Comprehensive Documentation and Code Comments`

**Labels:** `documentation`

**Description:**

Add comprehensive documentation for the codebase to support future development and maintenance.

**Acceptance Criteria:**
- [ ] Backend code comments:
  - [ ] XML documentation for all public methods
  - [ ] Comments for complex algorithms
  - [ ] Comments for tax calculations
  - [ ] Comments for withdrawal optimization logic
- [ ] Frontend code comments:
  - [ ] JSDoc for components and functions
  - [ ] Comments for complex logic
  - [ ] PropTypes or TypeScript interfaces (if added)
- [ ] API documentation:
  - [ ] Update Swagger/OpenAPI descriptions
  - [ ] Add example requests/responses
  - [ ] Document error codes
- [ ] User documentation:
  - [ ] Create USER_GUIDE.md
  - [ ] Explain how to use the calculator
  - [ ] Explain what the results mean
  - [ ] Add disclaimers and assumptions
- [ ] Developer documentation:
  - [ ] Update README files
  - [ ] Add CONTRIBUTING.md
  - [ ] Document architecture decisions
  - [ ] Add setup instructions
- [ ] Calculation methodology document:
  - [ ] Explain algorithms used
  - [ ] Document assumptions
  - [ ] Cite sources (Trinity Study, Bengen, etc.)

**Files to Create:**
- `USER_GUIDE.md`
- `CONTRIBUTING.md`
- `CALCULATION_METHODOLOGY.md`

**Files to Modify:**
- All source code files (add comments)
- README files (update with latest info)

**Dependencies:**
- None (can be done anytime)

**Testing:**
- Review documentation for accuracy
- Get feedback from potential users

---

### Issue #25: Final End-to-End Testing and MVP Validation

**Title:** `[FINAL] Final End-to-End Testing and MVP Launch Validation`

**Labels:** `testing`, `mvp`

**Description:**

Comprehensive final testing of the complete MVP before launch.

**Acceptance Criteria:**
- [ ] Complete user flow testing:
  - [ ] Enter data → Calculate → View results
  - [ ] Test all input combinations
  - [ ] Test all success thresholds
  - [ ] Test early retirement scenarios
  - [ ] Test normal retirement scenarios
- [ ] Cross-browser testing:
  - [ ] Chrome
  - [ ] Firefox
  - [ ] Safari
  - [ ] Edge
- [ ] Device testing:
  - [ ] Desktop (various resolutions)
  - [ ] Tablet (iPad, Android)
  - [ ] Mobile (various sizes)
- [ ] Accessibility testing:
  - [ ] Keyboard navigation
  - [ ] Screen reader compatibility
  - [ ] Color contrast
  - [ ] ARIA labels
- [ ] Performance validation:
  - [ ] API response < 2 seconds
  - [ ] Frontend render < 1 second
  - [ ] Total time < 3 seconds
- [ ] Calculation validation:
  - [ ] Results are deterministic
  - [ ] Results align with known studies
  - [ ] Tax calculations are accurate
  - [ ] Penalties are calculated correctly
- [ ] Edge case testing:
  - [ ] All scenarios from project_spec.md section 9
  - [ ] Document any issues
- [ ] Security testing:
  - [ ] Input validation
  - [ ] XSS prevention
  - [ ] SQL injection prevention (if applicable)
  - [ ] CORS configuration
- [ ] Launch checklist:
  - [ ] All 4 milestones completed
  - [ ] Results are clear and understandable
  - [ ] Calculations are reproducible
  - [ ] Users can experiment with inputs
  - [ ] Appropriate disclaimers in place
  - [ ] Basic documentation available

**Dependencies:**
- ALL previous issues must be completed

**Testing:**
- Create comprehensive test plan
- Execute all test scenarios
- Document all findings
- Create bug reports for any issues
- Verify all MVP launch criteria met

---

## Labels to Create

Create these labels in your GitHub repository:

- `enhancement` - New feature or request
- `bug` - Something isn't working
- `documentation` - Improvements or additions to documentation
- `testing` - Related to testing
- `frontend` - Frontend React code
- `backend` - Backend .NET code
- `milestone-1` - Milestone 1: Basic UI & Data Flow
- `milestone-2` - Milestone 2: Historical Withdrawal Modeling
- `milestone-3` - Milestone 3: Tax Modeling
- `milestone-4` - Milestone 4: Early Retirement & Penalties
- `integration` - Integration between components
- `performance` - Performance optimization
- `design` - UI/UX design
- `visualization` - Data visualization
- `mvp` - MVP launch related

---

## Milestones to Create

Create these milestones in your GitHub repository:

1. **Milestone 1: Basic UI & Data Flow**
   - Due date: (your choice)
   - Description: Establish end-to-end communication between UI and API
   - Issues: #1-5

2. **Milestone 2: Historical Withdrawal Modeling**
   - Due date: (your choice)
   - Description: Calculate safe withdrawal rates using historical simulations
   - Issues: #6-10

3. **Milestone 3: Tax Modeling**
   - Due date: (your choice)
   - Description: Add realistic tax considerations
   - Issues: #11-15

4. **Milestone 4: Early Retirement & Penalties**
   - Due date: (your choice)
   - Description: Support users retiring before age 59½
   - Issues: #16-19

5. **Final Integration & Launch**
   - Due date: (your choice)
   - Description: Polish, testing, and MVP launch
   - Issues: #20-25

---

## How to Use This Document

1. **Create Labels**: Go to your GitHub repository → Issues → Labels → New label
2. **Create Milestones**: Go to your GitHub repository → Issues → Milestones → New milestone
3. **Create Issues**: Copy each issue section above and create a new issue on GitHub
   - Use the title exactly as shown
   - Copy the full description
   - Add appropriate labels
   - Assign to the correct milestone
4. **Prioritize**: Start with Milestone 1 issues (#1-5)
5. **Track Progress**: Use GitHub's project board or milestones view to track progress

---

**Total Issues Created: 25**
**Total Milestones: 5**
**Estimated MVP Completion: Based on your development velocity**
