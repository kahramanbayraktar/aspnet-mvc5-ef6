using Microsoft.AspNet.SignalR;

namespace Ced.Web.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return request.GetHttpContext().User.Identity.Name;
        }
    }
}