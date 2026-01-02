import { useState } from 'react';
import PropTypes from 'prop-types';
import { calculateReverseRetirement } from '../services/apiClient';

const ReverseInputForm = ({ onCalculationComplete }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [apiError, setApiError] = useState(null);
  const [formData, setFormData] = useState({
    desiredAfterTaxIncome: '60000',
    currentAge: '30',
    retirementAge: '65',
    successRateThreshold: '0.90',
    currentRetirementAccountBalance: '',
    currentTaxableAccountBalance: '',
    annualSavings: ''
  });

  const [errors, setErrors] = useState({});

  const formatCurrency = (value) => {
    const number = parseFloat(value.replace(/[^0-9.-]+/g, ''));
    if (isNaN(number)) return '';
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(number);
  };

  const parseCurrency = (formattedValue) => {
    const number = parseFloat(formattedValue.replace(/[^0-9.-]+/g, ''));
    return isNaN(number) ? '' : number.toString();
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));

    // Clear error for this field
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }
  };

  const handleCurrencyBlur = (e) => {
    const { name, value } = e.target;
    if (value && value !== '') {
      const formatted = formatCurrency(value);
      setFormData(prev => ({
        ...prev,
        [name]: formatted
      }));
    }
  };

  const handleCurrencyFocus = (e) => {
    const { name, value } = e.target;
    if (value && value !== '') {
      const parsed = parseCurrency(value);
      setFormData(prev => ({
        ...prev,
        [name]: parsed
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};

    // Required fields
    const desiredIncome = parseFloat(parseCurrency(formData.desiredAfterTaxIncome));
    if (!desiredIncome || desiredIncome < 10000 || desiredIncome > 10000000) {
      newErrors.desiredAfterTaxIncome = 'Desired income must be between $10,000 and $10,000,000';
    }

    const currentAge = parseInt(formData.currentAge);
    if (!currentAge || currentAge < 18 || currentAge > 100) {
      newErrors.currentAge = 'Current age must be between 18 and 100';
    }

    const retirementAge = parseInt(formData.retirementAge);
    if (!retirementAge || retirementAge < 18 || retirementAge > 100) {
      newErrors.retirementAge = 'Retirement age must be between 18 and 100';
    }

    if (currentAge && retirementAge && retirementAge <= currentAge) {
      newErrors.retirementAge = 'Retirement age must be greater than current age';
    }

    const successRate = parseFloat(formData.successRateThreshold);
    if (!successRate || successRate < 0.85 || successRate > 0.98) {
      newErrors.successRateThreshold = 'Success rate must be between 85% and 98%';
    }

    // Optional fields validation
    if (formData.currentRetirementAccountBalance) {
      const retirementBalance = parseFloat(parseCurrency(formData.currentRetirementAccountBalance));
      if (retirementBalance < 0) {
        newErrors.currentRetirementAccountBalance = 'Balance cannot be negative';
      }
    }

    if (formData.currentTaxableAccountBalance) {
      const taxableBalance = parseFloat(parseCurrency(formData.currentTaxableAccountBalance));
      if (taxableBalance < 0) {
        newErrors.currentTaxableAccountBalance = 'Balance cannot be negative';
      }
    }

    if (formData.annualSavings) {
      const savings = parseFloat(parseCurrency(formData.annualSavings));
      if (savings < 0) {
        newErrors.annualSavings = 'Annual savings cannot be negative';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setApiError(null);

    if (!validateForm()) {
      return;
    }

    setIsLoading(true);

    try {
      const inputData = {
        desiredAfterTaxIncome: parseFloat(parseCurrency(formData.desiredAfterTaxIncome)),
        currentAge: parseInt(formData.currentAge),
        retirementAge: parseInt(formData.retirementAge),
        successRateThreshold: parseFloat(formData.successRateThreshold)
      };

      // Add optional fields if provided
      if (formData.currentRetirementAccountBalance) {
        inputData.currentRetirementAccountBalance = parseFloat(parseCurrency(formData.currentRetirementAccountBalance));
      }
      if (formData.currentTaxableAccountBalance) {
        inputData.currentTaxableAccountBalance = parseFloat(parseCurrency(formData.currentTaxableAccountBalance));
      }
      if (formData.annualSavings) {
        inputData.annualSavings = parseFloat(parseCurrency(formData.annualSavings));
      }

      const result = await calculateReverseRetirement(inputData);
      onCalculationComplete(result, inputData);
    } catch (error) {
      console.error('Reverse calculation error:', error);
      setApiError(
        error.response?.data?.error ||
        error.message ||
        'An error occurred while calculating. Please try again.'
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="reverse-input-form">
      <div className="form-section">
        <h3>Retirement Goal</h3>

        <div className="form-group">
          <label htmlFor="desiredAfterTaxIncome">
            Desired Annual After-Tax Income *
          </label>
          <input
            type="text"
            id="desiredAfterTaxIncome"
            name="desiredAfterTaxIncome"
            value={formData.desiredAfterTaxIncome}
            onChange={handleChange}
            onBlur={handleCurrencyBlur}
            onFocus={handleCurrencyFocus}
            className={errors.desiredAfterTaxIncome ? 'error' : ''}
            placeholder="$60,000"
            required
          />
          {errors.desiredAfterTaxIncome && (
            <span className="error-message">{errors.desiredAfterTaxIncome}</span>
          )}
          <small>How much do you want to spend per year in retirement (after taxes)?</small>
        </div>
      </div>

      <div className="form-section">
        <h3>Personal Information</h3>

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="currentAge">Current Age *</label>
            <input
              type="number"
              id="currentAge"
              name="currentAge"
              value={formData.currentAge}
              onChange={handleChange}
              className={errors.currentAge ? 'error' : ''}
              min="18"
              max="100"
              required
            />
            {errors.currentAge && (
              <span className="error-message">{errors.currentAge}</span>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="retirementAge">Target Retirement Age *</label>
            <input
              type="number"
              id="retirementAge"
              name="retirementAge"
              value={formData.retirementAge}
              onChange={handleChange}
              className={errors.retirementAge ? 'error' : ''}
              min="18"
              max="100"
              required
            />
            {errors.retirementAge && (
              <span className="error-message">{errors.retirementAge}</span>
            )}
          </div>
        </div>
      </div>

      <div className="form-section">
        <h3>Risk Profile</h3>

        <div className="form-group">
          <label htmlFor="successRateThreshold">Minimum Success Rate *</label>
          <select
            id="successRateThreshold"
            name="successRateThreshold"
            value={formData.successRateThreshold}
            onChange={handleChange}
            className={errors.successRateThreshold ? 'error' : ''}
            required
          >
            <option value="0.98">98% - Very Conservative</option>
            <option value="0.95">95% - Conservative</option>
            <option value="0.90">90% - Moderate</option>
            <option value="0.85">85% - Aggressive</option>
          </select>
          {errors.successRateThreshold && (
            <span className="error-message">{errors.successRateThreshold}</span>
          )}
          <small>Higher success rates require larger portfolios but provide more security</small>
        </div>
      </div>

      <div className="form-section optional-section">
        <h3>Current Situation (Optional)</h3>
        <p className="section-description">
          Provide your current savings to see gap analysis and savings recommendations
        </p>

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="currentRetirementAccountBalance">
              Current Retirement Account Balance
            </label>
            <input
              type="text"
              id="currentRetirementAccountBalance"
              name="currentRetirementAccountBalance"
              value={formData.currentRetirementAccountBalance}
              onChange={handleChange}
              onBlur={handleCurrencyBlur}
              onFocus={handleCurrencyFocus}
              className={errors.currentRetirementAccountBalance ? 'error' : ''}
              placeholder="$0"
            />
            {errors.currentRetirementAccountBalance && (
              <span className="error-message">{errors.currentRetirementAccountBalance}</span>
            )}
            <small>401(k), IRA, etc.</small>
          </div>

          <div className="form-group">
            <label htmlFor="currentTaxableAccountBalance">
              Current Taxable Account Balance
            </label>
            <input
              type="text"
              id="currentTaxableAccountBalance"
              name="currentTaxableAccountBalance"
              value={formData.currentTaxableAccountBalance}
              onChange={handleChange}
              onBlur={handleCurrencyBlur}
              onFocus={handleCurrencyFocus}
              className={errors.currentTaxableAccountBalance ? 'error' : ''}
              placeholder="$0"
            />
            {errors.currentTaxableAccountBalance && (
              <span className="error-message">{errors.currentTaxableAccountBalance}</span>
            )}
            <small>Brokerage accounts</small>
          </div>
        </div>

        <div className="form-group">
          <label htmlFor="annualSavings">Annual Savings</label>
          <input
            type="text"
            id="annualSavings"
            name="annualSavings"
            value={formData.annualSavings}
            onChange={handleChange}
            onBlur={handleCurrencyBlur}
            onFocus={handleCurrencyFocus}
            className={errors.annualSavings ? 'error' : ''}
            placeholder="$0"
          />
          {errors.annualSavings && (
            <span className="error-message">{errors.annualSavings}</span>
          )}
          <small>How much you save per year toward retirement</small>
        </div>
      </div>

      {apiError && (
        <div className="api-error">
          <strong>Error:</strong> {apiError}
        </div>
      )}

      <button
        type="submit"
        className="btn-primary calculate-button"
        disabled={isLoading}
      >
        {isLoading ? (
          <>
            <span className="spinner"></span>
            Calculating...
          </>
        ) : (
          'Calculate Required Portfolio'
        )}
      </button>
    </form>
  );
};

ReverseInputForm.propTypes = {
  onCalculationComplete: PropTypes.func.isRequired
};

export default ReverseInputForm;
