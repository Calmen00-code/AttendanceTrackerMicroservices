using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AttendanceTrackerMicroservices.Hubs
{
    public class RefreshHub : Hub
    {
        public async Task SendRefreshPageCommand()
        {
            // Send a message to all connected clients to refresh the page
            await Clients.All.SendAsync("RefreshPage");
        }
    }
}
