using AsyncMessaging.Notifications;

namespace AsyncMessaging.Producers;

public class ImageUploadedProducer(QueueService queueService)
{
    public async Task SendNotification(EmailNotificationMessage notificationMessage, CancellationToken cancellationToken)
    {
        await queueService.Send(notificationMessage, cancellationToken);
    }
}
