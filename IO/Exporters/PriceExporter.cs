using Newtonsoft.Json;
using SCXAuctionGrabber.Domain.Interfaces;
using SCXAuctionGrabber.IO.Interfaces;
using System.Globalization;
using System.Text;

namespace SCXAuctionGrabber.IO.Exporters;

public class PriceExporter : IExporter
{
    public void ExportToConsole(IItem item)
    {
        Console.WriteLine($"Название товара: {item.Name}");
        Console.WriteLine($"ID товара: {item.Id}");
        Console.WriteLine($"Всего записей: {item.Records.Count}");
        Console.WriteLine();
        Console.WriteLine("Дата;Цена (RUB);Количество;Дополнительно");

        foreach (var record in item.Records)
        {
            var date = record.TimeStamp.ToString();
            var price = record.Price.ToString("N0", CultureInfo.InvariantCulture).Replace(",", " ");
            var amount = record.Amount.ToString("N0", CultureInfo.InvariantCulture);
            var additional = record.Additional != null
                ? JsonConvert.SerializeObject(record.Additional, Formatting.None)
                : string.Empty;

            Console.WriteLine($"{date};{price};{amount};{additional}");
        }
    }

    public void ExportToCSV(IItem item, string filePath)
    {
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

        writer.WriteLine($"Название товара: {item.Name}");
        writer.WriteLine($"ID товара: {item.Id}");
        writer.WriteLine($"Всего записей: {item.Records.Count}");
        writer.WriteLine();
        writer.WriteLine("Дата;Цена (RUB);Количество;Дополнительно");

        foreach (var record in item.Records)
        {
            var date = record.TimeStamp.ToString();
            var price = record.Price.ToString("N0", CultureInfo.InvariantCulture).Replace(",", " ");
            var amount = record.Amount.ToString("N0", CultureInfo.InvariantCulture);
            var additional = record.Additional != null
                ? JsonConvert.SerializeObject(record.Additional, Formatting.None)
                : string.Empty;

            writer.WriteLine($"{date};{price};{amount};{additional}");
        }

        // TODO: logger
        Console.WriteLine($"[{DateTime.Now}] The item \"{item.Name}\" ({item.Id}) successfully exported");
    }
}
