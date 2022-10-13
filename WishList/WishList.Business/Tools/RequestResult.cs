using System.Net;

namespace WishList.Business.Tools
{
    public class RequestResult
    {
        public HttpStatusCode resultCode { get; set; }
        public string Message { get; set; }
    }
}
