import { useState } from 'react'
import './App.css'
import InputForm from './components/InputForm'
import ResultsDisplay from './components/ResultsDisplay'
import AllocationAnalysis from './components/AllocationAnalysis'
import ReverseInputForm from './components/ReverseInputForm'
import ReverseResultsDisplay from './components/ReverseResultsDisplay'
import ErrorBoundary from './components/ErrorBoundary'

function App() {
  const [calculatorMode, setCalculatorMode] = useState('standard') // 'standard' or 'reverse'
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

  const handleModeToggle = (mode) => {
    setCalculatorMode(mode)
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
            {calculatorMode === 'standard'
              ? 'Calculate safe withdrawal strategies based on historical market data'
              : 'Calculate required portfolio size from your desired retirement income'
            }
          </p>

          {/* Calculator Mode Toggle */}
          <div className="mode-toggle">
            <button
              className={`mode-button ${calculatorMode === 'standard' ? 'active' : ''}`}
              onClick={() => handleModeToggle('standard')}
            >
              Standard Calculator
              <span className="mode-description">I have $X, what can I withdraw?</span>
            </button>
            <button
              className={`mode-button ${calculatorMode === 'reverse' ? 'active' : ''}`}
              onClick={() => handleModeToggle('reverse')}
            >
              Goal-Based Planning
              <span className="mode-description">I want $Y per year, how much do I need?</span>
            </button>
          </div>
        </header>

        <main className="app-main">
          <div className="content-wrapper">
            {calculatorMode === 'standard' ? (
              // Standard Calculator Flow
              !isCalculated ? (
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
              )
            ) : (
              // Reverse Calculator Flow
              !isCalculated ? (
                <ReverseInputForm onCalculationComplete={handleCalculationComplete} />
              ) : (
                <ReverseResultsDisplay
                  results={calculationResults}
                  onCalculateAgain={handleCalculateAgain}
                />
              )
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
