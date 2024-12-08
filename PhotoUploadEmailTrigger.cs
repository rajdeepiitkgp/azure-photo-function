using Azure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Azure.Photo.Function;

public class PhotoUploadEmailTrigger(ILogger<PhotoUploadEmailTrigger> logger)
{
    private readonly ILogger<PhotoUploadEmailTrigger> _logger = logger;

    [Function(nameof(PhotoUploadEmailTrigger))]
    public async Task Run([EventGridTrigger] CloudEvent cloudEvent)
    {
        _logger.LogInformation("Email Event type: {type}, Event subject: {subject}", cloudEvent.Type, cloudEvent.Subject);

        if (cloudEvent.Type != "Microsoft.Storage.BlobCreated" && cloudEvent.Type != "Microsoft.Storage.BlobCreated")
        {
            _logger.LogInformation("Email Event type not Blob Created or Deleted, hence ignored");
            return;
        }
        await Task.Delay(3000);

        _logger.LogInformation("This is a simulation of Email getting fired");
    }
}
