import { useState } from 'react'
import { calculateRetirement } from '../services/apiClient'
import './InputForm.css'

function InputForm({ onCalculationComplete }) {
  const [formData, setFormData] = useState({
    currentAge: '',
    retirementAge: '',
    retirementAccountBalance: '',
    taxableAccountBalance: '',
    successRateThreshold: '0.95'
  })

  const [errors, setErrors] = useState({})
  const [isLoading, setIsLoading] = useState(false)
  const [apiError, setApiError] = useState(null)

  const validateField = (name, value) => {
    switch (name) {
      case 'currentAge': {
        const age = parseInt(value)
        if (!value) return 'Current age is required'
        if (isNaN(age)) return 'Please enter a valid number'
        if (age < 18 || age > 100) return 'Age must be between 18 and 100'
        return ''
      }
      case 'retirementAge': {
        const retAge = parseInt(value)
        const currAge = parseInt(formData.currentAge)
        if (!value) return 'Retirement age is required'
        if (isNaN(retAge)) return 'Please enter a valid number'
        if (retAge < 18 || retAge > 100) return 'Age must be between 18 and 100'
        if (currAge && retAge < currAge) return 'Retirement age must be greater than or equal to current age'
        return ''
      }
      case 'retirementAccountBalance': {
        const balance = parseFloat(value)
        if (!value) return 'Retirement account balance is required'
        if (isNaN(balance)) return 'Please enter a valid number'
        if (balance < 0) return 'Balance must be a positive number'
        return ''
      }
      case 'taxableAccountBalance': {
        const balance = parseFloat(value)
        if (!value) return 'Taxable account balance is required'
        if (isNaN(balance)) return 'Please enter a valid number'
        if (balance < 0) return 'Balance must be a positive number'
        return ''
      }
      case 'successRateThreshold': {
        if (!value) return 'Success rate threshold is required'
        return ''
      }
      default:
        return ''
    }
  }

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({ ...prev, [name]: value }))

    // Real-time validation
    const error = validateField(name, value)
    setErrors(prev => ({ ...prev, [name]: error }))

    // Re-validate retirement age when current age changes
    if (name === 'currentAge' && formData.retirementAge) {
      const retAgeError = validateField('retirementAge', formData.retirementAge)
      setErrors(prev => ({ ...prev, retirementAge: retAgeError }))
    }
  }

  const validateForm = () => {
    const newErrors = {}
    Object.keys(formData).forEach(key => {
      const error = validateField(key, formData[key])
      if (error) newErrors[key] = error
    })
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const formatCurrency = (value) => {
    if (!value) return ''
    const num = parseFloat(value.replace(/,/g, ''))
    if (isNaN(num)) return value
    return num.toLocaleString('en-US', { maximumFractionDigits: 2 })
  }

  const handleCurrencyChange = (e) => {
    const { name, value } = e.target
    const numericValue = value.replace(/,/g, '')
    setFormData(prev => ({ ...prev, [name]: numericValue }))

    const error = validateField(name, numericValue)
    setErrors(prev => ({ ...prev, [name]: error }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setApiError(null)

    if (!validateForm()) {
      return
    }

    setIsLoading(true)

    try {
      const payload = {
        currentAge: parseInt(formData.currentAge),
        retirementAge: parseInt(formData.retirementAge),
        retirementAccountBalance: parseFloat(formData.retirementAccountBalance),
        taxableAccountBalance: parseFloat(formData.taxableAccountBalance),
        successRateThreshold: parseFloat(formData.successRateThreshold)
      }

      const result = await calculateRetirement(payload)
      onCalculationComplete(result, payload)
    } catch (error) {
      console.error('Calculation error:', error)
      setApiError(
        error.response?.data?.message ||
        error.message ||
        'An error occurred while calculating. Please try again.'
      )
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="input-form-container">
      <h2>Enter Your Information</h2>
      <form onSubmit={handleSubmit} className="input-form">
        <div className="form-group">
          <label htmlFor="currentAge">
            Current Age
            <span className="tooltip" title="Your age today (between 18 and 100)">?</span>
          </label>
          <input
            type="number"
            id="currentAge"
            name="currentAge"
            value={formData.currentAge}
            onChange={handleChange}
            className={errors.currentAge ? 'error' : ''}
            min="18"
            max="100"
            placeholder="e.g., 35"
          />
          {errors.currentAge && <span className="error-message">{errors.currentAge}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="retirementAge">
            Retirement Age
            <span className="tooltip" title="The age at which you plan to retire (must be at least your current age)">?</span>
          </label>
          <input
            type="number"
            id="retirementAge"
            name="retirementAge"
            value={formData.retirementAge}
            onChange={handleChange}
            className={errors.retirementAge ? 'error' : ''}
            min="18"
            max="100"
            placeholder="e.g., 65"
          />
          {errors.retirementAge && <span className="error-message">{errors.retirementAge}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="retirementAccountBalance">
            Retirement Account Balance (401k/IRA)
            <span className="tooltip" title="Total balance in your tax-deferred retirement accounts (401k, Traditional IRA, etc.)">?</span>
          </label>
          <div className="currency-input">
            <span className="currency-symbol">$</span>
            <input
              type="text"
              id="retirementAccountBalance"
              name="retirementAccountBalance"
              value={formatCurrency(formData.retirementAccountBalance)}
              onChange={handleCurrencyChange}
              className={errors.retirementAccountBalance ? 'error' : ''}
              placeholder="e.g., 500,000"
            />
          </div>
          {errors.retirementAccountBalance && <span className="error-message">{errors.retirementAccountBalance}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="taxableAccountBalance">
            Taxable Account Balance
            <span className="tooltip" title="Total balance in your taxable brokerage accounts and savings">?</span>
          </label>
          <div className="currency-input">
            <span className="currency-symbol">$</span>
            <input
              type="text"
              id="taxableAccountBalance"
              name="taxableAccountBalance"
              value={formatCurrency(formData.taxableAccountBalance)}
              onChange={handleCurrencyChange}
              className={errors.taxableAccountBalance ? 'error' : ''}
              placeholder="e.g., 250,000"
            />
          </div>
          {errors.taxableAccountBalance && <span className="error-message">{errors.taxableAccountBalance}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="successRateThreshold">
            Success Rate Threshold
            <span className="tooltip" title="The minimum probability of not running out of money. Higher values are more conservative.">?</span>
          </label>
          <select
            id="successRateThreshold"
            name="successRateThreshold"
            value={formData.successRateThreshold}
            onChange={handleChange}
            className={errors.successRateThreshold ? 'error' : ''}
          >
            <option value="0.90">90% - Moderate</option>
            <option value="0.95">95% - Conservative</option>
            <option value="0.98">98% - Very Conservative</option>
          </select>
          {errors.successRateThreshold && <span className="error-message">{errors.successRateThreshold}</span>}
        </div>

        {apiError && (
          <div className="api-error">
            <strong>Error:</strong> {apiError}
          </div>
        )}

        <button type="submit" className="submit-button" disabled={isLoading}>
          {isLoading ? (
            <>
              <span className="spinner"></span>
              Calculating...
            </>
          ) : (
            'Calculate Retirement Strategy'
          )}
        </button>
      </form>
    </div>
  )
}

export default InputForm
