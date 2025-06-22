using SCXAuctionGrabber.Domain.Interfaces;

namespace SCXAuctionGrabber.Domain.Base;

public class BaseAuctionRecord : IAuctionRecord
{
    public DateTime TimeStamp { get; }

    public double Price { get; }

    public int Amount { get; }

    public BaseAuctionRecord(DateTime date, double price, int amount)
    {
        TimeStamp = date;
        Price = price;
        Amount = amount;
    }
}
