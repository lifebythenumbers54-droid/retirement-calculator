import axios from 'axios';

const API_BASE_URL = '/api';
const MAX_RETRIES = 3;
const RETRY_DELAY = 1000;

/**
 * API Client for the Retirement Calculator backend
 * All API calls are proxied through Vite to http://localhost:5000
 * Includes comprehensive error handling, retry logic, and request/response logging
 */

// Create axios instance with default configuration
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000, // 30 second timeout
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request interceptor for logging
apiClient.interceptors.request.use(
  (config) => {
    if (import.meta.env.DEV) {
      console.log('[API Request]', {
        method: config.method?.toUpperCase(),
        url: config.url,
        data: config.data
      });
    }
    return config;
  },
  (error) => {
    if (import.meta.env.DEV) {
      console.error('[API Request Error]', error);
    }
    return Promise.reject(error);
  }
);

// Response interceptor for logging and error handling
apiClient.interceptors.response.use(
  (response) => {
    if (import.meta.env.DEV) {
      console.log('[API Response]', {
        status: response.status,
        url: response.config.url,
        data: response.data
      });
    }
    return response;
  },
  (error) => {
    if (import.meta.env.DEV) {
      console.error('[API Response Error]', {
        message: error.message,
        status: error.response?.status,
        data: error.response?.data,
        url: error.config?.url
      });
    }
    return Promise.reject(error);
  }
);

/**
 * Sleep utility for retry delays
 * @param {number} ms - Milliseconds to sleep
 * @returns {Promise<void>}
 */
const sleep = (ms) => new Promise(resolve => setTimeout(resolve, ms));

/**
 * Retry wrapper for API calls
 * @param {Function} fn - Async function to retry
 * @param {number} retries - Number of retries remaining
 * @returns {Promise<any>} Result of the function
 */
const retryWithBackoff = async (fn, retries = MAX_RETRIES) => {
  try {
    return await fn();
  } catch (error) {
    if (retries === 0) {
      throw error;
    }

    // Only retry on network errors or 5xx server errors
    const shouldRetry =
      !error.response ||
      (error.response.status >= 500 && error.response.status < 600);

    if (!shouldRetry) {
      throw error;
    }

    const delay = RETRY_DELAY * (MAX_RETRIES - retries + 1);
    if (import.meta.env.DEV) {
      console.log(`[API Retry] Retrying in ${delay}ms... (${retries} retries left)`);
    }

    await sleep(delay);
    return retryWithBackoff(fn, retries - 1);
  }
};

/**
 * Format error message for user display
 * @param {Error} error - Axios error object
 * @returns {string} User-friendly error message
 */
const formatErrorMessage = (error) => {
  if (!error.response) {
    return 'Unable to connect to the server. Please check your internet connection and try again.';
  }

  switch (error.response.status) {
    case 400:
      return error.response.data?.message || 'Invalid input data. Please check your entries and try again.';
    case 404:
      return 'The requested resource was not found. Please try again.';
    case 500:
      return 'An internal server error occurred. Please try again later.';
    case 503:
      return 'The service is temporarily unavailable. Please try again in a moment.';
    default:
      return `An error occurred (${error.response.status}). Please try again.`;
  }
};

/**
 * Submit calculation request to the backend
 * Includes automatic retry logic for network errors and 5xx errors
 *
 * @param {Object} inputData - User input data
 * @param {number} inputData.currentAge - Current age (18-100)
 * @param {number} inputData.retirementAge - Retirement age (>= current age)
 * @param {number} inputData.retirementAccountBalance - 401k/IRA balance
 * @param {number} inputData.taxableAccountBalance - Taxable account balance
 * @param {number} inputData.successRateThreshold - Success rate (0.90, 0.95, or 0.98)
 * @returns {Promise<Object>} Calculation results
 * @throws {Error} Formatted error with user-friendly message
 */
export const calculateRetirement = async (inputData) => {
  try {
    const startTime = performance.now();

    const response = await retryWithBackoff(async () => {
      return await apiClient.post('/Calculation', inputData);
    });

    const endTime = performance.now();
    const duration = endTime - startTime;

    if (import.meta.env.DEV) {
      console.log(`[API Performance] Calculation completed in ${duration.toFixed(2)}ms`);
    }

    return response.data;
  } catch (error) {
    const userMessage = formatErrorMessage(error);
    const enhancedError = new Error(userMessage);
    enhancedError.originalError = error;
    throw enhancedError;
  }
};

/**
 * Analyze portfolio allocations and return top 3 recommended strategies
 * Includes automatic retry logic for network errors and 5xx errors
 *
 * @param {Object} inputData - User input data
 * @param {number} inputData.currentAge - Current age (18-100)
 * @param {number} inputData.retirementAge - Retirement age (>= current age)
 * @param {number} inputData.retirementAccountBalance - 401k/IRA balance
 * @param {number} inputData.taxableAccountBalance - Taxable account balance
 * @param {number} inputData.successRateThreshold - Success rate (0.90, 0.95, or 0.98)
 * @returns {Promise<Object>} Allocation analysis results with Conservative, Balanced, and Aggressive strategies
 * @throws {Error} Formatted error with user-friendly message
 */
export const analyzeAllocations = async (inputData) => {
  try {
    const startTime = performance.now();

    const response = await retryWithBackoff(async () => {
      return await apiClient.post('/Calculation/analyze-allocations', inputData);
    });

    const endTime = performance.now();
    const duration = endTime - startTime;

    if (import.meta.env.DEV) {
      console.log(`[API Performance] Allocation analysis completed in ${duration.toFixed(2)}ms`);
    }

    return response.data;
  } catch (error) {
    const userMessage = formatErrorMessage(error);
    const enhancedError = new Error(userMessage);
    enhancedError.originalError = error;
    throw enhancedError;
  }
};

/**
 * Health check endpoint to verify API connectivity
 * @returns {Promise<Object>} API status
 * @throws {Error} Formatted error with user-friendly message
 */
export const checkApiHealth = async () => {
  try {
    const response = await apiClient.get('/health');
    return response.data;
  } catch (error) {
    const userMessage = formatErrorMessage(error);
    const enhancedError = new Error(userMessage);
    enhancedError.originalError = error;
    throw enhancedError;
  }
};
