import { useState } from 'react'
import './App.css'
import InputForm from './components/InputForm'
import ResultsDisplay from './components/ResultsDisplay'
import AllocationAnalysis from './components/AllocationAnalysis'
import ErrorBoundary from './components/ErrorBoundary'

function App() {
  const [isCalculated, setIsCalculated] = useState(false)
  const [calculationResults, setCalculationResults] = useState(null)
  const [showAllocationAnalysis, setShowAllocationAnalysis] = useState(false)
  const [inputData, setInputData] = useState(null)

  const handleCalculationComplete = (results, input) => {
    setCalculationResults(results)
    setInputData(input)
    setIsCalculated(true)
    setShowAllocationAnalysis(false)
  }

  const handleCalculateAgain = () => {
    setIsCalculated(false)
    setCalculationResults(null)
    setInputData(null)
    setShowAllocationAnalysis(false)
  }

  const handleShowAllocationAnalysis = () => {
    setShowAllocationAnalysis(true)
  }

  const handleBackToResults = () => {
    setShowAllocationAnalysis(false)
  }

  return (
    <ErrorBoundary>
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
            ) : showAllocationAnalysis ? (
              <AllocationAnalysis
                inputData={inputData}
                onBack={handleBackToResults}
              />
            ) : (
              <ResultsDisplay
                results={calculationResults}
                onCalculateAgain={handleCalculateAgain}
                onShowAllocationAnalysis={handleShowAllocationAnalysis}
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
    </ErrorBoundary>
  )
}

export default App
