using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ChatCompletionStreaming.Core.Entities;
public class PartCatalog
{
    [JsonPropertyName("id")]
    //[Description("Unique identifier for the part")]
    public Guid Id { get; set; }

    [JsonPropertyName("partNumber")]
    //[Description("Part number of the item")]
    public string PartNumber { get; set; } = string.Empty;

    [JsonPropertyName("serialNumber")]
    //[Description("Serial number of the item")]
    public string SerialNumber { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    //[Description("Description of the item")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    //[Description("Quantity of the item in stock")]
    public int Quantity { get; set; }

    [JsonPropertyName("conditionCode")]
    [Description("Condition code value are Factory New (FN), Equipment Manufacturer (OEM), New (NE), New Surplus (NS), Modified (MD), Overhauled (OH), Serviceable (SV), As Removed (AR), Rejected (RJ)")]
    public string ConditionCode { get; set; } = string.Empty;

    [JsonPropertyName("unitOfMeasure")]
    //[Description("Unit of measure for the item")]
    public string UnitOfMeasure { get; set; } = string.Empty;

    [JsonPropertyName("stockStatus")]
    [Description("Stock status of the item")]
    public string StockStatus { get; set; } = string.Empty;

    [JsonPropertyName("traceCode")]
    //[Description("Trace code of the item")]
    public string TraceCode { get; set; } = string.Empty;

    [JsonPropertyName("tagDate")]
    //[Description("Tag date of the item")]
    public DateTime TagDate { get; set; }

    [JsonPropertyName("tagShow")]
    //[Description("Tag show of the item")]
    public string TagShow { get; set; } = string.Empty;

    [JsonPropertyName("shelfLifeDays")]
    //[Description("Shelf life days of the item")]
    public int ShelfLifeDays { get; set; }

    [JsonPropertyName("isHazmat")]
    [Description("Indicates if the item is hazardous material")]
    public bool IsHazmat { get; set; }

    [JsonPropertyName("minimumOrderQuantity")]
    //[Description("Minimum order quantity of the item")]
    public int MinimumOrderQuantity { get; set; }

    [JsonPropertyName("leadDays")]
    //[Description("Lead days for the item")]
    public int LeadDays { get; set; }

    [JsonPropertyName("warehouseCode")]
    //[Description("Warehouse code where the item is stored")]
    public string WarehouseCode { get; set; } = string.Empty;

    [JsonPropertyName("warehouse")]
    [Description("Warehouse where the item is stored")]
    public string Warehouse { get; set; } = string.Empty;

    [JsonPropertyName("warehouseCountry")]
    [Description("Country of the warehouse where the item is stored")]
    public string WarehouseCountry { get; set; } = string.Empty;

    [JsonPropertyName("projectCode")]
    //[Description("Project code associated with the item")]
    public string ProjectCode { get; set; } = string.Empty;

    [JsonPropertyName("vendor")]
    //[Description("Vendor of the item")]
    public string Vendor { get; set; } = string.Empty;

    [JsonPropertyName("unitCost")]
    //[Description("Unit cost of the item")]
    public decimal UnitCost { get; set; }

    [JsonPropertyName("lastTouchDate")]
    //[Description("Last touch date of the item")]
    public DateTime LastTouchDate { get; set; }

    [JsonPropertyName("traceCategory")]
    //[Description("Trace category of the item")]
    public string TraceCategory { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    [Description("Price of the part number")]
    public decimal UnitPrice { get; set; }
}
