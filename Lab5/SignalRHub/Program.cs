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
                        new Uri(origin).Host == "localhost") // ��������� ��� localhost-�����
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // ������������ SignalR
        builder.Services.AddSignalR();

        var app = builder.Build();

        app.UseCors("CorsPolicy");

        // ������������ �������� ����� ����
        app.MapHub<NotificationHubService>("/hub");

        app.Run();
    }
}