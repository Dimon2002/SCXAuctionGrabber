namespace SCXAuctionGrabber.IO.Interfaces;

public interface IReader
{
    public IEnumerable<ItemParams> Parse(string path);
}
