using Newtonsoft.Json;

namespace SCXAuctionGrabber.Model.DTO;

public class AuctionResponseDTO
{
    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("prices")]
    public List<AuctionEntryDTO> Prices { get; set; } = [];
}
