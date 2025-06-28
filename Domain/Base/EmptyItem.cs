using SCXAuctionGrabber.Domain.Interfaces;

namespace SCXAuctionGrabber.Domain.Base;

public class EmptyItem : IItem
{
    public string Id => "-1";

    public string Name => "Unown";

    public IList<IAuctionRecord> Records { get; set; } = [];
}
