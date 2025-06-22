namespace SCXAuctionGrabber.Domain.Base;

public abstract class BaseItemProperty
{
    public string Id { get; }
    public string Name { get; }

    protected BaseItemProperty(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
