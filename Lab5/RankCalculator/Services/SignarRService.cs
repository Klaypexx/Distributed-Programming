using Microsoft.AspNetCore.SignalR.Client;

namespace RankCalculator.Services
{
    internal class SignarRService
    {
        private readonly HubConnection _connection;

        public SignarRService(string hubUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                await _connection.StartAsync();
                await _connection.InvokeAsync("RankCalculated", message);
                Console.WriteLine("Сообщение отправлено успешно.");
                await _connection.StopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
            }
        }
    }
}
