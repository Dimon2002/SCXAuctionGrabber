using SCXAuctionGrabber.Domain.Base;

namespace SCXAuctionGrabber.Domain.Interfaces;

public interface IAuctionRecord
{
    public DateTime TimeStamp { get; }
    
    public double Price { get; }
    
    public int Amount { get; } 

    public Additional? Additional { get; }
}
