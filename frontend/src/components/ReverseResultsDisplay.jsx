import PropTypes from 'prop-types';

const ReverseResultsDisplay = ({ results, onCalculateAgain }) => {
  if (!results || !results.scenarios || results.scenarios.length === 0) {
    return (
      <div className="results-container">
        <div className="error-message">
          <p>No results available. Please try again with different inputs.</p>
          <button onClick={onCalculateAgain} className="btn-secondary calculate-again-button">
            Calculate Again
          </button>
        </div>
      </div>
    );
  }

  const formatCurrency = (value) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(value);
  };

  const formatPercentage = (value) => {
    return `${value.toFixed(2)}%`;
  };

  const getRiskColor = (riskProfile) => {
    switch (riskProfile) {
      case 'Conservative':
        return '#4CAF50'; // Green
      case 'Moderate':
        return '#2196F3'; // Blue
      case 'Aggressive':
        return '#FF9800'; // Orange
      default:
        return '#666';
    }
  };

  return (
    <div className="results-container">
      <div className="results-header">
        <h2>Required Portfolio Analysis</h2>
        <button onClick={onCalculateAgain} className="btn-secondary calculate-again-button">
          Calculate Again
        </button>
      </div>

      {/* Summary Section */}
      <div className="summary-section">
        <h3>Summary</h3>
        <p className="summary-text">{results.summary}</p>
      </div>

      {/* Scenarios */}
      <div className="scenarios-section">
        <h3>Portfolio Requirements by Risk Profile</h3>
        <div className="scenarios-grid">
          {results.scenarios.map((scenario, index) => (
            <div
              key={index}
              className="scenario-card"
              style={{ borderColor: getRiskColor(scenario.riskProfile) }}
            >
              <div className="scenario-header" style={{ backgroundColor: getRiskColor(scenario.riskProfile) }}>
                <h4>{scenario.riskProfile}</h4>
                <div className="allocation-badge">
                  {scenario.stockAllocationPercent}% Stocks / {scenario.bondAllocationPercent}% Bonds
                </div>
              </div>

              <div className="scenario-body">
                {/* Required Portfolio */}
                <div className="metric-highlight">
                  <span className="metric-label">Required Portfolio</span>
                  <span className="metric-value-large">{formatCurrency(scenario.requiredPortfolioSize)}</span>
                </div>

                {/* Key Metrics */}
                <div className="metrics-grid">
                  <div className="metric">
                    <span className="metric-label">Withdrawal Rate</span>
                    <span className="metric-value">{formatPercentage(scenario.withdrawalRate)}</span>
                  </div>
                  <div className="metric">
                    <span className="metric-label">Success Rate</span>
                    <span className="metric-value">{formatPercentage(scenario.historicalSuccessRate)}</span>
                  </div>
                  <div className="metric">
                    <span className="metric-label">Annual Pre-Tax Withdrawal</span>
                    <span className="metric-value">{formatCurrency(scenario.annualPreTaxWithdrawal)}</span>
                  </div>
                  <div className="metric">
                    <span className="metric-label">Annual After-Tax Income</span>
                    <span className="metric-value">{formatCurrency(scenario.annualAfterTaxIncome)}</span>
                  </div>
                  <div className="metric">
                    <span className="metric-label">Annual Taxes</span>
                    <span className="metric-value">{formatCurrency(scenario.estimatedAnnualTaxes)}</span>
                  </div>
                  <div className="metric">
                    <span className="metric-label">Effective Tax Rate</span>
                    <span className="metric-value">{formatPercentage(scenario.effectiveTaxRate)}</span>
                  </div>
                </div>

                {/* Portfolio Outcomes */}
                <div className="outcomes-section">
                  <h5>Historical Outcomes</h5>
                  <div className="metrics-grid">
                    <div className="metric">
                      <span className="metric-label">Median Final Value</span>
                      <span className="metric-value">{formatCurrency(scenario.medianFinalPortfolioValue)}</span>
                    </div>
                    <div className="metric">
                      <span className="metric-label">Best Case</span>
                      <span className="metric-value">{formatCurrency(scenario.bestCaseScenario)}</span>
                    </div>
                    <div className="metric">
                      <span className="metric-label">Worst Case</span>
                      <span className="metric-value">{formatCurrency(scenario.worstCaseScenario)}</span>
                    </div>
                  </div>
                </div>

                {/* Early Retirement Warning */}
                {scenario.penaltyWarning && (
                  <div className="warning-box">
                    <strong>Early Retirement Alert</strong>
                    <p>{scenario.penaltyWarning}</p>
                    <p>Annual Penalty: {formatCurrency(scenario.earlyWithdrawalPenalty || 0)} for {scenario.yearsWithPenalty} years</p>
                  </div>
                )}

                {/* Roth Conversion Analysis */}
                {scenario.rothConversionAnalysis && scenario.rothConversionAnalysis.isRecommended && (
                  <div className="roth-recommendation">
                    <h5>Roth Conversion Ladder Recommended</h5>
                    <p>{scenario.rothConversionAnalysis.strategyExplanation}</p>
                    <div className="metrics-grid">
                      <div className="metric">
                        <span className="metric-label">Conversion Tax Cost</span>
                        <span className="metric-value">{formatCurrency(scenario.rothConversionAnalysis.totalConversionTaxCost)}</span>
                      </div>
                      <div className="metric">
                        <span className="metric-label">Penalty Cost (Alternative)</span>
                        <span className="metric-value">{formatCurrency(scenario.rothConversionAnalysis.totalPenaltyCost)}</span>
                      </div>
                      <div className="metric">
                        <span className="metric-label">Estimated Savings</span>
                        <span className="metric-value savings">{formatCurrency(scenario.rothConversionAnalysis.estimatedSavings)}</span>
                      </div>
                    </div>
                  </div>
                )}

                {/* Recommendation */}
                <div className="recommendation-box">
                  <p>{scenario.recommendation}</p>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Gap Analysis */}
      {results.gapAnalysis && (
        <div className="gap-analysis-section">
          <h3>Gap Analysis</h3>
          <div className="gap-summary">
            <div className="metric-highlight">
              <span className="metric-label">Current Total Savings</span>
              <span className="metric-value-large">{formatCurrency(results.gapAnalysis.currentTotalSavings)}</span>
            </div>
          </div>

          <div className="gap-grid">
            <div className="gap-card">
              <h4 style={{ color: getRiskColor('Conservative') }}>Conservative Gap</h4>
              <div className="gap-amount">
                {results.gapAnalysis.gapConservative > 0 ? (
                  <span className="gap-negative">{formatCurrency(results.gapAnalysis.gapConservative)}</span>
                ) : (
                  <span className="gap-positive">Fully Funded!</span>
                )}
              </div>
              <p className="gap-target">Target: {formatCurrency(results.gapAnalysis.requiredAmountConservative)}</p>
            </div>

            <div className="gap-card">
              <h4 style={{ color: getRiskColor('Moderate') }}>Moderate Gap</h4>
              <div className="gap-amount">
                {results.gapAnalysis.gapModerate > 0 ? (
                  <span className="gap-negative">{formatCurrency(results.gapAnalysis.gapModerate)}</span>
                ) : (
                  <span className="gap-positive">Fully Funded!</span>
                )}
              </div>
              <p className="gap-target">Target: {formatCurrency(results.gapAnalysis.requiredAmountModerate)}</p>
            </div>

            <div className="gap-card">
              <h4 style={{ color: getRiskColor('Aggressive') }}>Aggressive Gap</h4>
              <div className="gap-amount">
                {results.gapAnalysis.gapAggressive > 0 ? (
                  <span className="gap-negative">{formatCurrency(results.gapAnalysis.gapAggressive)}</span>
                ) : (
                  <span className="gap-positive">Fully Funded!</span>
                )}
              </div>
              <p className="gap-target">Target: {formatCurrency(results.gapAnalysis.requiredAmountAggressive)}</p>
            </div>
          </div>

          {/* Savings Roadmap */}
          {results.gapAnalysis.savingsRoadmap && (
            <div className="savings-roadmap">
              <h4>Savings Roadmap</h4>
              <p className="roadmap-intro">
                Years until retirement: {results.gapAnalysis.savingsRoadmap.yearsUntilRetirement}
              </p>
              <p className="roadmap-intro">
                Current annual savings: {formatCurrency(results.gapAnalysis.savingsRoadmap.annualSavingsAmount)}
              </p>

              <div className="roadmap-grid">
                {results.gapAnalysis.savingsRoadmap.yearsToReachConservativeGoal !== null && (
                  <div className="roadmap-item">
                    <h5 style={{ color: getRiskColor('Conservative') }}>Conservative Goal</h5>
                    {results.gapAnalysis.savingsRoadmap.yearsToReachConservativeGoal <= results.gapAnalysis.savingsRoadmap.yearsUntilRetirement ? (
                      <p className="roadmap-success">
                        Reach goal in {results.gapAnalysis.savingsRoadmap.yearsToReachConservativeGoal} years
                      </p>
                    ) : (
                      <div>
                        <p className="roadmap-warning">
                          Would take {results.gapAnalysis.savingsRoadmap.yearsToReachConservativeGoal} years
                        </p>
                        {results.gapAnalysis.savingsRoadmap.requiredMonthlySavingsConservative && (
                          <p className="roadmap-action">
                            Save {formatCurrency(results.gapAnalysis.savingsRoadmap.requiredMonthlySavingsConservative)}/month to reach by retirement
                          </p>
                        )}
                      </div>
                    )}
                  </div>
                )}

                {results.gapAnalysis.savingsRoadmap.yearsToReachModerateGoal !== null && (
                  <div className="roadmap-item">
                    <h5 style={{ color: getRiskColor('Moderate') }}>Moderate Goal</h5>
                    {results.gapAnalysis.savingsRoadmap.yearsToReachModerateGoal <= results.gapAnalysis.savingsRoadmap.yearsUntilRetirement ? (
                      <p className="roadmap-success">
                        Reach goal in {results.gapAnalysis.savingsRoadmap.yearsToReachModerateGoal} years
                      </p>
                    ) : (
                      <div>
                        <p className="roadmap-warning">
                          Would take {results.gapAnalysis.savingsRoadmap.yearsToReachModerateGoal} years
                        </p>
                        {results.gapAnalysis.savingsRoadmap.requiredMonthlySavingsModerate && (
                          <p className="roadmap-action">
                            Save {formatCurrency(results.gapAnalysis.savingsRoadmap.requiredMonthlySavingsModerate)}/month to reach by retirement
                          </p>
                        )}
                      </div>
                    )}
                  </div>
                )}

                {results.gapAnalysis.savingsRoadmap.yearsToReachAggressiveGoal !== null && (
                  <div className="roadmap-item">
                    <h5 style={{ color: getRiskColor('Aggressive') }}>Aggressive Goal</h5>
                    {results.gapAnalysis.savingsRoadmap.yearsToReachAggressiveGoal <= results.gapAnalysis.savingsRoadmap.yearsUntilRetirement ? (
                      <p className="roadmap-success">
                        Reach goal in {results.gapAnalysis.savingsRoadmap.yearsToReachAggressiveGoal} years
                      </p>
                    ) : (
                      <div>
                        <p className="roadmap-warning">
                          Would take {results.gapAnalysis.savingsRoadmap.yearsToReachAggressiveGoal} years
                        </p>
                        {results.gapAnalysis.savingsRoadmap.requiredMonthlySavingsAggressive && (
                          <p className="roadmap-action">
                            Save {formatCurrency(results.gapAnalysis.savingsRoadmap.requiredMonthlySavingsAggressive)}/month to reach by retirement
                          </p>
                        )}
                      </div>
                    )}
                  </div>
                )}
              </div>

              <div className="roadmap-recommendation">
                <strong>Recommendation:</strong> {results.gapAnalysis.savingsRoadmap.recommendation}
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

ReverseResultsDisplay.propTypes = {
  results: PropTypes.shape({
    scenarios: PropTypes.arrayOf(PropTypes.shape({
      riskProfile: PropTypes.string.isRequired,
      requiredPortfolioSize: PropTypes.number.isRequired,
      withdrawalRate: PropTypes.number.isRequired,
      annualPreTaxWithdrawal: PropTypes.number.isRequired,
      annualAfterTaxIncome: PropTypes.number.isRequired,
      estimatedAnnualTaxes: PropTypes.number.isRequired,
      effectiveTaxRate: PropTypes.number.isRequired,
      historicalSuccessRate: PropTypes.number.isRequired,
      stockAllocationPercent: PropTypes.number.isRequired,
      bondAllocationPercent: PropTypes.number.isRequired,
      medianFinalPortfolioValue: PropTypes.number.isRequired,
      worstCaseScenario: PropTypes.number.isRequired,
      bestCaseScenario: PropTypes.number.isRequired,
      earlyWithdrawalPenalty: PropTypes.number,
      yearsWithPenalty: PropTypes.number,
      penaltyWarning: PropTypes.string,
      rothConversionAnalysis: PropTypes.object,
      recommendation: PropTypes.string.isRequired
    })).isRequired,
    gapAnalysis: PropTypes.object,
    summary: PropTypes.string.isRequired
  }),
  onCalculateAgain: PropTypes.func.isRequired
};

export default ReverseResultsDisplay;
