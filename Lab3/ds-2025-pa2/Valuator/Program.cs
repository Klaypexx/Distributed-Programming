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
        var queueName = rabbitSection.GetValue<string>("QueueName");

        builder.Services.AddSingleton<IRabbitMqService>(provider => new RabbitMqService(queueName!, hostName!));

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
