import { useState } from 'react'
import './App.css'
import InputForm from './components/InputForm'
import ResultsDisplay from './components/ResultsDisplay'

function App() {
  const [isCalculated, setIsCalculated] = useState(false)
  const [calculationResults, setCalculationResults] = useState(null)

  const handleCalculationComplete = (results) => {
    setCalculationResults(results)
    setIsCalculated(true)
  }

  const handleCalculateAgain = () => {
    setIsCalculated(false)
    setCalculationResults(null)
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
            <ResultsDisplay
              results={calculationResults}
              onCalculateAgain={handleCalculateAgain}
            />
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
