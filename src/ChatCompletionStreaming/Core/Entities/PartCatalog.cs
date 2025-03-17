using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ChatCompletionStreaming.Core.Entities;
public class PartCatalog
{
    [JsonPropertyName("id")]
    [Description("Unique Identifier for the part number")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("part_number")]
    [Description("Define the Part Number")]
    public string PartNumber { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    [Description("Description of the part number")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("available_quantity")]
    [Description("Available quantity in stock of the part number")]
    public int AvailableQuantity { get; set; }

    [JsonPropertyName("condition_code")]
    [Description("Aircraft part condition code of the part number")]
    public string ConditionCode { get; set; } = string.Empty;

    [JsonPropertyName("warehouse_name")]
    [Description("Name of the warehouse where the part number is located")]
    public string WarehouseName { get; set; } = string.Empty;

    [JsonPropertyName("unit_price")]
    [Description("Price per unit of the part number")]
    public float UnitPrice { get; set; }
}
