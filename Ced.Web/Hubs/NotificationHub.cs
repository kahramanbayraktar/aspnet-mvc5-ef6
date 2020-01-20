using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ced.Web.Hubs
{
    public class NotificationHub : Hub
    {
        // Limit connectionIds stored in memory. It shouldn't be limitless.
        public static List<KeyValuePair<string, string>> Receivers = new List<KeyValuePair<string, string>>();

        public static void PushNotification(List<string> userIds, string title, string url)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();

            var connectionIds = Receivers.Where(x => userIds.Select(u => u.ToLower()).Contains(x.Key.ToLower())).Select(y => y.Value).ToList();

            //hubContext.Clients.All.DisplayNotification(title, url);
            hubContext.Clients.Clients(connectionIds).DisplayNotification(title, url);
        }

        // Remove a user's connectionIds after logging out.
        public override Task OnConnected()
        {
            var userId = Context.User.Identity.Name;
            Receivers.Add(new KeyValuePair<string, string> (userId, Context.ConnectionId));

            return base.OnConnected();
        }
    }
}