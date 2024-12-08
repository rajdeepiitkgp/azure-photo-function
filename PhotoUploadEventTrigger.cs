// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Azure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Azure.Photo.Function;

public class PhotoUploadEventTrigger(ILogger<PhotoUploadEventTrigger> logger)
{
    private readonly ILogger<PhotoUploadEventTrigger> _logger = logger;
    private const string BlobCreatedEvent = "Microsoft.Storage.BlobCreated";
    private const string BlobDeletedEvent = "Microsoft.Storage.BlobDeleted";

    [Function(nameof(PhotoUploadEventTrigger))]
    public void Run([EventGridTrigger] CloudEvent cloudEvent)
    {
        _logger.LogInformation("Event type: {type}, Event subject: {subject}", cloudEvent.Type, cloudEvent.Subject);

        // if (cloudEvent.Type != "Microsoft.Storage.BlobCreated" && cloudEvent.Type != "Microsoft.Storage.BlobDeleted")
        // {
        //     _logger.LogInformation("Event type not Blob Created or Deleted, hence ignored");
        //     return;
        // }
        // var data = cloudEvent.Data;
        // if (data is null) return;
        // var blobUrl = data.ToDynamicFromJson()["url"];

        // _logger.LogInformation($"blobUrl is {blobUrl}");
        try
        {
            if (cloudEvent is null || string.IsNullOrWhiteSpace(cloudEvent.Type))
            {
                throw new ArgumentNullException("Null or Invalid Event Grid Event");
            }
            _logger.LogInformation($@"New Event Grid Event:
    - Id=[{cloudEvent.Id}]
    - EventType=[{cloudEvent.Type}]
    - EventTime=[{cloudEvent.Time}]
    - Subject=[{cloudEvent.Subject}]");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}

