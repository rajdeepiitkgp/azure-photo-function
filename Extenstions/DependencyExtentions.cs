using Azure.Photo.Function.Interface;
using Azure.Photo.Function.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Azure.Photo.Function.Extenstions;

public static class DependencyExtentions
{
    public static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IDiscordService, DiscordService>();
    }

}
