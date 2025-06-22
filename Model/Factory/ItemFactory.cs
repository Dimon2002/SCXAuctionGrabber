using SCXAuctionGrabber.Domain.Base;

namespace SCXAuctionGrabber.Model.Factory;

public class ItemFactory
{
    public static Item CreateItem(string id, string name)
    {
        return new Item(id, name);
    }
}
