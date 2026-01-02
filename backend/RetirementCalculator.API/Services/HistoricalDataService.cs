using System.Text.Json;
using RetirementCalculator.API.Models;

namespace RetirementCalculator.API.Services;

public class HistoricalDataService : IHistoricalDataService
{
    private readonly ILogger<HistoricalDataService> _logger;
    private readonly List<MarketDataPoint> _historicalData;
    private readonly int _minYear;
    private readonly int _maxYear;

    public HistoricalDataService(ILogger<HistoricalDataService> logger)
    {
        _logger = logger;
        _historicalData = new List<MarketDataPoint>();

        try
        {
            LoadHistoricalData();
            ValidateData();

            if (_historicalData.Count > 0)
            {
                _minYear = _historicalData.Min(d => d.Year);
                _maxYear = _historicalData.Max(d => d.Year);
                _logger.LogInformation(
                    "Historical data loaded successfully. {Count} years of data from {MinYear} to {MaxYear}",
                    _historicalData.Count, _minYear, _maxYear);
            }
            else
            {
                throw new InvalidOperationException("No historical data was loaded");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize HistoricalDataService");
            throw;
        }
    }

    public IReadOnlyList<MarketDataPoint> GetHistoricalData()
    {
        return _historicalData.AsReadOnly();
    }

    public MarketDataPoint? GetDataForYear(int year)
    {
        return _historicalData.FirstOrDefault(d => d.Year == year);
    }

    public (int MinYear, int MaxYear) GetYearRange()
    {
        return (_minYear, _maxYear);
    }

    private void LoadHistoricalData()
    {
        var dataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "data", "historical_market_data.json");

        _logger.LogInformation("Loading historical data from: {FilePath}", dataFilePath);

        if (!File.Exists(dataFilePath))
        {
            throw new FileNotFoundException($"Historical market data file not found at: {dataFilePath}");
        }

        var jsonContent = File.ReadAllText(dataFilePath);
        var data = JsonSerializer.Deserialize<List<MarketDataPoint>>(jsonContent);

        if (data == null || data.Count == 0)
        {
            throw new InvalidOperationException("Failed to parse historical market data or file is empty");
        }

        _historicalData.AddRange(data.OrderBy(d => d.Year));
        _logger.LogInformation("Loaded {Count} data points", _historicalData.Count);
    }

    private void ValidateData()
    {
        var invalidDataPoints = new List<string>();

        foreach (var dataPoint in _historicalData)
        {
            var errors = new List<string>();

            if (dataPoint.Year < 1900 || dataPoint.Year > 2100)
            {
                errors.Add($"Invalid year: {dataPoint.Year}");
            }

            // Returns should be reasonable (between -100% and +200%)
            if (dataPoint.Sp500Return < -1m || dataPoint.Sp500Return > 2m)
            {
                errors.Add($"S&P 500 return out of reasonable range: {dataPoint.Sp500Return}");
            }

            if (dataPoint.BondReturn < -1m || dataPoint.BondReturn > 2m)
            {
                errors.Add($"Bond return out of reasonable range: {dataPoint.BondReturn}");
            }

            if (dataPoint.Inflation < -0.5m || dataPoint.Inflation > 0.5m)
            {
                errors.Add($"Inflation out of reasonable range: {dataPoint.Inflation}");
            }

            if (errors.Count > 0)
            {
                invalidDataPoints.Add($"Year {dataPoint.Year}: {string.Join(", ", errors)}");
            }
        }

        if (invalidDataPoints.Count > 0)
        {
            var errorMessage = $"Data validation failed:\n{string.Join("\n", invalidDataPoints)}";
            _logger.LogError(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        // Check for duplicate years
        var duplicateYears = _historicalData
            .GroupBy(d => d.Year)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateYears.Count > 0)
        {
            var errorMessage = $"Duplicate years found in historical data: {string.Join(", ", duplicateYears)}";
            _logger.LogError(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _logger.LogInformation("Data validation passed");
    }
}
