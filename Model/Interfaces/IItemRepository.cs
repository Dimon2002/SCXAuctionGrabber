using SCXAuctionGrabber.Domain.DataStructures;
using SCXAuctionGrabber.Model.Storage;

namespace SCXAuctionGrabber.Model.Interfaces;

public interface IItemRepository
{
    Task UpdateBuildIDAsync();
    Task<RequestResult> GetItemByIdAsync(string id, ItemCategory category);
}
