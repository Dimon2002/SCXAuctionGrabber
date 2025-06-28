namespace SCXAuctionGrabber.IO.Interfaces;

public interface IReader
{
    public IEnumerable<ItemRequest> Parse(string path);
}
