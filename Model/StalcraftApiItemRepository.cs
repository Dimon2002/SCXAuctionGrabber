using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCXAuctionGrabber.Domain.Base;
using SCXAuctionGrabber.Domain.DataStructures;
using SCXAuctionGrabber.Model.DTO;
using SCXAuctionGrabber.Model.Factory;
using SCXAuctionGrabber.Model.Interfaces;
using SCXAuctionGrabber.Model.Storage;
using System.Net;
using System.Text.RegularExpressions;

namespace SCXAuctionGrabber.Model;

public class StalcraftApiItemRepository : IItemRepository
{
    private const string BaseUrl = "https://stalcraft.wiki/api/exbo/item";
    private const string WikiDataUrl = "https://stalcraft.wiki/_next/data";

    private readonly HttpClient _httpClient;

    private string _buildId = "uirhDfCccrYCmPxDIZkBA";

    private readonly Dictionary<ItemCategory, string> Categorys = new()
    {
        { ItemCategory.Weapon, "weapon" },
        { ItemCategory.Armor, "armor" },
        { ItemCategory.Artefacts, "artefacts" },
        { ItemCategory.Attachments, "attachments" },
        { ItemCategory.Devices, "devices" },
        { ItemCategory.Containers, "containers" },
        { ItemCategory.Consumables, "consumables" },
        { ItemCategory.Other, "other" },
    };

    public StalcraftApiItemRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<RequestResult> GetItemByIdAsync(string exbo_id, ItemCategory category)
    {
        if (!Categorys.TryGetValue(category, out var result))
        {
            return new RequestResult(HttpStatusCode.BadRequest, null, $"Category {category} does not exist");
        }

        var namePair = await GetNameAsync(exbo_id, result);

        if (namePair.Item1 != HttpStatusCode.OK)
        {
            return new RequestResult(namePair.Item1, null, namePair.Item3);
        }

        var item = ItemFactory.CreateItem(id: exbo_id, name: namePair.Item2);

        var pricesPair = await GetPricesAsync(exbo_id);
        if (pricesPair.Item1 != HttpStatusCode.OK)
        {
            return new RequestResult(pricesPair.Item1, null, pricesPair.Item3);
        }

        item.Records = [.. pricesPair.Item2.Select(e => new BaseAuctionRecord(
            e.Time,
            e.Price,
            e.Amount)
        )];

        return new RequestResult(pricesPair.Item1, item, string.Empty);
    }

    private async Task<(HttpStatusCode, string, string)> GetNameAsync(string exbo_id, string category)
    {
        var url = $"{WikiDataUrl}/{_buildId}/ru/items/{category}/{exbo_id}.json?category={category}&item={exbo_id}";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        using var httpResponse = await _httpClient.SendAsync(httpRequest);
        if (!httpResponse.IsSuccessStatusCode)
        {
            var error = await httpResponse.Content.ReadAsStringAsync();

            return new(httpResponse.StatusCode, string.Empty, error);
        }

        var responseContentString = await httpResponse.Content.ReadAsStringAsync();
        var itemInfo = JsonConvert.DeserializeObject<JObject>(responseContentString);

        return (httpResponse.StatusCode, itemInfo
            ?.SelectToken("pageProps.data.name.lines.ru")
            ?.Value<string>()
            ?? "unown name"
            , string.Empty);
    }

    private async Task<(HttpStatusCode, IEnumerable<AuctionEntryDTO>, string)> GetPricesAsync(string exbo_id)
    {
        var uri = $"{BaseUrl}/auction-history?region=ru&id={exbo_id}";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
        using var httpResponse = await _httpClient.SendAsync(httpRequest);
        if (!httpResponse.IsSuccessStatusCode)
        {
            var error = await httpResponse.Content.ReadAsStringAsync();

            return new(httpResponse.StatusCode, [], error);
        }

        var responseContentString = await httpResponse.Content.ReadAsStringAsync();

        var auctionData = JsonConvert.DeserializeObject<AuctionResponseDTO>(responseContentString) ?? new AuctionResponseDTO()
        {
            Total = 0,
            Prices = []
        };

        return (httpResponse.StatusCode, auctionData.Prices, string.Empty);
    }

    public async Task UpdateBuildIDAsync()
    {
        const string uri = "https://stalcraft.wiki";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
        using var httpResponse = await _httpClient.SendAsync(httpRequest);
        if (!httpResponse.IsSuccessStatusCode)
        {
            var error = await httpResponse.Content.ReadAsStringAsync();

            return;
        }
        var responseContentString = await httpResponse.Content.ReadAsStringAsync();

        var start = responseContentString.IndexOf("__NEXT_DATA__");
        if (start > 0)
        {
            var end = responseContentString.IndexOf("</script>", start);
            var json = responseContentString[start..end];
            var match = Regex.Match(json, "\"buildId\":\"([^\"]+)\"");

            if (match.Success)
            {
                _buildId = match.Groups[1].Value;
            }
        }
    }
}