using Microsoft.Extensions.DependencyInjection;
using SCXAuctionGrabber.Domain.Base;
using SCXAuctionGrabber.Domain.Interfaces;
using SCXAuctionGrabber.IO.Exporters;
using SCXAuctionGrabber.IO.Interfaces;
using SCXAuctionGrabber.IO.ServiceExtensions;
using SCXAuctionGrabber.MathStatistics;
using SCXAuctionGrabber.Model.Interfaces;
using SCXAuctionGrabber.Model.ServiceExtensions;
using System.Text.RegularExpressions;

const string BasePath = "../../../";
const string InputFolder = "Input/";
const string OutputFolder = "Output/";

const string recommendedPriceFileName = "Price.csv";

var serviceProvider = new ServiceCollection()
    .AddStalcraftAuctionServices()
    .AddIOComponents()
    .BuildServiceProvider(new ServiceProviderOptions
    {
        ValidateOnBuild = true
    });

var readerService = serviceProvider.GetRequiredService<IReader>();
var writerPriceService = serviceProvider.GetRequiredService<PriceExporter>();
var writerRecommendedService = serviceProvider.GetRequiredService<RecommendedPurchasePriceExporter>();

var auctionService = serviceProvider.GetService<IAuctionService>();

await auctionService!.Setup();
writerRecommendedService.Setup(Path.Combine(BasePath, OutputFolder, recommendedPriceFileName));

var itemsRequest = readerService?.Parse($"{BasePath}{InputFolder}ItemsList.txt");

if (itemsRequest is not null)
{
    foreach (var itemRequest in itemsRequest)
    {
        var item = await auctionService.GetItemAuctionHistoryAsync(itemRequest.Id, itemRequest.Category);

        if (item is EmptyItem)
        {
            continue;
        }

        var priceFilePath = Path.Combine(
            BasePath,
            OutputFolder,
            SanitizeFileName(item.Name) + ".csv"
        );

        //writerPriceService.ExportToConsole(item);
        writerRecommendedService.ExportToConsole(item);

        writerPriceService.ExportToCSV(item, priceFilePath);
        writerRecommendedService.ExportToCSV(item, Path.Combine(BasePath, OutputFolder, recommendedPriceFileName));
    }

    writerRecommendedService.FinalizeExport();
}

static void PrintRecommendedPriceByItem(IItem item)
{
    if (item is not Item baseItem) return;

    var records = (item as Item)?.Records;

    var recommendPrice = ItemPriceAnalyzer.RecommendedPrice(records);
    var meanCut = ItemPriceAnalyzer.MeanCut(records);
    var trueValueMB = ItemPriceAnalyzer.CalculateStrategicPrice(records);
    var (min, max) = ItemPriceAnalyzer.GetPriceConfidenceInterval(records);

    Console.WriteLine($"Item: {baseItem.Name}");

    Console.WriteLine($"Iterval [{min:F2};{max:F2}]");
    Console.WriteLine($"Recomend Price: {recommendPrice:F2}");
    Console.WriteLine($"Mean Price: {meanCut:F2}");
    Console.WriteLine($"AVG Price: {trueValueMB:F2}");
}

static string SanitizeFileName(string name)
{
    var invalidChars = Path.GetInvalidFileNameChars();
    var sanitized = new string([.. name.Where(c => !invalidChars.Contains(c))]);

    return Regex.Replace(sanitized, @"\s+", "_");
}