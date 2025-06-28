using SCXAuctionGrabber.Domain.DataStructures;
using SCXAuctionGrabber.Domain.Interfaces;
using SCXAuctionGrabber.Model.Interfaces;
using System.Net;

namespace SCXAuctionGrabber.Model.Services;

public class BaseAuctionService : IAuctionService
{
    private readonly IItemRepository _itemRepository;

    public BaseAuctionService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task Setup()
    {
        await _itemRepository.UpdateBuildIDAsync();
    }

    public async Task<IItem> GetItemAuctionHistoryAsync(string itemId, ItemCategory category)
    {
        var requestResult = await _itemRepository.GetItemByIdAsync(itemId, category);

        if (requestResult.StatusCode != HttpStatusCode.OK) 
        {
            // TODO: logger
            Console.WriteLine($"[{DateTime.Now}] the item ({itemId}) contains an error {requestResult.Error}");
        }

        return requestResult.Result;
    }

    public async Task<IList<IAuctionRecord>> GetItemAuctionRecordsAsync(string itemId, ItemCategory category)
    {
        var response = await _itemRepository.GetItemByIdAsync(itemId, category);

        return response.Result.Records;
    }
}
