using System.Text.Json;
using Azure.Core.Serialization;
using Azure.Messaging.EventGrid;
using Azure.Photo.Function.Constants;
using Azure.Photo.Function.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Azure.Photo.Function;

public class PhotoUploadEventTrigger(ILogger<PhotoUploadEventTrigger> logger)
{
    private readonly ILogger<PhotoUploadEventTrigger> _logger = logger;

    [Function(nameof(PhotoUploadEventTrigger))]
    public void Run([EventGridTrigger] EventGridEvent photoEvent)
    {
        try
        {
            _logger.LogInformation("Event type: {type}, Event subject: {subject}", photoEvent.EventType, photoEvent.Subject);

            if (photoEvent.EventType != FunctionConstants.BlobCreatedEvent && photoEvent.EventType != FunctionConstants.BlobDeletedEvent)
            {
                _logger.LogInformation("Event type - {eventType} not Blob Created or Deleted, hence ignored", photoEvent.EventType);
                return;
            }
            var data = photoEvent.Data;
            if (data is null) return;
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var serializer = new JsonObjectSerializer(options);
            var blobUrl = data.ToObject<Data>(serializer)?.Url ?? string.Empty;

            _logger.LogInformation("blobUrl is {blobUrl}", blobUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Some error occured while processing function");
            throw;
        }
    }
}

