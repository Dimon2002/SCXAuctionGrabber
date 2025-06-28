namespace SCXAuctionGrabber.Domain.Interfaces;

public interface IItem
{
    public string Id { get; }
    public string Name { get; }

    public IList<IAuctionRecord> Records { get; set; }
}
