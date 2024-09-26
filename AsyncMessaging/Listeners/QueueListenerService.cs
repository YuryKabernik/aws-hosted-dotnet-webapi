using AsyncMessaging.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AsyncMessaging.Listeners;

public class QueueListenerService(
    IServiceProvider services,
    ILogger<QueueListenerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = services.CreateScope();
        var queueService = scope.ServiceProvider.GetRequiredService<QueueService>();
        var notificationService = scope.ServiceProvider.GetRequiredService<EmailNotificationService>();

        await PingQueue(queueService, notificationService, stoppingToken);
    }

    private async Task PingQueue(QueueService queueService, EmailNotificationService notificationService, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await foreach (var message in queueService.Read(stoppingToken))
                {
                    await notificationService.PushNotificationAsync(message, stoppingToken);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occured while trying to Read the queue and Push the notification.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}