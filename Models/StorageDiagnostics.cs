using System.Text.Json.Serialization;

namespace Azure.Photo.Function.Models;

public class StorageDiagnostics
{
    [JsonPropertyName("batchId")]
    public string BatchId { get; set; } = string.Empty;
}