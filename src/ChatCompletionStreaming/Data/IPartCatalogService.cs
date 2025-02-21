
using ChatCompletionStreaming.Core.Entities;

namespace ChatCompletionStreaming.Data;
public interface IPartCatalogService
{
    List<PartCatalog> Data { get; set; }

    PartCatalog? GetPartCatalog(string partNumber);
}