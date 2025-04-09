using Microsoft.AspNetCore.SignalR.Client;

namespace RankCalculator.Services
{
    public class SignalRClientService
    {
        public async Task SendMessage( string connectionUrl, string id, string message )
        {
            try
            {
                var connection = new HubConnectionBuilder()
                    .WithUrl(connectionUrl)
                    .Build();

                await connection.StartAsync();

                await connection.InvokeAsync("SendMessage", id, message);
                Console.WriteLine("Сообщение signalR отправлено!");

                await connection.DisposeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка подключения: " + ex.Message);
            }
        }
    }
}
