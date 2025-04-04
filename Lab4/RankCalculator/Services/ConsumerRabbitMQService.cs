using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RankCalculator.Services
{
    public class ConsumerRabbitMQService
    {
        private IConnection _connection;
        private IChannel _channel;
        private string _queueName;
        private readonly string _exchangeName;
        private readonly ConnectionFactory _factory;

        public ConsumerRabbitMQService( string hostName, string exchangeName )
        {
            _factory = new ConnectionFactory { HostName = hostName };
            _exchangeName = exchangeName;
        }

        public async Task InitializeAsync()
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await DeclareTopologyAsync(_channel, _exchangeName);
        }

        public async Task<string> RunConsumerAsync( Func<string, Task> onMessageReceived )
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async ( _, eventArgs ) =>
            {
                string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                Console.WriteLine($"Consuming: {message} from subject {eventArgs.Exchange}");
                await onMessageReceived(message);
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            };

            return await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);
        }

        public async Task StopConsumerAsync( string consumerTag )
        {
            await _channel.BasicCancelAsync(consumerTag);
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }

        private async Task DeclareTopologyAsync( IChannel channel, string exchangeName )
        {
            await channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Fanout
            );
            QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
            _queueName = queueDeclareResult.QueueName;
            await channel.QueueBindAsync(
                queue: _queueName,
                exchange: exchangeName,
                routingKey: string.Empty
            );
        }
    }
}
