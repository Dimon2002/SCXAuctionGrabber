using SCXAuctionGrabber.Domain.Interfaces;

namespace SCXAuctionGrabber.IO.Interfaces;

public interface IExporter
{
    void ExportToCSV(IItem item, string filePath);
    void ExportToConsole(IItem item);
}
