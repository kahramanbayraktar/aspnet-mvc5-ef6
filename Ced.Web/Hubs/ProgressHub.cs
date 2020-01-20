using Microsoft.AspNet.SignalR;

namespace Ced.Web.Hubs
{
    // Hub Class
    public class ProgressHub : Hub
    {
        public static void SendProgress(string connectionId, string progressMessage, int progressCount, int totalItems, int taskId)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            var percentage = (progressCount * 100) / totalItems;

            //var connectionId = CurrentHubConnectionId();
            //hubContext.Clients.All.AddProgress(progressMessage, percentage + "%", taskId);
            hubContext.Clients.Client(connectionId).AddProgress(progressMessage, percentage + "%", taskId);
            //hubContext.Clients.Clients(CurrentHubConnectionIds()).AddProgress(progressMessage, percentage + "%", taskId);
        }

        //public static IList<HubUser> HubUsers = new List<HubUser>();
        //public static string ConnectionId { get; set; }

        //public override Task OnConnected()
        //{
        //    //var currentHubUser = HubUsers.SingleOrDefault(x => x.UserName == "kahramanb@ite-turkey.com");
        //    //if (currentHubUser != null)
        //    //    HubUsers.Remove(currentHubUser);
        //    HubUsers.Add(new HubUser { UserName = "kahramanb@ite-turkey.com", ConnectionId = Context.ConnectionId });

        //    //ConnectionId = Context.ConnectionId;

        //    return base.OnConnected();
        //}
    }

    //public class HubUser
    //{
    //    public string UserName { get; set; }
    //    public string ConnectionId { get; set; }
    //}

    //private static IList<string> CurrentHubConnectionIds()
    //{
    //    return ProgressHub.HubUsers.Select(x => x.ConnectionId).ToList();
    //    //return ProgressHub.ConnectionId;
    //}
}