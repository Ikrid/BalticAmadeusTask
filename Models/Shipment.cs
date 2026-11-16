namespace BalticAmadeusTask.Models;
using System.Text.Json.Serialization;


public class Shipment
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("client")]
    public string Client { get; set; } = "";

    [JsonPropertyName("date")]
    public string Date { get; set; } = "";

    [JsonPropertyName("volume")]
    public decimal Volume { get; set; }

    [JsonPropertyName("distance")]
    public decimal Distance { get; set; }

    [JsonPropertyName("basePrice")]
    public decimal BasePrice { get; set; }
}