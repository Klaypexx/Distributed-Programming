using Microsoft.AspNetCore.SignalR;

namespace SignalRHub.Services
{
    public class NotificationHubService : Hub
    {
        private readonly ILogger<NotificationHubService> _logger;

        public NotificationHubService(ILogger<NotificationHubService> logger)
        {
            _logger = logger;
        }

        public async Task Subscribe(string id)
        {
            _logger.LogInformation($"Подписка id: {id}");
            await Groups.AddToGroupAsync(Context.ConnectionId, id);
        }

        public async Task SendMessage(string id, string message)
        {
            _logger.LogInformation($"Сообщение получено успешно: {message}, id: {id}");
            await Clients.Group(id).SendAsync("ReceiveMessage", message);
        }
    }
}
