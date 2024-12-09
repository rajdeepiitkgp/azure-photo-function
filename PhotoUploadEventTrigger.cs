using System.Text.Json;
using Azure.Core.Serialization;
using Azure.Messaging.EventGrid;
using Azure.Photo.Function.Constants;
using Azure.Photo.Function.Interface;
using Azure.Photo.Function.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Azure.Photo.Function;

public class PhotoUploadEventTrigger(ILogger<PhotoUploadEventTrigger> logger, IDiscordService discordService)
{
    private readonly ILogger<PhotoUploadEventTrigger> _logger = logger;
    private readonly IDiscordService _discordService = discordService;

    [Function(nameof(PhotoUploadEventTrigger))]
    public async Task Run([EventGridTrigger] EventGridEvent photoEvent)
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
            if (data is null)
            {
                _logger.LogWarning("Data is null in the massage, hence skipping operation");
                return;
            }

            var blobExtractedMetaData = ExtractBlobMetadata(data);
            if (blobExtractedMetaData is null)
            {
                _logger.LogWarning("blob Extracted Metadata, hence skipping process");
                return;
            }
            await _discordService.SendDiscordNotification(photoEvent.EventType, blobExtractedMetaData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Some error occured while processing function : {functionName}", nameof(PhotoUploadEventTrigger));
            throw;
        }
    }

    private static Data? ExtractBlobMetadata(BinaryData photoBinaryData)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var serializer = new JsonObjectSerializer(options);
        return photoBinaryData.ToObject<Data>(serializer);
    }
}

