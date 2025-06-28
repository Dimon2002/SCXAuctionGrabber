using SCXAuctionGrabber.Domain.Interfaces;

namespace SCXAuctionGrabber.Domain.Base;

public class Item : IItem
{
    public string Id { get; }

    public string Name { get; }
    
    public IList<IAuctionRecord> Records { get; set; }

    public Item(string id, string name)
    {
        Id = id;
        Name = name;
        Records = [];
    }
}
