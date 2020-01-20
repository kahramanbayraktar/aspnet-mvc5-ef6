using System.Net;

namespace Ced.BusinessServices.Models
{
    public interface ICedRestResponse
    {
        string Content { get; set; }

        HttpStatusCode StatusCode { get; set; }
        
        string ErrorMessage { get; set; }
    }
}
