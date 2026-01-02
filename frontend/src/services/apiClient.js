import axios from 'axios';

const API_BASE_URL = '/api';

/**
 * API Client for the Retirement Calculator backend
 * All API calls are proxied through Vite to http://localhost:5000
 */

/**
 * Submit calculation request to the backend
 * @param {Object} inputData - User input data
 * @param {number} inputData.currentAge - Current age (18-100)
 * @param {number} inputData.retirementAge - Retirement age (>= current age)
 * @param {number} inputData.retirementAccountBalance - 401k/IRA balance
 * @param {number} inputData.taxableAccountBalance - Taxable account balance
 * @param {number} inputData.successRateThreshold - Success rate (0.90, 0.95, or 0.98)
 * @returns {Promise<Object>} Calculation results
 */
export const calculateRetirement = async (inputData) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/calculate`, inputData);
    return response.data;
  } catch (error) {
    console.error('Error calculating retirement:', error);
    throw error;
  }
};

/**
 * Health check endpoint to verify API connectivity
 * @returns {Promise<Object>} API status
 */
export const checkApiHealth = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/health`);
    return response.data;
  } catch (error) {
    console.error('Error checking API health:', error);
    throw error;
  }
};
