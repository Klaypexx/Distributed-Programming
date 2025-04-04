using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EventLogger.Services
{
    public class ConsumerRabbitMQService
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly ConnectionFactory _factory;

        public ConsumerRabbitMQService( string hostName )
        {
            _factory = new ConnectionFactory { HostName = hostName };
        }

        public async Task InitializeAsync( string exchangeName,
            string queueName )
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await DeclareTopologyAsync( _channel, exchangeName, queueName );
        }

        public async Task<string> RunConsumerAsync(string queueName)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async ( _, eventArgs ) =>
            {
                string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                EventMessage deserializedMessage = EventMessage.Deserialize(message);

                Console.WriteLine($"Consuming: id: {deserializedMessage.Id} calculatedValue: {deserializedMessage.CalculatedValue} from subject {eventArgs.RoutingKey}");
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            };

            return await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
        }

        public async Task StopConsumerAsync( string consumerTag )
        {
            await _channel.BasicCancelAsync(consumerTag);
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }

        private static async Task DeclareTopologyAsync( IChannel channel, string exchangeName, string queueName)
        {

            // Объявляем обменник для событий
            await channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );

            // Объявляем очередь
            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // Привязываем очередь к обменнику с двумя routing keys
            await channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "log.*"
            );
        }
    }
}
