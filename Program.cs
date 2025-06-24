using Microsoft.Extensions.DependencyInjection;
using SCXAuctionGrabber.Domain.Base;
using SCXAuctionGrabber.Domain.Interfaces;
using SCXAuctionGrabber.MathStatistics;
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

var item = await auctionService.GetItemAuctionHistoryAsync("0rl9k");

PrintRecommendedPriceByItem(item);
//PrintItemInfo(item);

static void PrintItemInfo(IItem item)
{
    if (item is not Item baseItem) return;

    Console.WriteLine($"Item: {baseItem.Name} ({baseItem.Id})");

    Console.WriteLine("\nAuction history:");
    foreach (var record in baseItem.Records.OrderBy(e => e.Price))
    {
        Console.WriteLine($"{record.TimeStamp}: {record.Price:F2} RUB {record.Amount} шт");
    }

    Console.WriteLine(new string('=', 50));
}

static void PrintRecommendedPriceByItem(IItem item)
{
    if (item is not Item baseItem) return;

    var records = (item as Item)?.Records;

    var recommendPrice = ItemPriceAnalyzer.RecommendedPrice(records);
    var meanCut = ItemPriceAnalyzer.MeanCut(records);
    var trueValueMB = ItemPriceAnalyzer.CalculateStrategicPrice(records);
    var (min, max) = ItemPriceAnalyzer.GetPriceConfidenceInterval(records);
    
    Console.WriteLine($"Iterval [{min:F2};{max:F2}]");
    Console.WriteLine($"Item: {baseItem.Name}; Recomend Price: {recommendPrice:F2}");
    Console.WriteLine($"Item: {baseItem.Name}; Mean Price: {meanCut:F2}");
    Console.WriteLine($"Item: {baseItem.Name}; AVG Price: {trueValueMB:F2}");
}