using RabbitMQ.Client;
using System.Text;

namespace RankCalculator.Services
{
    public class ProducerRabbitMQService
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly string _echangeName;
        private readonly string _routingKey;
        private readonly ConnectionFactory _factory;

        private ProducerRabbitMQService() { }

        public ProducerRabbitMQService( string hostName, string exchangeName, string routingKey )
        {
            _factory = new ConnectionFactory { HostName = hostName };
            _echangeName = exchangeName;
            _routingKey = routingKey;   
        }

        public async Task InitializeAsync()
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await DeclareTopologyAsync(_channel, _echangeName);
        }

        public async Task SendMessage( string id, double calculatedValue )
        {
            EventMessage message = new(id, calculatedValue);

            Task produceTask = ProduceAsync(message);

            await produceTask;
        }

        private async Task ProduceAsync( EventMessage message )
        {
            string serializedMessage = EventMessage.Serialize(message);
            byte[] body = Encoding.UTF8.GetBytes(serializedMessage);

            await _channel.BasicPublishAsync(
                exchange: _echangeName,
                routingKey: _routingKey,
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
