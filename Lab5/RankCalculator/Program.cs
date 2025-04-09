using RankCalculator.Services;

class Program
{
    private const string ConsumerExchangeName = "valuator.processing.rank";
    private const string ProducerExchangeName = "event";
    private const string ProducerRoutingKey = "log.RankCalculated";

    private const string HostName = "localhost:6379";
    private const string RabbitHostName = "localhost";

    private const string SignalHubUrl = "http://localhost:6001/processing-hubb";

    static async Task Main( string[] args )
    {
        var rabbitConsumerService = new ConsumerRabbitMQService(RabbitHostName);
        var rabbitMQProducerService = new ProducerRabbitMQService(RabbitHostName);
        var redisService = new RedisService(HostName);
        var rankCalculator = new RankService();
        var signarRService = new SignarRService(SignalHubUrl);

        await rabbitConsumerService.InitializeAsync(ConsumerExchangeName);
        await rabbitMQProducerService.InitializeAsync(ProducerExchangeName);
        string consumerTag = await rabbitConsumerService.RunConsumerAsync(async ( id ) =>
        {
            string text = await redisService.GetTextAsync(id);
            double rank = rankCalculator.Calculate(text);
            await redisService.SetRankAsync(id, rank);

            await signarRService.SendMessageAsync(rank.ToString());

            await rabbitMQProducerService.SendMessage(ProducerExchangeName, ProducerRoutingKey, id, rank);
        });

        Console.WriteLine("RankCalculated started");
        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();

        await rabbitConsumerService.StopConsumerAsync(consumerTag);
    }
}
