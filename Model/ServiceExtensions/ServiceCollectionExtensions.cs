using Microsoft.Extensions.DependencyInjection;
using SCXAuctionGrabber.Model.Interfaces;
using SCXAuctionGrabber.Model.Services;

namespace SCXAuctionGrabber.Model.ServiceExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStalcraftAuctionServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IItemRepository, StalcraftApiItemRepository>();
        services.AddScoped<IAuctionService, BaseAuctionService>();

        return services;
    }
}
