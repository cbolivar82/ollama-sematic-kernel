using System.Text.Json;

using ChatCompletionStreaming.Core.Entities;

namespace ChatCompletionStreaming.Data;
// **************************************
// TODO: 2.4 Data Service 
public class PartCatalogService : IPartCatalogService
{
    public PartCatalogService()
    {
        LoadPartCatalog();
    }

    public List<PartCatalog> Data { get; set; } = [];

    private List<PartCatalog> LoadPartCatalog()
    {
        string json = File.ReadAllText("Data/part-catalog.json");
        Data = JsonSerializer.Deserialize<List<PartCatalog>>(json) ?? [];
        return Data;
    }

    public PartCatalog? Get(string partNumber)
    {
        return Data
            .FirstOrDefault(x => x.PartNumber.Equals(partNumber, StringComparison.OrdinalIgnoreCase));
    }
}
