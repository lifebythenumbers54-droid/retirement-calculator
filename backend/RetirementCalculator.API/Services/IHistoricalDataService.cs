using RetirementCalculator.API.Models;

namespace RetirementCalculator.API.Services;

public interface IHistoricalDataService
{
    /// <summary>
    /// Gets all historical market data points.
    /// </summary>
    /// <returns>List of market data points sorted by year</returns>
    IReadOnlyList<MarketDataPoint> GetHistoricalData();

    /// <summary>
    /// Gets market data for a specific year.
    /// </summary>
    /// <param name="year">The year to retrieve data for</param>
    /// <returns>Market data point for the specified year, or null if not found</returns>
    MarketDataPoint? GetDataForYear(int year);

    /// <summary>
    /// Gets the range of years available in the historical data.
    /// </summary>
    /// <returns>Tuple containing (minYear, maxYear)</returns>
    (int MinYear, int MaxYear) GetYearRange();
}
