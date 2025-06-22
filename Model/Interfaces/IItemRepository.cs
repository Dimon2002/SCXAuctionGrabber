using SCXAuctionGrabber.Model.Storage;

namespace SCXAuctionGrabber.Model.Interfaces;

public interface IItemRepository
{
    Task UpdateBuildIdAsync();
    Task<RequestResult> GetItemByIdAsync(string id);
}
