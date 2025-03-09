using Microsoft.AspNetCore.SignalR;

namespace NewsManagementSystem.Pages.Hubs
{
    public class NewsHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
