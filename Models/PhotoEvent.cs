using System.Text.Json.Serialization;

namespace Azure.Photo.Function.Models;

public class PhotoEvent
{
    [JsonPropertyName("topic")]
    public string Topic { get; set; } = string.Empty;

    [JsonPropertyName("subject")]
    public string Subject { get; set; } = string.Empty;

    [JsonPropertyName("eventType")]
    public string EventType { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public Data Data { get; set; } = new();

    [JsonPropertyName("dataVersion")]
    public string DataVersion { get; set; } = string.Empty;

    [JsonPropertyName("metadataVersion")]
    public string MetadataVersion { get; set; } = string.Empty;

    [JsonPropertyName("eventTime")]
    public DateTime EventTime { get; set; }
}

