namespace Valuator.Services
{
    public interface IRabbitMqService
    {
        Task SendMessage( string message );
        Task SendEvent( string id, double calculatedValue );
    }
}
