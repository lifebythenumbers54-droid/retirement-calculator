import { useState } from 'react'
import { analyzeAllocations } from '../services/apiClient'
import './AllocationAnalysis.css'

function AllocationAnalysis({ inputData, onBack }) {
  const [analysisResults, setAnalysisResults] = useState(null)
  const [isLoading, setIsLoading] = useState(false)
  const [apiError, setApiError] = useState(null)
  const [selectedStrategy, setSelectedStrategy] = useState(null)

  const formatCurrency = (value) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(value)
  }

  const formatPercentage = (value) => {
    return `${value.toFixed(1)}%`
  }

  const handleAnalyze = async () => {
    setApiError(null)
    setIsLoading(true)

    try {
      const result = await analyzeAllocations(inputData)
      setAnalysisResults(result)
    } catch (error) {
      console.error('Allocation analysis error:', error)
      setApiError(error.message || 'An error occurred while analyzing allocations. Please try again.')
    } finally {
      setIsLoading(false)
    }
  }

  const renderStrategyCard = (strategy, type) => {
    const isSelected = selectedStrategy === type
    const typeColors = {
      conservative: '#10b981',
      balanced: '#3b82f6',
      aggressive: '#f59e0b'
    }

    return (
      <div
        className={`strategy-card ${type} ${isSelected ? 'selected' : ''}`}
        onClick={() => setSelectedStrategy(type)}
      >
        <div className="strategy-header">
          <h3>{strategy.name}</h3>
          <div className="allocation-badge" style={{ backgroundColor: typeColors[type] }}>
            {formatPercentage(strategy.stockAllocation)} Stocks / {formatPercentage(strategy.bondAllocation)} Bonds
          </div>
        </div>

        <div className="strategy-metrics">
          <div className="metric-row primary">
            <span className="metric-label">Historical Success Rate</span>
            <span className="metric-value success-rate">{formatPercentage(strategy.historicalSuccessRate)}</span>
          </div>

          <div className="metric-row">
            <span className="metric-label">Recommended Withdrawal Rate</span>
            <span className="metric-value">{formatPercentage(strategy.recommendedWithdrawalRate)}</span>
          </div>

          <div className="metric-row">
            <span className="metric-label">Expected Annual Withdrawal</span>
            <span className="metric-value">{formatCurrency(strategy.expectedAnnualWithdrawal)}</span>
          </div>

          <div className="metric-row">
            <span className="metric-label">Expected Net Income (After Tax)</span>
            <span className="metric-value">{formatCurrency(strategy.expectedNetIncome)}</span>
          </div>

          <div className="metric-row">
            <span className="metric-label">Portfolio Volatility</span>
            <span className="metric-value">{formatPercentage(strategy.averageVolatility)}</span>
          </div>
        </div>

        <div className="strategy-outcomes">
          <h4>Historical Performance</h4>
          <div className="outcomes-grid">
            <div className="outcome">
              <span className="outcome-label">Best Case</span>
              <span className="outcome-value">{formatCurrency(strategy.bestCaseFinalValue)}</span>
            </div>
            <div className="outcome">
              <span className="outcome-label">Typical Case</span>
              <span className="outcome-value">{formatCurrency(strategy.medianFinalValue)}</span>
            </div>
            <div className="outcome">
              <span className="outcome-label">Worst Case</span>
              <span className="outcome-value">{formatCurrency(strategy.worstCaseFinalValue)}</span>
            </div>
          </div>
          <div className="scenario-count">
            Tested across {strategy.successfulScenarios} successful / {strategy.totalScenarios} total historical periods
          </div>
        </div>

        <div className="strategy-description">
          <p>{strategy.description}</p>
        </div>

        {strategy.failurePeriods && strategy.failurePeriods.length > 0 && (
          <details className="failure-periods">
            <summary>Historical Failure Periods ({strategy.failurePeriods.length})</summary>
            <div className="failure-list">
              {strategy.failurePeriods.slice(0, 10).map((period, idx) => (
                <span key={idx} className="failure-period">{period}</span>
              ))}
              {strategy.failurePeriods.length > 10 && (
                <span className="more-periods">+{strategy.failurePeriods.length - 10} more</span>
              )}
            </div>
          </details>
        )}
      </div>
    )
  }

  if (!analysisResults) {
    return (
      <div className="allocation-analysis-container">
        <div className="analysis-intro">
          <h2>Dynamic Portfolio Allocation Analysis</h2>
          <p className="intro-text">
            Instead of using a fixed 60/40 stock/bond allocation, let's analyze which allocation strategy
            has historically performed best for your specific retirement scenario.
          </p>

          <div className="analysis-details">
            <h3>What This Analysis Does</h3>
            <ul>
              <li>Tests 8 different stock/bond allocations (from 30% to 100% stocks)</li>
              <li>Simulates your retirement across all historical periods since 1925</li>
              <li>Identifies the optimal withdrawal rate for each allocation</li>
              <li>Ranks strategies by success rate, final portfolio value, and volatility</li>
              <li>Presents 3 strategies: Conservative, Balanced, and Aggressive</li>
            </ul>

            <h3>Your Scenario</h3>
            <div className="scenario-summary">
              <div className="scenario-item">
                <span className="scenario-label">Retirement Duration:</span>
                <span className="scenario-value">{analysisResults?.retirementDuration || (95 - inputData.retirementAge)} years</span>
              </div>
              <div className="scenario-item">
                <span className="scenario-label">Total Portfolio:</span>
                <span className="scenario-value">{formatCurrency(inputData.retirementAccountBalance + inputData.taxableAccountBalance)}</span>
              </div>
              <div className="scenario-item">
                <span className="scenario-label">Success Rate Target:</span>
                <span className="scenario-value">{formatPercentage(inputData.successRateThreshold * 100)}</span>
              </div>
            </div>
          </div>

          {apiError && (
            <div className="api-error">
              <strong>Error:</strong> {apiError}
            </div>
          )}

          <div className="analysis-actions">
            <button onClick={onBack} className="back-button">
              Back to Results
            </button>
            <button
              onClick={handleAnalyze}
              className="analyze-button"
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <span className="spinner"></span>
                  Analyzing Allocations...
                </>
              ) : (
                'Analyze Optimal Allocations'
              )}
            </button>
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="allocation-analysis-container">
      <div className="analysis-header">
        <h2>Portfolio Allocation Recommendations</h2>
        <p className="subtitle">
          Based on {analysisResults.allocationsAnalyzed} allocations tested across historical data
        </p>
      </div>

      <div className="methodology-note">
        <details>
          <summary>Methodology</summary>
          <p>{analysisResults.methodology}</p>
        </details>
      </div>

      <div className="strategies-grid">
        {renderStrategyCard(analysisResults.conservative, 'conservative')}
        {renderStrategyCard(analysisResults.balanced, 'balanced')}
        {renderStrategyCard(analysisResults.aggressive, 'aggressive')}
      </div>

      <div className="comparison-section">
        <h3>Strategy Comparison</h3>
        <table className="comparison-table">
          <thead>
            <tr>
              <th>Strategy</th>
              <th>Allocation</th>
              <th>Success Rate</th>
              <th>Withdrawal Rate</th>
              <th>Annual Income (Net)</th>
              <th>Volatility</th>
            </tr>
          </thead>
          <tbody>
            {[
              { name: 'Conservative', data: analysisResults.conservative },
              { name: 'Balanced', data: analysisResults.balanced },
              { name: 'Aggressive', data: analysisResults.aggressive }
            ].map(({ name, data }) => (
              <tr key={name} className={selectedStrategy === name.toLowerCase() ? 'selected-row' : ''}>
                <td className="strategy-name">{name}</td>
                <td>{formatPercentage(data.stockAllocation)} / {formatPercentage(data.bondAllocation)}</td>
                <td className="success-rate">{formatPercentage(data.historicalSuccessRate)}</td>
                <td>{formatPercentage(data.recommendedWithdrawalRate)}</td>
                <td>{formatCurrency(data.expectedNetIncome)}</td>
                <td>{formatPercentage(data.averageVolatility)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="analysis-actions bottom-actions">
        <button onClick={onBack} className="back-button">
          Back to Results
        </button>
        <button
          onClick={handleAnalyze}
          className="reanalyze-button"
          disabled={isLoading}
        >
          Re-analyze
        </button>
      </div>
    </div>
  )
}

export default AllocationAnalysis
