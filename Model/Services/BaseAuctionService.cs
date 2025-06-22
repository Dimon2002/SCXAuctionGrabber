using SCXAuctionGrabber.Domain.Base;
using SCXAuctionGrabber.Domain.Interfaces;
using SCXAuctionGrabber.Model.Interfaces;

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
        await _itemRepository.UpdateBuildIdAsync();
    }

    public async Task<IItem> GetItemAuctionHistoryAsync(string itemId)
    {
        var requestResult = await _itemRepository.GetItemByIdAsync(itemId);

        if (requestResult.StatusCode != System.Net.HttpStatusCode.OK) 
        {
            // TODO: logger
            Console.WriteLine($"[{DateTime.Now}] the item ({itemId}) contains an error {requestResult.Error}");
        }

        return requestResult.Result;
    }

    public async Task<IList<IAuctionRecord>> GetItemAuctionRecordsAsync(string itemId)
    {
        var response = await _itemRepository.GetItemByIdAsync(itemId);

        if (response.Result is not Item item)
        {
            return [];
        }
     
        return item.Records;
    }
}
