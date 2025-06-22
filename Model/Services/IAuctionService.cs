using SCXAuctionGrabber.Domain.Interfaces;

namespace SCXAuctionGrabber.Model.Services;

public interface IAuctionService
{
    Task Setup();
    Task<IItem> GetItemAuctionHistoryAsync(string itemId);
    Task<IList<IAuctionRecord>> GetItemAuctionRecordsAsync(string itemId);
}
