using Azure.Photo.Function.Constants;
using Azure.Photo.Function.Interface;
using Azure.Photo.Function.Models;
using Discord;
using Discord.Webhook;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Azure.Photo.Function.Services;

public class DiscordService(IConfiguration configuration, ILogger<DiscordService> logger) : IDiscordService
{
    private readonly string _discordWebhookConnectionString = configuration.GetValue<string>("DISCORD_WEBHOOK_CONNECTION_STRING") ?? string.Empty;
    private readonly ILogger<DiscordService> _logger = logger;

    public async Task SendDiscordNotification(string eventType, Data photoMetaData)
    {
        if (string.IsNullOrEmpty(_discordWebhookConnectionString))
        {
            throw new ArgumentException("Discord Webhook Connection String is null/empty", nameof(_discordWebhookConnectionString));
        }

        var embed = eventType == FunctionConstants.BlobCreatedEvent ? GetImageCreatedEventEmbed(photoMetaData) : GetImageDeletedEventEmbed(photoMetaData);

        try
        {
            using var client = new DiscordWebhookClient(_discordWebhookConnectionString);
            await client.SendMessageAsync(text: "Send a message to this webhook!", embeds: [embed.Build()]);
            _logger.LogInformation("Successfully sent embed message to Discord.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send embed message to Discord");
        }
    }

    private static EmbedBuilder GetImageCreatedEventEmbed(Data photoMetaData)
    {
        var encodedUrl = EncodeUrl(photoMetaData.Url);
        var embed = new EmbedBuilder
        {
            Title = "🚀 New Photo Uploaded!",
            Description = "A new photo has just been uploaded to your storage account.",
            Color = Color.Blue,
            ImageUrl = encodedUrl,
            Fields = [
                new(){Name="🖼️ Photo Name",Value=photoMetaData.Url.Split('/').Last(),IsInline=true},
                new(){Name="📏 Size",Value=$"{photoMetaData.ContentLength} Bytes",IsInline=true},
                new(){Name = "🌐 URI", Value = encodedUrl}
            ],
            Footer = new() { Text = "Azure Blob Storage", IconUrl = "https://azure.microsoft.com/svghandler/storage/" }

        };

        return embed;
    }
    private static EmbedBuilder GetImageDeletedEventEmbed(Data photoMetaData)
    {
        var encodedUrl = EncodeUrl(photoMetaData.Url);
        var embed = new EmbedBuilder
        {
            Title = "🗑️ Image Deleted",
            Description = "An existing image has been deleted from your storage account.",
            Color = Color.Red,
            ImageUrl = encodedUrl,
            Fields = [
                new(){ Name = "🖼️ Photo Name", Value = photoMetaData.Url.Split('/').Last(), IsInline=true },
                new(){ Name = "📏 Size", Value = $"{photoMetaData.ContentLength} Bytes", IsInline=true},
                new(){ Name = "🌐 URI", Value = encodedUrl}
            ],
            Footer = new() { Text = "Azure Blob Storage", IconUrl = "https://azure.microsoft.com/svghandler/storage/" }

        };

        return embed;
    }
    private static string EncodeUrl(string fullUrl)
    {
        var uri = new Uri(fullUrl);
        var baseUrl = fullUrl[..(fullUrl.LastIndexOf('/') + 1)];
        var fileName = uri.Segments[^1];
        var encodedFileName = Uri.EscapeDataString(fileName);
        var encodedUrl = $"{baseUrl}{encodedFileName}";
        return encodedUrl;
    }
}