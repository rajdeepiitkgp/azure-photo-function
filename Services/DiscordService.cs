using System.Text;
using System.Text.Json;
using Azure.Photo.Function.Constants;
using Azure.Photo.Function.Interface;
using Azure.Photo.Function.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Azure.Photo.Function.Services;

public class DiscordService(IConfiguration configuration, ILogger<DiscordService> logger, HttpClient httpClient) : IDiscordService
{
    private readonly string _discordWebhookConnectionString = configuration.GetValue<string>("DISCORD_WEBHOOK_CONNECTION_STRING") ?? string.Empty;
    private readonly ILogger<DiscordService> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;

    public async Task SendDiscordNotification(string eventType, Data photoMetaData)
    {
        if (string.IsNullOrEmpty(_discordWebhookConnectionString))
        {
            throw new ArgumentException("Discord Webhook Connection String is null/empty", nameof(_discordWebhookConnectionString));
        }

        var jsonPayload = eventType == FunctionConstants.BlobCreatedEvent ? GetImageCreatedEventEmbed(photoMetaData) : GetImageDeletedEventEmbed(photoMetaData);

        try
        {
            var response = await _httpClient.PostAsync(_discordWebhookConnectionString, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Successfully sent embed message to Discord.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send embed message to Discord");
        }
    }

    private static string GetImageCreatedEventEmbed(Data photoMetaData)
    {
        var embed = new
        {
            embeds = new[]
                    {
                new
                {
                    title =  "🚀 New Photo Uploaded!" ,
                    description = $"A new photo has just been uploaded to your storage account.",
                    color = 5814783,
                    fields = new[]
                    {
                        new { name = "🖼️ Photo Name", value = photoMetaData.Url.Split('/').Last(), inline = true },
                        new { name = "📏 Size", value = $"{photoMetaData.ContentLength} Bytes", inline = true },
                        new { name = "🌐 URI", value = photoMetaData.Url , inline = false }
                    },
                    image = new { url = photoMetaData.Url },
                    footer = new { text = "Azure Blob Storage", icon_url = "https://azure.microsoft.com/svghandler/storage/" }
                }
            }
        };

        return JsonSerializer.Serialize(embed);
    }
    private static string GetImageDeletedEventEmbed(Data photoMetaData)
    {
        var embed = new
        {
            embeds = new[]
                    {
                new
                {
                    title = "🗑️ Image Deleted",
                    description = "An existing image has been deleted from your storage account.",
                    color = 16711680,
                    fields = new[]
                    {
                        new { name = "🖼️ Photo Name", value = photoMetaData.Url.Split('/').Last(), inline = true },
                        new { name = "📏 Size", value = $"{photoMetaData.ContentLength} Bytes", inline = true },
                        new { name = "🌐 URI", value = photoMetaData.Url , inline = false }
                    },
                    image = new { url = photoMetaData.Url },
                    footer = new { text = "Azure Blob Storage", icon_url = "https://azure.microsoft.com/svghandler/storage/" }
                }
            }
        };

        return JsonSerializer.Serialize(embed);
    }

}
