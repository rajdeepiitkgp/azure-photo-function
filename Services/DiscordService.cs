using System.Text;
using System.Text.Json;
using Azure.Photo.Function.Constants;
using Azure.Photo.Function.Interface;
using Azure.Photo.Function.Models;
using Discord;
using Discord.Webhook;
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

        var embed = eventType == FunctionConstants.BlobCreatedEvent ? GetImageCreatedEventEmbed(photoMetaData) : GetImageDeletedEventEmbed(photoMetaData);

        try
        {
            // var response = await _httpClient.PostAsync(_discordWebhookConnectionString, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
            // response.EnsureSuccessStatusCode();
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
        // var embed = new
        // {
        //     embeds = new[]
        //             {
        //         new
        //         {
        //             title =  "🚀 New Photo Uploaded!" ,
        //             description = "A new photo has just been uploaded to your storage account.",
        //             color = 5814783,
        //             fields = new[]
        //             {
        //                 new { name = "🖼️ Photo Name", value = photoMetaData.Url.Split('/').Last(), inline = true },
        //                 new { name = "📏 Size", value = $"{photoMetaData.ContentLength} Bytes", inline = true },
        //                 new { name = "🌐 URI", value = photoMetaData.Url , inline = false }
        //             },
        //             image = new { url = photoMetaData.Url },
        //             footer = new { text = "Azure Blob Storage", icon_url = "https://azure.microsoft.com/svghandler/storage/" }
        //         }
        //     }
        // };
        // return JsonSerializer.Serialize(embed);
        var embed = new EmbedBuilder
        {
            Title = "🚀 New Photo Uploaded!",
            Description = "A new photo has just been uploaded to your storage account.",
            Color = Color.Blue,
            ImageUrl = photoMetaData.Url,
            Fields = [
                new(){Name="🖼️ Photo Name",Value=photoMetaData.Url.Split('/').Last(),IsInline=true},
                new(){Name="📏 Size",Value=$"{photoMetaData.ContentLength} Bytes",IsInline=true},
                new(){Name = "🌐 URI", Value = photoMetaData.Url}
            ],
            Footer = new() { Text = "Azure Blob Storage", IconUrl = "https://azure.microsoft.com/svghandler/storage/" }

        };

        return embed;
    }
    private static EmbedBuilder GetImageDeletedEventEmbed(Data photoMetaData)
    {
        // var embed = new
        // {
        //     embeds = new[]
        //             {
        //         new
        //         {
        //             title = "🗑️ Image Deleted",
        //             description = "An existing image has been deleted from your storage account.",
        //             color = 16711680,
        //             fields = new[]
        //             {
        //                 new { name = "🖼️ Photo Name", value = photoMetaData.Url.Split('/').Last(), inline = true },
        //                 new { name = "📏 Size", value = $"{photoMetaData.ContentLength} Bytes", inline = true },
        //                 new { name = "🌐 URI", value = photoMetaData.Url , inline = false }
        //             },
        //             image = new { url = photoMetaData.Url },
        //             footer = new { text = "Azure Blob Storage", icon_url = "https://azure.microsoft.com/svghandler/storage/" }
        //         }
        //     }
        // };

        // return JsonSerializer.Serialize(embed);
        var embed = new EmbedBuilder
        {
            Title = "🗑️ Image Deleted",
            Description = "An existing image has been deleted from your storage account.",
            Color = Color.Red,
            ImageUrl = photoMetaData.Url,
            Fields = [
                new(){ Name = "🖼️ Photo Name", Value = photoMetaData.Url.Split('/').Last(), IsInline=true },
                new(){ Name = "📏 Size", Value = $"{photoMetaData.ContentLength} Bytes", IsInline=true},
                new(){ Name = "🌐 URI", Value = photoMetaData.Url}
            ],
            Footer = new() { Text = "Azure Blob Storage", IconUrl = "https://azure.microsoft.com/svghandler/storage/" }

        };

        return embed;
    }

}
/*
{
  "content": "Welcome to **Embed Generator**! 🎉 Create stunning embed messages for your Discord server with ease!\n\nIf you're ready to start, simply click on the \"Clear\" button at the top of the editor and create your own message.\n\nShould you need any assistance or have questions, feel free to join our [support server](/discord) where you can connect with our helpful community members and get the support you need.\n\nWe also have a [complementary bot](/invite) that enhances the experience with Embed Generator. Check out our [Discord bot](/invite) which offers features like formatting for mentions, channels, and emoji, creating reaction roles, interactive components, and more.\n\nLet your creativity shine and make your server stand out with Embed Generator! ✨",
  "tts": false,
  "embeds": [
    {
      "id": 652627557,
      "title": "About Embed Generator",
      "description": "Embed Generator is a powerful tool that enables you to create visually appealing and interactive embed messages for your Discord server. With the use of webhooks, Embed Generator allows you to customize the appearance of your messages and make them more engaging.\n\nTo get started, all you need is a webhook URL, which can be obtained from the 'Integrations' tab in your server's settings. If you encounter any issues while setting up a webhook, our bot can assist you in creating one.\n\nInstead of using webhooks you can also select a server and channel directly here on the website. The bot will automatically create a webhook for you and use it to send the message.",
      "color": 2326507,
      "fields": []
    },
    {
      "id": 10674342,
      "title": "Discord Bot Integration",
      "description": "Embed Generator offers a Discord bot integration that can further enhance your the functionality. While it is not mandatory for sending messages, having the bot on your server gives you access to a lot more features!\n\nHere are some key features of our bot:",
      "color": 2326507,
      "fields": [
        {
          "id": 472281785,
          "name": "Interactive Components",
          "value": "With our bot on your server you can add interactive components like buttons and select menus to your messages. Just invite the bot to your server, select the right server here on the website and you are ready to go!"
        },
        {
          "id": 608893643,
          "name": "Special Formatting for Mentions, Channels, and Emoji",
          "value": "With the /format command, our bot provides special formatting options for mentions, channel tags, and ready-to-use emoji. No more manual formatting errors! Simply copy and paste the formatted text into the editor."
        },
        {
          "id": 724530251,
          "name": "Recover Embed Generator Messages",
          "value": "If you ever need to retrieve a previously sent message created with Embed Generator, our bot can assist you. Right-click or long-press any message in your server, navigate to the apps menu, and select Restore to Embed Generator. You'll receive a link that leads to the editor page with the selected message."
        },
        {
          "id": 927221233,
          "name": "Additional Features",
          "value": "Our bot also supports fetching images from profile pictures or emojis, webhook management, and more. Invite the bot to your server and use the /help command to explore all the available features!"
        }
      ]
    }
  ],
  "components": [],
  "actions": {}
}

*/