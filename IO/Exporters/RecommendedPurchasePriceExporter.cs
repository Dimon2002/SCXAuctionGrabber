using SCXAuctionGrabber.Domain.Interfaces;
using SCXAuctionGrabber.IO.Interfaces;
using SCXAuctionGrabber.MathStatistics;
using System.Text;

namespace SCXAuctionGrabber.IO.Exporters;

public class RecommendedPurchasePriceExporter : IExporter
{
    public Func<IList<IAuctionRecord>, double> RecommendedPrice => ItemPriceAnalyzer.RecommendedPrice;

    public void Setup(string filePath)
    {
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

        writer.WriteLine("Название товара;Рекомендованная цена");
    }

    public void ExportToConsole(IItem item)
    {
        var records = item.Records;

        Console.WriteLine($"[{DateTime.Now}] The item \"{item.Name}\" price - {RecommendedPrice(records):F2}");
    }

    public void ExportToCSV(IItem item, string filePath)
    {
        using var writer = new StreamWriter(filePath, true);

        writer.WriteLine($"{item.Name};{RecommendedPrice(item.Records):F2}");
    }

    public void FinalizeExport()
    {
        // TODO: logger
        Console.WriteLine($"[{DateTime.Now}] Recommended prices of goods successfully exported");
    }
}
