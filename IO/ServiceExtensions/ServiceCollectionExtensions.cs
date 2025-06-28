using Microsoft.Extensions.DependencyInjection;
using SCXAuctionGrabber.IO.Exporters;
using SCXAuctionGrabber.IO.Interfaces;
using SCXAuctionGrabber.IO.Readers;

namespace SCXAuctionGrabber.IO.ServiceExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIOComponents(this IServiceCollection services)
    {
        services.AddScoped<IReader, ItemParamsReader>();
        services.AddScoped<PriceExporter>();
        services.AddScoped<RecommendedPurchasePriceExporter>();

        return services;
    }
}
