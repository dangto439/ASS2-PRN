using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace NewsManagementSystem.Hubs
{
    public class NewsHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
