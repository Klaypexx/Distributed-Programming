using RabbitMQ.Client;
using System.Text;

namespace RankCalculator.Services
{
    public class ProducerRabbitMQService
    {

        private IConnection _connection;
        private IChannel _channel;
        private readonly ConnectionFactory _factory;

        public ProducerRabbitMQService( string hostName )
        {
            _factory = new ConnectionFactory { HostName = hostName };
        }

        public async Task InitializeAsync( string exchangeName )
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await DeclareTopologyAsync(_channel, exchangeName);
        }

        public async Task SendMessage( string exchangeName, string routingKey, string id, double calculatedValue )
        {
            EventMessage message = new(id, calculatedValue);

            Task produceTask = ProduceAsync(exchangeName, routingKey, message);

            await produceTask;
        }

        private async Task ProduceAsync( string exchangeName, string routingKey, EventMessage message )
        {
            string serializedMessage = EventMessage.Serialize(message);
            byte[] body = Encoding.UTF8.GetBytes(serializedMessage);

            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: routingKey,
                mandatory: false,
                body: body
            );

            Console.WriteLine($"Message {message} recived succesfully");

            await _connection.CloseAsync();
        }

        private static async Task DeclareTopologyAsync( IChannel channel, string exchangeName )
        {
            await channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );
        }
    }
}
