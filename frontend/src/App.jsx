import { useState } from 'react'
import './App.css'
import InputForm from './components/InputForm'

function App() {
  const [isCalculated, setIsCalculated] = useState(false)
  const [calculationResults, setCalculationResults] = useState(null)

  const handleCalculationComplete = (results) => {
    setCalculationResults(results)
    setIsCalculated(true)
  }

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
            <InputForm onCalculationComplete={handleCalculationComplete} />
          ) : (
            <div className="placeholder-section">
              <h2>Results Display</h2>
              <p className="placeholder-text">
                Calculation results and visualizations will be displayed here.
              </p>
              <button
                onClick={() => setIsCalculated(false)}
                style={{
                  padding: '0.75rem 1.5rem',
                  fontSize: '1rem',
                  background: '#646cff',
                  color: 'white',
                  border: 'none',
                  borderRadius: '6px',
                  cursor: 'pointer',
                  marginTop: '1rem'
                }}
              >
                Calculate Again
              </button>
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
