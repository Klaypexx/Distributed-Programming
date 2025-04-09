using SignalRHub.Services;

namespace SignalRHub;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.SetIsOriginAllowed(origin =>
                        new Uri(origin).Host == "localhost") // Разрешить все localhost-порты
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // Регистрируем SignalR
        builder.Services.AddSignalR();

        var app = builder.Build();

        app.UseCors("CorsPolicy");

        // Регистрируем конечную точку хаба
        app.MapHub<NotificationHubService>("/hub");

        app.Run();
    }
}