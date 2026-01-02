# Retirement Calculator MVP - Implementation Plan

## Project Structure

```
Tester2/
├── frontend/                 # React application
│   ├── src/
│   │   ├── components/      # React components
│   │   │   ├── InputForm.jsx
│   │   │   ├── ResultsDisplay.jsx
│   │   │   └── Visualization.jsx
│   │   ├── services/        # API client
│   │   │   └── apiClient.js
│   │   ├── App.jsx
│   │   └── index.js
│   ├── public/
│   └── package.json
│
├── backend/                  # .NET Core Web API
│   ├── RetirementCalculator.API/
│   │   ├── Controllers/
│   │   │   └── CalculationController.cs
│   │   ├── Models/
│   │   │   ├── UserInput.cs
│   │   │   └── CalculationResult.cs
│   │   ├── Services/
│   │   │   ├── HistoricalDataService.cs
│   │   │   ├── WithdrawalCalculationService.cs
│   │   │   ├── TaxCalculationService.cs
│   │   │   └── EarlyRetirementService.cs
│   │   ├── Data/
│   │   │   └── historical_data.json
│   │   └── Program.cs
│   └── RetirementCalculator.sln
│
└── data/                     # Historical market data
    └── market_data.csv
```

## Implementation Strategy

### Phase 1: Project Setup (Parallel Execution)
**Agents: 2 (Frontend Setup + Backend Setup)**

1. **Agent 1: Frontend Setup**
   - Initialize React app with Vite
   - Install dependencies (React, Axios, Chart.js)
   - Create basic folder structure
   - Set up proxy for API calls

2. **Agent 2: Backend Setup**
   - Initialize .NET Core Web API project
   - Configure CORS for React frontend
   - Set up project structure
   - Add required NuGet packages

### Phase 2: Milestone 1 - Basic UI & Data Flow (Parallel Execution)
**Agents: 2 (Frontend Form + Backend API)**

1. **Agent 1: Frontend - Input Form**
   - Build form with all MVP fields:
     - Current age (18-100)
     - Retirement age (>= current age)
     - Retirement account balance
     - Taxable account balance
     - Success rate threshold (dropdown)
   - Add input validation
   - Create API service client
   - Handle form submission and response display

2. **Agent 2: Backend - API Endpoint**
   - Create UserInput model
   - Create CalculationResult model
   - Build POST endpoint for calculations
   - Implement input validation
   - Return echo response for testing

### Phase 3: Milestone 2 - Historical Withdrawal Modeling (Sequential)
**Agent: 1 (Complex interdependent logic)**

1. **Historical Data Setup**
   - Create/load 100 years of market data
   - S&P 500 returns
   - Bond returns
   - Inflation rates (CPI)

2. **Calculation Engine**
   - Portfolio allocation logic (default 60/40 stocks/bonds)
   - Rolling window analysis across historical data
   - Annual rebalancing
   - Inflation adjustment
   - Success rate calculation
   - Withdrawal rate optimizer

### Phase 4: Milestone 3 - Tax Modeling (Sequential)
**Agent: 1 (Builds on Milestone 2)**

1. **Tax Infrastructure**
   - Federal tax brackets (2025)
   - Standard deduction
   - Long-term capital gains rates

2. **Tax-Optimized Withdrawal Logic**
   - Separate handling for taxable vs tax-deferred accounts
   - Withdrawal order optimization
   - Minimize lifetime tax burden
   - Calculate gross and net income
   - Tax breakdown for display

### Phase 5: Milestone 4 - Early Retirement & Penalties (Sequential)
**Agent: 1 (Builds on Milestone 3)**

1. **Early Withdrawal Detection**
   - Detect withdrawals before age 59½
   - Apply 10% penalty to applicable withdrawals
   - Adjust withdrawal order to minimize penalties
   - Display penalty warnings and amounts

### Phase 6: Integration & Visualization (Parallel Execution)
**Agents: 2 (Frontend UI + Testing)**

1. **Agent 1: Results Display & Charts**
   - Results summary card
   - Tax breakdown section
   - Success rate visualization
   - Portfolio sustainability chart
   - Disclaimers

2. **Agent 2: Testing & Validation**
   - Test with known scenarios (4% rule baseline)
   - Validate tax calculations
   - Test edge cases
   - Performance testing

## Parallel Execution Plan

### Batch 1: Project Setup
- **Subagent 1**: Frontend project initialization
- **Subagent 2**: Backend project initialization

### Batch 2: Milestone 1 Implementation
- **Subagent 1**: React form components + validation
- **Subagent 2**: API models + endpoint + validation

### Batch 3: Core Calculation Logic (Sequential)
- **Subagent 1**: Milestone 2 (Historical modeling)
- Then **Subagent 1**: Milestone 3 (Tax modeling)
- Then **Subagent 1**: Milestone 4 (Early retirement)

### Batch 4: Final Integration
- **Subagent 1**: Results display + visualization
- **Subagent 2**: Testing and validation

## Key Technical Decisions

1. **Default Portfolio Allocation**: 60% stocks / 40% bonds
2. **Historical Data Period**: 1925-2024 (100 years)
3. **Rebalancing**: Annual
4. **Withdrawal Strategy**: Fixed real-dollar (inflation-adjusted)
5. **Tax Year**: 2025 federal rates
6. **Success Rate Options**: 90%, 95%, 98%

## Data Requirements

### Historical Market Data (JSON format)
```json
[
  {
    "year": 1925,
    "sp500Return": 0.334,
    "bondReturn": 0.055,
    "inflation": 0.035
  },
  ...
]
```

## Deliverables

1. Working React frontend with input form
2. .NET Core Web API with calculation engine
3. Historical data integration
4. Tax optimization logic
5. Early retirement penalty handling
6. Results visualization
7. Basic documentation

## Timeline Estimate

- Phase 1: Setup (Parallel) - Quick
- Phase 2: Milestone 1 (Parallel) - Quick
- Phase 3: Milestone 2 (Sequential) - Moderate
- Phase 4: Milestone 3 (Sequential) - Moderate
- Phase 5: Milestone 4 (Sequential) - Quick
- Phase 6: Integration (Parallel) - Quick

## Success Criteria

- All inputs validated properly
- Calculations are deterministic
- Results align with 4% rule baseline
- Tax calculations are accurate
- Early retirement penalties applied correctly
- Response time < 2 seconds
- Clear, understandable results
