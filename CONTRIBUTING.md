# Contributing to Retirement Calculator

Thank you for your interest in contributing to the Retirement Calculator project! This document provides guidelines and instructions for contributing.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Project Structure](#project-structure)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Testing Guidelines](#testing-guidelines)
- [Submitting Changes](#submitting-changes)
- [Reporting Issues](#reporting-issues)

## Code of Conduct

This project adheres to a code of conduct that all contributors are expected to follow:

- Be respectful and inclusive
- Welcome newcomers and help them learn
- Focus on constructive feedback
- Respect differing viewpoints and experiences
- Accept responsibility for mistakes and learn from them

## Getting Started

### Prerequisites

Before contributing, ensure you have:

- **Git**: Version control system
- **Node.js**: Version 18+ (for frontend)
- **.NET SDK**: Version 8.0+ (for backend)
- **Code Editor**: VS Code, Visual Studio, or similar
- **GitHub Account**: For submitting pull requests

### First-Time Contributors

If you're new to the project:

1. Read the [README.md](README.md) to understand the project
2. Review the [USER_GUIDE.md](USER_GUIDE.md) to understand functionality
3. Check the [CALCULATION_METHODOLOGY.md](CALCULATION_METHODOLOGY.md) for technical details
4. Look for issues tagged with `good-first-issue`

## Development Setup

### Clone the Repository

```bash
git clone https://github.com/yourusername/retirement-calculator.git
cd retirement-calculator
```

### Backend Setup

```bash
cd backend/RetirementCalculator.API
dotnet restore
dotnet build
dotnet run
```

The API will start on `http://localhost:5000`

### Frontend Setup

```bash
cd frontend
npm install
npm run dev
```

The frontend will start on `http://localhost:3000`

### Running Both Simultaneously

On Windows, you can use the provided PowerShell scripts:
```powershell
.\scripts\start-dev.ps1   # Start both servers
.\scripts\stop-dev.ps1    # Stop both servers
```

## Project Structure

```
retirement-calculator/
├── backend/
│   └── RetirementCalculator.API/
│       ├── Controllers/         # API endpoints
│       ├── Models/              # Data models
│       ├── Services/            # Business logic
│       │   ├── HistoricalDataService.cs
│       │   ├── TaxCalculationService.cs
│       │   ├── WithdrawalCalculationService.cs
│       │   └── EarlyRetirementService.cs
│       ├── data/                # Historical market data
│       └── Program.cs           # Application configuration
├── frontend/
│   └── src/
│       ├── components/          # React components
│       │   ├── InputForm.jsx
│       │   ├── ResultsDisplay.jsx
│       │   ├── Visualization.jsx
│       │   └── ErrorBoundary.jsx
│       ├── services/            # API client
│       └── App.jsx              # Main application
├── docs/                        # Documentation
├── USER_GUIDE.md
├── CALCULATION_METHODOLOGY.md
└── CONTRIBUTING.md
```

## Development Workflow

### Branch Naming

Use descriptive branch names:

- `feature/add-social-security-integration`
- `bugfix/fix-tax-calculation-error`
- `enhancement/improve-chart-performance`
- `docs/update-user-guide`

### Commit Messages

Write clear, descriptive commit messages:

```
Add Social Security integration to calculations

- Create new SocialSecurityService
- Update calculation controller to include SS benefits
- Add input fields for SS claiming age
- Update UI to display combined income
```

Format:
- First line: Brief summary (50 chars or less)
- Blank line
- Detailed description with bullet points

### Making Changes

1. **Create a Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make Your Changes**
   - Follow coding standards (see below)
   - Write/update tests as needed
   - Update documentation if applicable

3. **Test Your Changes**
   - Run backend tests: `dotnet test`
   - Test frontend: Manual testing and build
   - Verify calculations are accurate

4. **Commit Your Changes**
   ```bash
   git add .
   git commit -m "Your descriptive commit message"
   ```

5. **Push to GitHub**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Create Pull Request**
   - Go to GitHub and create a PR
   - Fill out the PR template
   - Link to any related issues

## Coding Standards

### C# Backend Standards

#### Naming Conventions
- **Classes/Interfaces**: PascalCase (`TaxCalculationService`, `IHistoricalDataService`)
- **Methods**: PascalCase (`CalculateWithdrawalStrategy`)
- **Private fields**: _camelCase with underscore (`_logger`, `_dataService`)
- **Parameters/locals**: camelCase (`userInput`, `withdrawalRate`)

#### Code Style
```csharp
// Use XML documentation for public methods
/// <summary>
/// Calculates the optimal withdrawal rate based on historical data
/// </summary>
/// <param name="userInput">User input parameters</param>
/// <returns>Calculation result with withdrawal strategy</returns>
public async Task<CalculationResult> CalculateWithdrawalStrategy(UserInput userInput)
{
    // Implementation
}

// Use early returns for validation
if (userInput == null)
{
    throw new ArgumentNullException(nameof(userInput));
}

// Use clear variable names
var effectiveTaxRate = totalTaxes / grossIncome;

// Use LINQ for data operations
var successfulScenarios = scenarios.Count(s => s.Success);
```

#### Logging
- Use structured logging with ILogger
- Log important business events
- Include relevant context in log messages
- Use appropriate log levels (Information, Warning, Error)

```csharp
_logger.LogInformation(
    "Calculation completed - WithdrawalRate: {WithdrawalRate}%, " +
    "SuccessRate: {SuccessRate}%",
    result.WithdrawalRate,
    result.AchievedSuccessRate
);
```

### JavaScript/React Frontend Standards

#### Naming Conventions
- **Components**: PascalCase (`InputForm`, `ResultsDisplay`)
- **Functions**: camelCase (`calculateRetirement`, `formatCurrency`)
- **Constants**: UPPER_SNAKE_CASE (`MAX_RETRIES`, `API_BASE_URL`)
- **Props/State**: camelCase (`userInput`, `isLoading`)

#### Code Style
```javascript
// Use JSDoc for component documentation
/**
 * Input form component for collecting user retirement data
 * @param {Object} props - Component props
 * @param {Function} props.onCalculationComplete - Callback when calculation succeeds
 */
function InputForm({ onCalculationComplete }) {
  // Implementation
}

// Use destructuring
const { currentAge, retirementAge } = formData;

// Use arrow functions for handlers
const handleSubmit = async (e) => {
  e.preventDefault();
  // Implementation
};

// Use hooks appropriately
const [formData, setFormData] = useState(initialState);
const memoizedValue = useMemo(() => expensiveCalculation(), [dep]);
```

#### Performance
- Use `React.memo` for components that render often
- Use `useMemo` for expensive calculations
- Use `useCallback` for event handlers passed to child components
- Avoid creating functions/objects in render

### CSS Standards

- Use meaningful class names
- Follow BEM naming for complex components
- Use CSS variables for theme values
- Mobile-first responsive design
- Ensure accessibility (color contrast, focus states)

```css
/* Component-specific styles */
.input-form-container {
  /* Styles */
}

/* State modifiers */
.submit-button:disabled {
  /* Styles */
}

/* Responsive design */
@media (max-width: 768px) {
  .input-form-container {
    /* Mobile styles */
  }
}
```

## Testing Guidelines

### Backend Testing

Create unit tests for:
- Service methods
- Tax calculations
- Withdrawal strategies
- Data transformations

```csharp
[Fact]
public void CalculateTax_WithStandardDeduction_ReturnsCorrectTax()
{
    // Arrange
    var service = new TaxCalculationService();
    var income = 50000m;

    // Act
    var tax = service.CalculateOrdinaryIncomeTax(income);

    // Assert
    Assert.True(tax > 0);
    Assert.True(tax < income);
}
```

### Frontend Testing

Test:
- Component rendering
- User interactions
- Form validation
- API error handling

Manual testing checklist:
- [ ] Form validates input correctly
- [ ] Calculations complete successfully
- [ ] Results display all expected data
- [ ] Visualizations render properly
- [ ] Error states display correctly
- [ ] Responsive design works on mobile
- [ ] Keyboard navigation works
- [ ] Screen reader compatibility

### Calculation Validation

When modifying calculations:
1. Compare against known results from financial calculators
2. Test edge cases (very low/high values, early/late retirement)
3. Verify against published research (Trinity Study, etc.)
4. Document any assumptions or limitations

## Submitting Changes

### Pull Request Process

1. **Update Documentation**
   - Update README if adding features
   - Update USER_GUIDE for user-facing changes
   - Update CALCULATION_METHODOLOGY for calculation changes

2. **Self-Review**
   - Review your own code first
   - Check for console.logs or debug code
   - Verify formatting and style
   - Run tests

3. **Create Pull Request**
   - Use descriptive title
   - Fill out PR template completely
   - Link related issues
   - Add screenshots for UI changes

4. **Code Review**
   - Address reviewer comments
   - Push additional commits as needed
   - Keep discussion professional and constructive

5. **Merge**
   - Wait for approval
   - Ensure CI/CD passes
   - Squash commits if requested

### Pull Request Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Enhancement
- [ ] Documentation update

## Testing
How has this been tested?

## Checklist
- [ ] Code follows project style guidelines
- [ ] Self-review performed
- [ ] Comments added for complex code
- [ ] Documentation updated
- [ ] No new warnings
- [ ] Tests added/updated
- [ ] All tests pass

## Screenshots (if applicable)
```

## Reporting Issues

### Bug Reports

When reporting bugs, include:

1. **Description**: Clear description of the bug
2. **Steps to Reproduce**: Exact steps to trigger the bug
3. **Expected Behavior**: What should happen
4. **Actual Behavior**: What actually happens
5. **Environment**: Browser, OS, versions
6. **Screenshots**: If applicable
7. **Console Errors**: Any error messages

### Feature Requests

When requesting features:

1. **Use Case**: Why is this needed?
2. **Proposed Solution**: How should it work?
3. **Alternatives**: Other ways to solve the problem
4. **Impact**: Who would benefit?

### Issue Labels

- `bug`: Something isn't working
- `enhancement`: New feature or request
- `documentation`: Documentation improvements
- `good-first-issue`: Good for newcomers
- `help-wanted`: Extra attention needed
- `question`: Further information requested
- `wontfix`: Will not be worked on

## Areas for Contribution

### High-Priority Enhancements

1. **Social Security Integration**
   - Add SS benefit calculations
   - Model claiming age strategies
   - Integrate with withdrawal calculations

2. **State Tax Support**
   - Add state tax calculations
   - Support multiple states
   - Model state tax differences in retirement

3. **Required Minimum Distributions (RMDs)**
   - Calculate RMDs starting at age 73
   - Ensure compliance in calculations
   - Show impact on tax liability

4. **Healthcare Cost Modeling**
   - Medicare premium calculations
   - Pre-Medicare healthcare costs
   - Long-term care considerations

5. **Advanced Portfolio Options**
   - Custom stock/bond allocations
   - Glide path strategies
   - Multiple asset classes

### Documentation Improvements

- Expand user guide with more examples
- Add video tutorials
- Create developer documentation
- Translate to other languages

### Testing and Quality

- Increase test coverage
- Add integration tests
- Performance benchmarking
- Accessibility improvements

## Questions?

If you have questions:

1. Check existing documentation
2. Search closed issues
3. Ask in discussion forums
4. Create a new issue with `question` label

## Thank You!

Your contributions make this project better for everyone. We appreciate your time and effort!
