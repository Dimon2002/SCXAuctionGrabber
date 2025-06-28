using SCXAuctionGrabber.Domain.DataStructures;
using SCXAuctionGrabber.Domain.Interfaces;

namespace SCXAuctionGrabber.Model.Interfaces;

public interface IAuctionService
{
    Task Setup();
    Task<IItem> GetItemAuctionHistoryAsync(string itemId, ItemCategory category);
    Task<IList<IAuctionRecord>> GetItemAuctionRecordsAsync(string itemId, ItemCategory category);
}
