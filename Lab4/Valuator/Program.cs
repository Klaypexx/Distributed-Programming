using Valuator.Services;

namespace Valuator;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");

        builder.Services.AddSingleton<IRedisService>(provider => new RedisService(redisConnectionString!));

        var rabbitSection = builder.Configuration.GetSection("RabbitMQ");
        var hostName = rabbitSection.GetValue<string>("HostName");

        var messageExchangeName = rabbitSection.GetValue<string>("MessageExchangeName");

        var eventExchangeName = rabbitSection.GetValue<string>("EventExchangeName");
        var eventRoutingKey = rabbitSection.GetValue<string>("EventRoutingKey");

        builder.Services.AddSingleton<IRabbitMqService>(provider => new ProducerRabbitMqService(messageExchangeName!, eventExchangeName!, eventRoutingKey!, hostName!));

        builder.Services.AddRazorPages();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}


