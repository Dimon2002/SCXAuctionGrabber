using SCXAuctionGrabber.Domain.Base;
using SCXAuctionGrabber.Domain.Interfaces;
using System.Net;

namespace SCXAuctionGrabber.Model.Storage;

public class RequestResult
{
    public HttpStatusCode StatusCode { get; }
    public IItem Result
    {
        get
        {
            if (StatusCode == HttpStatusCode.OK)
            {
                return _result;
            }

            return new EmptyItem();
        }
    }
    public string Error { get; }

    private readonly IItem _result;

    public RequestResult(HttpStatusCode statusCode, IItem? item, string error)
    {
        StatusCode = statusCode;
        _result = item ?? new EmptyItem();
        Error = error;
    }
}
