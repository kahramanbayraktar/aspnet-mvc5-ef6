using System.Net;

namespace Ced.BusinessServices.Models
{
    public class CedRestResponse : ICedRestResponse
    {
        public string Content { get; set; }

        public HttpStatusCode StatusCode { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}
