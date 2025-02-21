using System.ComponentModel;

using ChatCompletionStreaming.Core.Entities;
using ChatCompletionStreaming.Data;

using Microsoft.SemanticKernel;

namespace ChatCompletionStreaming;
public class PartCatalogPlugin
{
    private readonly IPartCatalogService _partCatalogService;

    public PartCatalogPlugin(IPartCatalogService partCatalogService)
    {
        _partCatalogService = partCatalogService;
    }

    //[KernelFunction(nameof(GetPartNumberAsString))]
    //[Description("Retrieve Part Number Inventory Information based on the given Part Number.")]
    //public string GetPartNumberAsString(
    //    [Description("The part number to retrieve.")] string partNumber)
    //{
    //    var data = _partCatalogService.GetPartCatalog(partNumber);

    //    return data is not null
    //        ? $"Part {partNumber} '{data.Description}' is in stock."
    //        : $"Part {partNumber} is not in stock.";
    //}

    [KernelFunction(nameof(RetreivePartNumber))]
    [Description(@"""Retrieve Part Number Record based on the given Part Number and return:
    PN: this partNumber field
    Cond: this conditionCode field
    Unit Price: this is the price field formatted as currency
    Warehouse: this is the warehouse field.
    Do not change the data or autocomplete the text""")]
    public PartCatalog? RetreivePartNumber(
        [Description("The part number to retrieve.")] string partNumber)
    {
        return _partCatalogService.GetPartCatalog(partNumber);
    }

    [KernelFunction(nameof(GetEmailContent))]
    [Description("Write a email text content for a customer with a proposal quote related to the part number. Offer write a email if the part nume is in stock.")]
    public string GetEmailContent(
        [Description("The part number to write the email.")] string partNumber)
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
            : "Part Number is not in stock";
    }
}
