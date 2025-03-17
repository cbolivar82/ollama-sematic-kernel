using System.ComponentModel;

using ChatCompletionStreaming.Core.Entities;
using ChatCompletionStreaming.Data;

using Microsoft.SemanticKernel;

namespace ChatCompletionStreaming;
public class PartCatalogPlugin
{
    public const string CreateEmailTextFuncName = "create_email_text";
    public const string RetrivePartNumberRecordFuncName = "check_part_number_in_stock_and_retrive_details";
    public const string CreateQuoteFuncName = "create_part_number_quote";
    private readonly IPartCatalogService _partCatalogService;

    public PartCatalogPlugin(IPartCatalogService partCatalogService)
    {
        _partCatalogService = partCatalogService;
    }

    [KernelFunction(RetrivePartNumberRecordFuncName)]
    [Description(@"Check a part number is in stock and retrieve Part Number Details")]
    //[return: Description("Part Number is in stock with the Details.")]
    public string RetreivePartNumberInfo(
        [Description("The part number to retrieve.")] string partNumber)
    {
        var result = _partCatalogService.Get(partNumber);

        if (result is null)
        {
            return "Part out of stock";
        }

        return @$"Part number {result.PartNumber} '{result.Description}' is in stock and here are the details:
                Available Quantity: {result.AvailableQuantity}
                Price: {result.UnitPrice:C}
                Condition Code: {result.ConditionCode}
                Warehouse: {result.WarehouseName}";
    }

    [KernelFunction(CreateEmailTextFuncName)]
    [Description("Create email text with the part number information.")]
    public string CreateEmailText(
        [Description("The part number to create the email.")] string partNumber)
    {
        var data = _partCatalogService.Get(partNumber);
        return data is not null
            ? $"Dear Customer,\n\n" +
              $"We are pleased to inform you that we have the following part in stock:\n\n" +
              $"Part Number: {data.PartNumber}\n" +
              $"Description: {data.Description}\n" +
              $"Avialable Quantity: {data.AvailableQuantity}\n" +
              $"Condition: {data.ConditionCode}\n" +
              $"Quantity:  {data.UnitPrice:C}\n\n" +
              $"Please let us know if you would like to proceed with the purchase.\n\n" +
              $"Best regards,\n" +
              $"Customer Support Team"
            : "Part Number is out of stock";
    }

    [KernelFunction(CreateQuoteFuncName)]
    [Description("Create a quote with part number details.")]
    public PartCatalog? CreateQuote(
        [Description("The part number to create the quote.")] string partNumber)
    {
        return _partCatalogService.Get(partNumber);
    }
}
