using System.Text.Json.Serialization;

namespace RetirementCalculator.API.Models;

public class MarketDataPoint
{
    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("sp500Return")]
    public decimal Sp500Return { get; set; }

    [JsonPropertyName("bondReturn")]
    public decimal BondReturn { get; set; }

    [JsonPropertyName("inflation")]
    public decimal Inflation { get; set; }
}
