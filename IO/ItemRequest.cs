using SCXAuctionGrabber.Domain.DataStructures;

namespace SCXAuctionGrabber.IO;

public class ItemRequest
{
    public required ItemCategory Category { get; set; }
    public required string Id { get; set; }
}
