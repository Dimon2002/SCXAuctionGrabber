using Microsoft.Extensions.DependencyInjection;
using SCXAuctionGrabber.Domain.Base;
using SCXAuctionGrabber.Domain.Interfaces;
using SCXAuctionGrabber.Model.ServiceExtensions;
using SCXAuctionGrabber.Model.Services;

var serviceProvider = new ServiceCollection()
    .AddStalcraftAuctionServices()
    .BuildServiceProvider(new ServiceProviderOptions
    {
        ValidateOnBuild = true
    });

var auctionService = serviceProvider.GetService<IAuctionService>();
await auctionService!.Setup();

var item = await auctionService.GetItemAuctionHistoryAsync("55olq");

PrintItemInfo(item);

static void PrintItemInfo(IItem item)
{
    if (item is not Item baseItem) return;

    Console.WriteLine($"Item: {baseItem.Name} ({baseItem.Id})");

    Console.WriteLine("\nAuction history:");
    foreach (var record in baseItem!.Records.OrderBy(e => e.Price))
    {
        Console.WriteLine($"{record.TimeStamp}: {record.Price:F2} RUB {record.Amount} шт");
    }

    Console.WriteLine(new string('=', 50));
}
