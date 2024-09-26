using AsyncMessaging.Notifications;
using AsyncMessaging.Producers;
using Microsoft.Extensions.Logging;
using Storage.Buckets;
using Storage.DbContexts;
using Storage.DbModels;
using UseCases.AutoMappers;

namespace UseCases.Images.Upload;

public record class FileData(string Name, string ContentType, Stream Stream, Uri Address);

public class UploadImage(
    ImagesBucket bucket,
    ImagesDbContext context,
    ImageUploadedProducer publisher,
    ILogger<UploadImage> logger)
{
    public async Task HandleAsync(FileData file, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(file, nameof(file));

        try
        {
            await this.SaveImageAsync(file, cancellation);
            await this.ProduceMetadataAsync(file, cancellation);
        }
        catch
        {
            await this.TryRemoveImageAsync(file, cancellation);

            throw;
        }
    }

    private async Task SaveImageAsync(FileData file, CancellationToken cancellation) =>
        await bucket.SaveAsync(file.Name, file.Stream, cancellation);

    private async Task ProduceMetadataAsync(FileData file, CancellationToken cancellation)
    {
        var metadataResponse = await bucket.ScrapMetadataAsync(file.Name, cancellation);

        if (metadataResponse is not null)
        {
            ImageMetadata metadata = new()
            {
                Name = file.Name,
                Size = metadataResponse.ContentLength,
                Extension = file.ContentType,
                LastUpdate = metadataResponse.LastModified
            };

            context.Images.Add(metadata);

            await context.SaveChangesAsync(cancellation);
            await this.NotifyImageSubmittedAsync(file, metadata, cancellation);
        }
    }

    private async Task NotifyImageSubmittedAsync(FileData file, ImageMetadata metadata, CancellationToken cancellation)
    {
        EmailNotificationMessage notificationMessage = new(metadata.ToNotify(), file.Address);

        await publisher.SendNotification(notificationMessage, cancellation);
    }

    private async Task TryRemoveImageAsync(FileData file, CancellationToken cancellation)
    {
        try
        {
            await bucket.RemoveAsync(file.Name, cancellation);
        }
        catch (Exception exception)
        {
            logger.LogInformation(exception, "Unable to rollback the image '{0}' from the bucket.", file.Name);
        }
    }
}
