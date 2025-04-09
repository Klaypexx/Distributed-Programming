using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RankCalculator.Services
{
    public class ConsumerRabbitMQService
    {
        private string QueueName { get; set; }

        private IConnection _connection;
        private IChannel _channel;
        private readonly ConnectionFactory _factory;

        public ConsumerRabbitMQService(string hostName)
        {
            _factory = new ConnectionFactory { HostName = hostName };
        }

        public async Task InitializeAsync(string exchangeName)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await DeclareTopologyAsync(_channel, exchangeName);
        }

        public async Task<string> RunConsumerAsync(Func<string, Task> onMessageReceived)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                Console.WriteLine($"Consuming: {message} from subject {eventArgs.Exchange}");
                await onMessageReceived(message);
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            };

            return await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer);
        }

        public async Task StopConsumerAsync(string consumerTag)
        {
            await _channel.BasicCancelAsync(consumerTag);
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }

        private async Task DeclareTopologyAsync(IChannel channel, string exchangeName)
        {
            await channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Fanout
            );
            QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
            QueueName = queueDeclareResult.QueueName;
            await channel.QueueBindAsync(
                queue: QueueName,
                exchange: exchangeName,
                routingKey: string.Empty
            );
        }
    }
}
