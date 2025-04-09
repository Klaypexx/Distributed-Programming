using EventLogger.Services;

namespace EventsLogger
{
    public class Consumer
    {
        private const string HostName = "localhost";
        private const string ExchangeName = "event";
        private const string QueueName = "valuator.events";

        public static async Task Main( string[] args )
        {
            var rabbitConsumerService = new ConsumerRabbitMQService(HostName, QueueName, ExchangeName);

            await rabbitConsumerService.InitializeAsync();
            string consumerTag = await rabbitConsumerService.RunConsumerAsync();

            Console.WriteLine("EventLogger started");
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            await rabbitConsumerService.StopConsumerAsync(consumerTag);
        }
    }
}
