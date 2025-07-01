using Microsoft.Extensions.Logging;
using SCXAuctionGrabber.Domain.DataStructures;
using SCXAuctionGrabber.Domain.Interfaces;
using SCXAuctionGrabber.Model.Interfaces;
using System.Net;

namespace SCXAuctionGrabber.Model.Services;

public class BaseAuctionService : IAuctionService
{
    private readonly ILogger<BaseAuctionService> _logger;
    private readonly IItemRepository _itemRepository;

    public BaseAuctionService(
        IItemRepository itemRepository, 
        ILogger<BaseAuctionService> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
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
            _logger.LogError("[{DateTime}] the item ({itemId}) contains an error {requestResult.Error}", DateTime.Now, itemId, requestResult.Error);
        }

        return requestResult.Result;
    }

    public async Task<IList<IAuctionRecord>> GetItemAuctionRecordsAsync(string itemId, ItemCategory category)
    {
        var response = await _itemRepository.GetItemByIdAsync(itemId, category);

        return response.Result.Records;
    }
}
