using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SCXAuctionGrabber.Model.DTO;

public class AuctionEntryDTO
{
    [JsonProperty("amount")]
    public int Amount { get; set; }

    [JsonProperty("price")]
    public double Price { get; set; }

    [JsonProperty("time")]
    public DateTime Time { get; set; }

    [JsonProperty("additional")]
    public JObject Additional { get; set; }
}
