import './ResultsDisplay.css'
import Visualization from './Visualization'

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

      {results.penaltyWarning && (
        <div className="early-retirement-warning">
          <div className="warning-icon">⚠️</div>
          <div className="warning-content">
            <h3>Early Retirement Penalty Notice</h3>
            <p>{results.penaltyWarning}</p>
            <div className="penalty-summary">
              <div className="penalty-stat">
                <span className="penalty-label">Years with Penalty:</span>
                <span className="penalty-value">{results.yearsWithPenalty}</span>
              </div>
              <div className="penalty-stat">
                <span className="penalty-label">Total Penalty Cost:</span>
                <span className="penalty-value">{formatCurrency(results.earlyWithdrawalPenalty)}</span>
              </div>
            </div>
            <p className="penalty-explanation">{results.penaltyExplanation}</p>
          </div>
        </div>
      )}

      {results.rothConversionAnalysis && (
        <div className={`roth-conversion-strategy ${results.rothConversionAnalysis.isRecommended ? 'recommended' : 'not-recommended'}`}>
          <div className="strategy-header">
            <div className="strategy-icon">{results.rothConversionAnalysis.isRecommended ? '✅' : 'ℹ️'}</div>
            <h3>{results.rothConversionAnalysis.isRecommended ? 'Roth Conversion Ladder: Recommended Strategy' : 'Roth Conversion Ladder: Analysis'}</h3>
          </div>

          <div className="strategy-content">
            <div className="strategy-comparison">
              <div className="comparison-card penalty-approach">
                <h4>Standard Penalty Approach</h4>
                <div className="comparison-amount">{formatCurrency(results.rothConversionAnalysis.totalPenaltyCost)}</div>
                <p>Total cost (penalties + taxes)</p>
              </div>

              <div className="comparison-vs">vs</div>

              <div className="comparison-card conversion-approach">
                <h4>Roth Conversion Ladder</h4>
                <div className="comparison-amount">{formatCurrency(results.rothConversionAnalysis.totalConversionTaxCost)}</div>
                <p>Total conversion taxes</p>
              </div>
            </div>

            {results.rothConversionAnalysis.isRecommended && (
              <div className="savings-highlight">
                <span className="savings-label">Estimated Savings:</span>
                <span className="savings-amount">{formatCurrency(results.rothConversionAnalysis.estimatedSavings)}</span>
              </div>
            )}

            <div className="strategy-explanation">
              <p>{results.rothConversionAnalysis.strategyExplanation}</p>
            </div>

            {results.rothConversionAnalysis.transitionWarning && (
              <div className="transition-warning">
                <h4>Transition Period</h4>
                <p>{results.rothConversionAnalysis.transitionWarning}</p>
              </div>
            )}

            {results.rothConversionAnalysis.conversionSchedule && results.rothConversionAnalysis.conversionSchedule.length > 0 && (
              <div className="conversion-schedule">
                <h4>Year-by-Year Conversion Schedule</h4>
                <div className="schedule-table">
                  <div className="schedule-header">
                    <span>Year</span>
                    <span>Age</span>
                    <span>Convert</span>
                    <span>Tax</span>
                    <span>Available</span>
                  </div>
                  {results.rothConversionAnalysis.conversionSchedule.slice(0, 10).map((year) => (
                    <div key={year.year} className="schedule-row">
                      <span>{year.year}</span>
                      <span>{year.age}</span>
                      <span>{formatCurrency(year.conversionAmount)}</span>
                      <span>{formatCurrency(year.conversionTax)}</span>
                      <span>{formatCurrency(year.availableForWithdrawal)}</span>
                    </div>
                  ))}
                  {results.rothConversionAnalysis.conversionSchedule.length > 10 && (
                    <div className="schedule-note">
                      Showing first 10 years of {results.rothConversionAnalysis.conversionSchedule.length} total years
                    </div>
                  )}
                </div>
              </div>
            )}
          </div>
        </div>
      )}

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
            {results.yearsWithPenalty > 0 && (
              <div className="tax-detail-row penalty-row">
                <span className="tax-label">Early Withdrawal Penalty (Annual):</span>
                <span className="tax-value">{formatCurrency(results.earlyWithdrawalPenalty / results.yearsWithPenalty)}</span>
              </div>
            )}
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

      <Visualization results={results} />

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
