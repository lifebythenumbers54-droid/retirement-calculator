import './ResultsDisplay.css'

function ResultsDisplay({ results, onCalculateAgain }) {
  const formatCurrency = (value) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(value)
  }

  const formatPercentage = (value) => {
    return `${value.toFixed(2)}%`
  }

  const getSuccessRateColor = (rate) => {
    if (rate >= 95) return 'success-high'
    if (rate >= 90) return 'success-medium'
    return 'success-low'
  }

  const getSuccessRateLabel = (rate) => {
    if (rate >= 95) return 'Excellent'
    if (rate >= 90) return 'Good'
    return 'Moderate'
  }

  return (
    <div className="results-display-container">
      <h2>Your Retirement Withdrawal Strategy</h2>

      <div className="results-grid">
        <div className="result-card primary">
          <div className="result-icon">📊</div>
          <div className="result-content">
            <h3>Recommended Withdrawal Rate</h3>
            <p className="result-value">{formatPercentage(results.withdrawalRate)}</p>
            <p className="result-description">Safe annual withdrawal rate</p>
          </div>
        </div>

        <div className="result-card">
          <div className="result-icon">💰</div>
          <div className="result-content">
            <h3>Annual Gross Withdrawal</h3>
            <p className="result-value">{formatCurrency(results.annualGrossWithdrawal)}</p>
            <p className="result-description">Before taxes</p>
          </div>
        </div>

        <div className="result-card highlight">
          <div className="result-icon">✅</div>
          <div className="result-content">
            <h3>Net Annual Income</h3>
            <p className="result-value">{formatCurrency(results.netAnnualIncome)}</p>
            <p className="result-description">After-tax retirement income</p>
          </div>
        </div>

        <div className={`result-card ${getSuccessRateColor(results.achievedSuccessRate)}`}>
          <div className="result-icon">🎯</div>
          <div className="result-content">
            <h3>Success Rate Achieved</h3>
            <p className="result-value">{formatPercentage(results.achievedSuccessRate)}</p>
            <p className="result-description">{getSuccessRateLabel(results.achievedSuccessRate)} probability</p>
            <div className="progress-bar" role="progressbar" aria-valuenow={results.achievedSuccessRate} aria-valuemin="0" aria-valuemax="100">
              <div
                className={`progress-fill ${getSuccessRateColor(results.achievedSuccessRate)}`}
                style={{ width: `${results.achievedSuccessRate}%` }}
              ></div>
            </div>
          </div>
        </div>

        <div className="result-card">
          <div className="result-icon">📈</div>
          <div className="result-content">
            <h3>Scenarios Simulated</h3>
            <p className="result-value">{results.numberOfScenariosSimulated.toLocaleString()}</p>
            <p className="result-description">Historical data points analyzed</p>
          </div>
        </div>
      </div>

      <div className="tax-breakdown-section">
        <h3>Tax Breakdown & Withdrawal Strategy</h3>
        <div className="tax-grid">
          <div className="tax-card">
            <h4>Tax-Optimized Withdrawal Sources</h4>
            <div className="tax-detail-row">
              <span className="tax-label">From Taxable Accounts:</span>
              <span className="tax-value">{formatCurrency(results.taxableAccountWithdrawal)}</span>
            </div>
            <div className="tax-detail-row">
              <span className="tax-label">From Tax-Deferred Accounts:</span>
              <span className="tax-value">{formatCurrency(results.taxDeferredAccountWithdrawal)}</span>
            </div>
            <div className="tax-detail-row total">
              <span className="tax-label">Total Withdrawal:</span>
              <span className="tax-value">{formatCurrency(results.annualGrossWithdrawal)}</span>
            </div>
          </div>

          <div className="tax-card">
            <h4>Tax Calculation Details</h4>
            <div className="tax-detail-row">
              <span className="tax-label">Ordinary Income Tax:</span>
              <span className="tax-value">{formatCurrency(results.ordinaryIncomeTax)}</span>
            </div>
            <div className="tax-detail-row">
              <span className="tax-label">Long-Term Capital Gains Tax:</span>
              <span className="tax-value">{formatCurrency(results.capitalGainsTax)}</span>
            </div>
            <div className="tax-detail-row total">
              <span className="tax-label">Total Tax:</span>
              <span className="tax-value">{formatCurrency(results.estimatedAnnualTaxes)}</span>
            </div>
            <div className="tax-detail-row highlight">
              <span className="tax-label">Effective Tax Rate:</span>
              <span className="tax-value">{formatPercentage(results.effectiveTaxRate)}</span>
            </div>
          </div>
        </div>
        <div className="tax-strategy-note">
          <p>
            <strong>Tax Optimization:</strong> Our strategy minimizes your tax burden by withdrawing from tax-deferred
            accounts up to the 12% tax bracket, then using taxable accounts to benefit from lower capital gains rates.
          </p>
        </div>
      </div>

      <div className="info-section">
        <h3>What This Means</h3>
        <ul>
          <li>
            You can withdraw <strong>{formatCurrency(results.annualGrossWithdrawal)}</strong> per year
            from your total retirement savings
          </li>
          <li>
            After estimated taxes of <strong>{formatCurrency(results.estimatedAnnualTaxes)}</strong>,
            you'll have <strong>{formatCurrency(results.netAnnualIncome)}</strong> in annual income
          </li>
          <li>
            This withdrawal rate has a <strong>{formatPercentage(results.achievedSuccessRate)}</strong> success
            rate based on historical market data
          </li>
          <li>
            These results are based on <strong>{results.numberOfScenariosSimulated}</strong> historical
            market scenarios from 1925-2024
          </li>
        </ul>
      </div>

      <div className="disclaimer-section">
        <h3>⚠️ Important Disclaimer</h3>
        <p>
          These calculations are based on historical market data and assume a 60/40 stock/bond portfolio allocation.
          <strong> Past performance does not guarantee future results.</strong> Actual returns may vary significantly
          due to market volatility, economic conditions, and individual circumstances.
        </p>
        <p>
          This tool provides estimates only and should not be considered financial advice. Please consult with a
          qualified financial advisor before making retirement decisions.
        </p>
      </div>

      <button onClick={onCalculateAgain} className="calculate-again-button">
        Calculate Again
      </button>
    </div>
  )
}

export default ResultsDisplay
