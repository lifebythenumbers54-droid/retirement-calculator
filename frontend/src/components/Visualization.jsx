import { memo, useMemo } from 'react'
import { PieChart, Pie, Cell, ResponsiveContainer, Legend, Tooltip, Label } from 'recharts'
import './Visualization.css'

/**
 * Visualization component for displaying retirement calculation results
 * Uses Recharts library for interactive data visualizations
 * Optimized with React.memo and useMemo for better performance
 *
 * @param {Object} props - Component props
 * @param {Object} props.results - Calculation results from the API
 */
const Visualization = memo(function Visualization({ results }) {
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

  // Memoize expensive calculations
  const successRateData = useMemo(() => {
    const rate = results.achievedSuccessRate
    return [
      { name: 'Success', value: rate },
      { name: 'Risk', value: 100 - rate }
    ]
  }, [results.achievedSuccessRate])

  const successRateColor = useMemo(() => {
    const rate = results.achievedSuccessRate
    if (rate >= 95) return '#22c55e' // green
    if (rate >= 90) return '#eab308' // yellow
    return '#ef4444' // red
  }, [results.achievedSuccessRate])

  const successRateLabel = useMemo(() => {
    const rate = results.achievedSuccessRate
    if (rate >= 95) return 'Excellent'
    if (rate >= 90) return 'Good'
    return 'Moderate'
  }, [results.achievedSuccessRate])

  // Tax breakdown pie chart data
  const taxBreakdownData = useMemo(() => {
    const data = [
      {
        name: 'Net Income',
        value: results.netAnnualIncome,
        color: '#3b82f6'
      },
      {
        name: 'Ordinary Income Tax',
        value: results.ordinaryIncomeTax,
        color: '#f97316'
      },
      {
        name: 'Capital Gains Tax',
        value: results.capitalGainsTax,
        color: '#ec4899'
      }
    ]

    // Add early withdrawal penalty if applicable
    if (results.yearsWithPenalty > 0) {
      const annualPenalty = results.earlyWithdrawalPenalty / results.yearsWithPenalty
      data.push({
        name: 'Early Withdrawal Penalty',
        value: annualPenalty,
        color: '#ef4444'
      })
    }

    return data
  }, [results.netAnnualIncome, results.ordinaryIncomeTax, results.capitalGainsTax,
      results.yearsWithPenalty, results.earlyWithdrawalPenalty])

  // Withdrawal sources pie chart data
  const withdrawalSourcesData = useMemo(() => [
    {
      name: 'Tax-Deferred Accounts',
      value: results.taxDeferredAccountWithdrawal,
      color: '#8b5cf6'
    },
    {
      name: 'Taxable Accounts',
      value: results.taxableAccountWithdrawal,
      color: '#06b6d4'
    }
  ], [results.taxDeferredAccountWithdrawal, results.taxableAccountWithdrawal])

  // Custom tooltip for charts
  const CustomTooltip = ({ active, payload }) => {
    if (active && payload && payload.length) {
      return (
        <div className="chart-tooltip">
          <p className="tooltip-label">{payload[0].name}</p>
          <p className="tooltip-value">{formatCurrency(payload[0].value)}</p>
        </div>
      )
    }
    return null
  }

  // Custom label for pie charts
  const renderCustomLabel = ({ cx, cy, midAngle, innerRadius, outerRadius, percent }) => {
    if (percent < 0.05) return null // Don't show label for small slices
    const radius = innerRadius + (outerRadius - innerRadius) * 0.5
    const x = cx + radius * Math.cos(-midAngle * Math.PI / 180)
    const y = cy + radius * Math.sin(-midAngle * Math.PI / 180)

    return (
      <text
        x={x}
        y={y}
        fill="white"
        textAnchor={x > cx ? 'start' : 'end'}
        dominantBaseline="central"
        fontSize="14"
        fontWeight="bold"
      >
        {`${(percent * 100).toFixed(0)}%`}
      </text>
    )
  }

  return (
    <div className="visualization-container">
      <h2 className="visualization-title">Visual Analysis</h2>

      <div className="charts-grid">
        {/* Success Rate Gauge */}
        <div className="chart-card">
          <h3 className="chart-title">Success Rate</h3>
          <div className="gauge-container">
            <ResponsiveContainer width="100%" height={250}>
              <PieChart>
                <Pie
                  data={successRateData}
                  cx="50%"
                  cy="50%"
                  startAngle={180}
                  endAngle={0}
                  innerRadius={60}
                  outerRadius={100}
                  dataKey="value"
                  stroke="none"
                >
                  <Cell fill={successRateColor} />
                  <Cell fill="#e5e7eb" />
                  <Label
                    value={formatPercentage(results.achievedSuccessRate)}
                    position="center"
                    style={{ fontSize: '32px', fontWeight: 'bold', fill: successRateColor }}
                  />
                </Pie>
              </PieChart>
            </ResponsiveContainer>
            <div className="gauge-label" style={{ color: successRateColor }}>
              {successRateLabel} Probability
            </div>
            <p className="chart-description">
              Based on {results.numberOfScenariosSimulated.toLocaleString()} historical scenarios
            </p>
          </div>
        </div>

        {/* Tax Breakdown Pie Chart */}
        <div className="chart-card">
          <h3 className="chart-title">Annual Income & Tax Breakdown</h3>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={taxBreakdownData}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={renderCustomLabel}
                outerRadius={100}
                dataKey="value"
              >
                {taxBreakdownData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={entry.color} />
                ))}
              </Pie>
              <Tooltip content={<CustomTooltip />} />
              <Legend
                verticalAlign="bottom"
                height={36}
                formatter={(value, entry) => (
                  <span style={{ color: '#374151', fontSize: '14px' }}>
                    {value}: {formatCurrency(entry.payload.value)}
                  </span>
                )}
              />
            </PieChart>
          </ResponsiveContainer>
          <p className="chart-description">
            Total Annual Withdrawal: {formatCurrency(results.annualGrossWithdrawal)}
          </p>
        </div>

        {/* Withdrawal Sources Pie Chart */}
        <div className="chart-card">
          <h3 className="chart-title">Withdrawal Sources</h3>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={withdrawalSourcesData}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={renderCustomLabel}
                outerRadius={100}
                dataKey="value"
              >
                {withdrawalSourcesData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={entry.color} />
                ))}
              </Pie>
              <Tooltip content={<CustomTooltip />} />
              <Legend
                verticalAlign="bottom"
                height={36}
                formatter={(value, entry) => (
                  <span style={{ color: '#374151', fontSize: '14px' }}>
                    {value}: {formatCurrency(entry.payload.value)}
                  </span>
                )}
              />
            </PieChart>
          </ResponsiveContainer>
          <p className="chart-description">
            Tax-optimized withdrawal strategy across account types
          </p>
        </div>

        {/* Key Metrics Summary */}
        <div className="chart-card metrics-summary">
          <h3 className="chart-title">Key Metrics</h3>
          <div className="metrics-grid">
            <div className="metric-item">
              <div className="metric-label">Withdrawal Rate</div>
              <div className="metric-value" style={{ color: '#3b82f6' }}>
                {formatPercentage(results.withdrawalRate)}
              </div>
            </div>
            <div className="metric-item">
              <div className="metric-label">Effective Tax Rate</div>
              <div className="metric-value" style={{ color: '#f97316' }}>
                {formatPercentage(results.effectiveTaxRate)}
              </div>
            </div>
            <div className="metric-item">
              <div className="metric-label">Monthly Net Income</div>
              <div className="metric-value" style={{ color: '#22c55e' }}>
                {formatCurrency(results.netAnnualIncome / 12)}
              </div>
            </div>
            <div className="metric-item">
              <div className="metric-label">Success Rate</div>
              <div className="metric-value" style={{ color: successRateColor }}>
                {formatPercentage(results.achievedSuccessRate)}
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="visualization-notes">
        <h4>Understanding Your Results</h4>
        <ul>
          <li>
            <strong>Success Rate:</strong> The percentage of historical scenarios where your portfolio
            lasted the entire retirement period without running out of money.
          </li>
          <li>
            <strong>Tax Breakdown:</strong> Shows how your annual withdrawal is split between net income
            and various taxes, helping you understand your true take-home amount.
          </li>
          <li>
            <strong>Withdrawal Sources:</strong> Illustrates the tax-optimized strategy of withdrawing
            from different account types to minimize your tax burden.
          </li>
        </ul>
      </div>
    </div>
  )
})

export default Visualization
