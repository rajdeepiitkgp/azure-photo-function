using Azure.Photo.Function.Models;

namespace Azure.Photo.Function.Interface;

public interface IDiscordService
{
    Task SendDiscordNotification(string eventType, Data photoMetaData);
}
