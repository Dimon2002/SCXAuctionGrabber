using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SCXAuctionGrabber.Domain.Base;
using SCXAuctionGrabber.IO.Exporters;
using SCXAuctionGrabber.IO.Interfaces;
using SCXAuctionGrabber.Model.Interfaces;
using System.Text.RegularExpressions;

namespace SCXAuctionGrabber.AuctionGrabberService;

public class AuctionWorker : BackgroundService
{
    private readonly ILogger<AuctionWorker> _logger;
    private readonly IReader _reader;
    private readonly PriceExporter _priceExporter;
    private readonly RecommendedPurchasePriceExporter _recommendedExporter;
    private readonly IAuctionService _auctionService;
    private readonly AppSettings _settings;
    
    public AuctionWorker(
            ILogger<AuctionWorker> logger,
            IReader reader,
            RecommendedPurchasePriceExporter recommendedExporter,
            PriceExporter priceExporter,
            IAuctionService auctionService,
            IConfiguration configuration)
    {
        _logger = logger;
        _reader = reader;
        _priceExporter = priceExporter;
        _recommendedExporter = recommendedExporter;
        _auctionService = auctionService;
        _settings = configuration.GetSection("AppSettings").Get<AppSettings>()!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var BasePath = _settings.BasePath;
        var InputFolder = _settings.InputFolder;
        var OutputFolder = _settings.OutputFolder;
        var recommendedPriceFileName = _settings.RecommendedPriceFile;

        await _auctionService.Setup();
        _recommendedExporter.Setup(Path.Combine(BasePath, OutputFolder, recommendedPriceFileName));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting processing..");

                var itemsRequest = _reader.Parse(Path.Combine(BasePath, InputFolder, "ItemsList.txt"));

                foreach (var itemRequest in itemsRequest)
                {
                    var item = await _auctionService.GetItemAuctionHistoryAsync(itemRequest.Id, itemRequest.Category);

                    if (item is EmptyItem)
                    {
                        _logger.LogWarning("Item {id} not found. Skip it.", itemRequest.Id);
                        continue;
                    }

                    var priceFilePath = Path.Combine(
                        BasePath,
                        OutputFolder,
                        SanitizeFileName(item.Name) + ".csv"
                    );

                    _priceExporter.ExportToCSV(item, priceFilePath);
                    _recommendedExporter.ExportToCSV(item, Path.Combine(BasePath, OutputFolder, recommendedPriceFileName));
                }

                _recommendedExporter.FinalizeExport();
                _logger.LogInformation("Processing completed successfully");

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forced stop");

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private string SanitizeFileName(string name)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(name.Where(c => !invalidChars.Contains(c)).ToArray());
        return Regex.Replace(sanitized, @"\s+", "_");
    }
}
