﻿using RankCalculator.Services;

class Program
{
    private const string ConsumerExchangeName = "valuator.processing.rank";
    private const string ConsumerQueueName = "valuator.message";
    private const string ConsumerRoutingKey = "valuator.CalculateRank";

    private const string ProducerExchangeName = "event";
    private const string ProducerRoutingKey = "log.RankCalculated";

    private const string HostName = "localhost:6379";
    private const string RabbitHostName = "localhost";

    private const string SignalRConnectionUrl = "http://localhost:5005/hub";

    static async Task Main( string[] args )
    {
        var rabbitConsumerService = new ConsumerRabbitMQService(RabbitHostName, ConsumerExchangeName, ConsumerQueueName, ConsumerRoutingKey);
        var rabbitMQProducerService = new ProducerRabbitMQService(RabbitHostName, ProducerExchangeName, ProducerRoutingKey);
        var redisService = new RedisService(HostName);
        var rankCalculator = new RankService();
        var signalRClientService = new SignalRClientService();

        await rabbitConsumerService.InitializeAsync();
        string consumerTag = await rabbitConsumerService.RunConsumerAsync(async ( id ) =>
        {
            var t = Task.Run(async delegate
            {
                TimeSpan interval = TimeSpan.FromSeconds(new Random().Next(3, 5)); 
                Console.WriteLine($"Waiting {interval}");

                await Task.Delay(interval);
            });
            t.Wait();
            string text = await redisService.GetText(id);
            double rank = rankCalculator.Calculate(text);
            await redisService.SetRank(id, rank);
            await signalRClientService.SendMessage(SignalRConnectionUrl, id, rank.ToString());

            await rabbitMQProducerService.InitializeAsync();
            await rabbitMQProducerService.SendMessage(id, rank);
        });

        Console.WriteLine("RankCalculated started");
        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();

        await rabbitConsumerService.StopConsumerAsync(consumerTag);
    }
}
