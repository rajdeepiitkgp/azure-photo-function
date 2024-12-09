using System.Text.Json.Serialization;

namespace Azure.Photo.Function.Models;

public class Data
{
    [JsonPropertyName("api")]
    public string Api { get; set; } = string.Empty;

    [JsonPropertyName("clientRequestId")]
    public string ClientRequestId { get; set; } = string.Empty;

    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("eTag")]
    public string ETag { get; set; } = string.Empty;

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = string.Empty;

    [JsonPropertyName("contentLength")]
    public int ContentLength { get; set; }

    [JsonPropertyName("blobType")]
    public string BlobType { get; set; } = string.Empty;

    [JsonPropertyName("accessTier")]
    public string AccessTier { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("sequencer")]
    public string Sequencer { get; set; } = string.Empty;

    [JsonPropertyName("storageDiagnostics")]
    public StorageDiagnostics StorageDiagnostics { get; set; } = new();
}
