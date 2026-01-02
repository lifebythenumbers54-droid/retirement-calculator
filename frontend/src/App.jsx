import { useState } from 'react'
import './App.css'

function App() {
  const [isCalculated, setIsCalculated] = useState(false)

  return (
    <div className="app-container">
      <header className="app-header">
        <h1>Retirement Calculator</h1>
        <p className="subtitle">
          Calculate safe withdrawal strategies based on historical market data
        </p>
      </header>

      <main className="app-main">
        <div className="content-wrapper">
          {!isCalculated ? (
            <div className="placeholder-section">
              <h2>Input Form</h2>
              <p className="placeholder-text">
                The input form component will be added here in the next phase.
              </p>
              <div className="placeholder-box">
                <p>Form fields will include:</p>
                <ul>
                  <li>Current age</li>
                  <li>Retirement age</li>
                  <li>Retirement account balance (401k/IRA)</li>
                  <li>Taxable account balance</li>
                  <li>Success rate threshold</li>
                </ul>
              </div>
            </div>
          ) : (
            <div className="placeholder-section">
              <h2>Results Display</h2>
              <p className="placeholder-text">
                Calculation results and visualizations will be displayed here.
              </p>
            </div>
          )}
        </div>
      </main>

      <footer className="app-footer">
        <p className="disclaimer">
          This calculator provides estimates based on historical data and should not be
          considered financial advice. Consult with a qualified financial advisor for
          personalized guidance.
        </p>
      </footer>
    </div>
  )
}

export default App
