using RankCalculator.Services;

class Program
{
    private const string ConsumerExchangeName = "valuator.processing.rank";
    private const string ProducerExchangeName = "event";
    private const string ProducerRoutingKey = "log.RankCalculated";

    private const string HostName = "localhost:6379";
    private const string RabbitHostName = "localhost";

    static async Task Main( string[] args )
    {
        var rabbitConsumerService = new ConsumerRabbitMQService(RabbitHostName, ConsumerExchangeName);
        var rabbitMQProducerService = new ProducerRabbitMQService(RabbitHostName, ProducerExchangeName, ProducerRoutingKey);
        var redisService = new RedisService(HostName);
        var rankCalculator = new RankService();

        await rabbitConsumerService.InitializeAsync();
        await rabbitMQProducerService.InitializeAsync();
        string consumerTag = await rabbitConsumerService.RunConsumerAsync(async ( id ) =>
        {
            string text = await redisService.GetText(id);
            double rank = rankCalculator.Calculate(text);
            await redisService.SetRank(id, rank);

            await rabbitMQProducerService.SendMessage(id, rank);
        });

        Console.WriteLine("RankCalculated started");
        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();

        await rabbitConsumerService.StopConsumerAsync(consumerTag);
    }
}
