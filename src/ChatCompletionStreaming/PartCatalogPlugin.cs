using System.ComponentModel;

using ChatCompletionStreaming.Data;

using Microsoft.SemanticKernel;

namespace ChatCompletionStreaming;
public class PartCatalogPlugin
{
    public const string GenerateEmailTextFuncName = "generate_email_text";
    public const string RetrivePartNumberRecordFuncName = "check_part_number_in_stock_and_retrive_details";
    private readonly IPartCatalogService _partCatalogService;

    public PartCatalogPlugin(IPartCatalogService partCatalogService)
    {
        _partCatalogService = partCatalogService;
    }

    [KernelFunction(RetrivePartNumberRecordFuncName)]
    [Description(@"Check if the part number is in stock and retrieve Part Number Details")]
    [return: Description("Part Number is in stock with the Details.")]
    public string RetreivePartNumberInfo(
        [Description("The part number to retrieve.")] string partNumber)
    {
        var result = _partCatalogService.GetPartCatalog(partNumber);

        if (result is null)
        {
            return "Part out of stock";
        }

        return @$"Part {result.PartNumber} is in stock and here are the details:
                {nameof(result.Quantity)}: {result.Quantity}
                {nameof(result.UnitOfMeasure)}: {result.UnitOfMeasure}
                {nameof(result.Price)}: {result.Price}
                {nameof(result.Warehouse)}: {result.Warehouse}
                {nameof(result.WarehouseCountry)}: {result.WarehouseCountry}
                {nameof(result.IsHazmat)}: {result.IsHazmat}";
    }

    [KernelFunction(GenerateEmailTextFuncName)]
    [Description("Generate email text of a part number.")]
    public string GenerateEmailText(
        [Description("The part number to generate email text.")] string partNumber)
    {
        var data = _partCatalogService.GetPartCatalog(partNumber);
        return data is not null
            ? $"Dear Customer,\n\n" +
              $"We are pleased to inform you that we have the following part in stock:\n\n" +
              $"Part Number: {data.PartNumber}\n" +
              $"Description: {data.Description}\n" +
              $"Qty: {data.Quantity}\n" +
              $"Condition: {data.ConditionCode}\n" +
              $"Unit Price: {data.UnitOfMeasure}\n\n" +
              $"Please let us know if you would like to proceed with the purchase.\n\n" +
              $"Best regards,\n" +
              $"Customer Support Team"
            : "Part Number is out of stock";
    }
}
