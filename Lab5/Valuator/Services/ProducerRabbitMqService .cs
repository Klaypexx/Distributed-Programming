using RabbitMQ.Client;
using System.Text;

namespace Valuator.Services
{
    public class ProducerRabbitMqService : IRabbitMqService
    {
        private readonly string _messageExchangeName;
        private readonly string _messageRoutingKey;
        private readonly string _eventExchangeName;
        private readonly string _eventRoutingKey;
        private readonly string _hostName;

        public ProducerRabbitMqService( string messageExchangeName,
            string messageRoutingKey,
            string eventExchangeName,
            string eventRoutingKey,
            string hostName )
        {
            _messageExchangeName = messageExchangeName;
            _messageRoutingKey = messageRoutingKey;
            _eventExchangeName = eventExchangeName;
            _eventRoutingKey = eventRoutingKey;
            _hostName = hostName;
        }

        public async Task SendMessage( string message )
        {
            Task produceTask = ProduceAsync(message);

            await produceTask;
        }

        public async Task SendEvent( string id, double calculatedValue )
        {
            EventMessage message = new(id, calculatedValue);

            Task produceTask = ProduceEventAsync(message);

            await produceTask;
        }

        private async Task ProduceAsync( string message )
        {
            ConnectionFactory factory = new()
            {
                HostName = _hostName
            };
            await using IConnection connection = await factory.CreateConnectionAsync();
            await using IChannel channel = await connection.CreateChannelAsync(null);

            await DeclareTopologyAsync(channel);

            byte[] body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: _messageExchangeName,
                routingKey: _messageRoutingKey,
                mandatory: false,
                body: body
            );

            await connection.CloseAsync();
        }

        private async Task ProduceEventAsync( EventMessage message )
        {
            ConnectionFactory factory = new()
            {
                HostName = _hostName
            };
            await using IConnection connection = await factory.CreateConnectionAsync();
            await using IChannel channel = await connection.CreateChannelAsync(null);

            await DeclareEventTopologyAsync(channel);

            string serializedMessage = EventMessage.Serialize(message);
            byte[] body = Encoding.UTF8.GetBytes(serializedMessage);

            await channel.BasicPublishAsync(
                exchange: _eventExchangeName,
                routingKey: _eventRoutingKey,
                mandatory: false,
                body: body
            );

            await connection.CloseAsync();
        }

        private async Task DeclareTopologyAsync( IChannel channel )
        {
            await channel.ExchangeDeclareAsync(
                exchange: _messageExchangeName,
                type: ExchangeType.Direct
            );
        }

        private async Task DeclareEventTopologyAsync( IChannel channel )
        {
            await channel.ExchangeDeclareAsync(
                exchange: _eventExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );
        }
    }
}
