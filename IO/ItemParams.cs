using SCXAuctionGrabber.Domain.DataStructures;

namespace SCXAuctionGrabber.IO;

public class ItemParams
{
    public required ItemCategory Category { get; set; }
    public required string Id { get; set; }
}
