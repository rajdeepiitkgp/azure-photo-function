using Azure.Messaging.EventGrid;
using Azure.Photo.Function.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Azure.Photo.Function;

public class PhotoUploadEmailTrigger(ILogger<PhotoUploadEmailTrigger> logger)
{
    private readonly ILogger<PhotoUploadEmailTrigger> _logger = logger;

    [Function(nameof(PhotoUploadEmailTrigger))]
    public async Task Run([EventGridTrigger] EventGridEvent photoEvent)
    {
        try
        {
            _logger.LogInformation("Event type: {type}, Event subject: {subject}", photoEvent.EventType, photoEvent.Subject);

            if (photoEvent.EventType != FunctionConstants.BlobCreatedEvent && photoEvent.EventType != FunctionConstants.BlobDeletedEvent)
            {
                _logger.LogInformation("Email Event type - {eventType} not Blob Created or Deleted, hence ignored", photoEvent.EventType);
                return;
            }
            await Task.Delay(30);

            _logger.LogInformation("This is a simulation of Email getting fired");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Some error occured while processing function");
            throw;
        }
    }
}
