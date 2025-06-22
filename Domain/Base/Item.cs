using SCXAuctionGrabber.Domain.Interfaces;

namespace SCXAuctionGrabber.Domain.Base;

public class Item : BaseItemProperty, IItem
{
    public IList<IAuctionRecord> Records { get; set; }

    public Item(string id, string name) : base (id, name)
    {
        Records = [];
    }
}
