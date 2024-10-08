using System.Net.Mime;
using Storage.Buckets;
using Storage.DbContexts;

namespace UseCases.Images.Download;

public record ImageToDownload(Stream Stream, string ContentType) : IDisposable
{
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this.Stream?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class DownloadImage(ImagesBucket bucket, ImagesDbContext context)
{
    private readonly ImagesBucket _bucket = bucket;

    public async Task<ImageToDownload> HandleAsync(string name, CancellationToken cancellation)
    {
        var imageData = await _bucket.GetImageAsync(name, cancellation);
        var imageMetadata = await context.Images.FindAsync([name], cancellation);

        return new ImageToDownload(imageData, imageMetadata?.Extension ?? MediaTypeNames.Image.Png);
    }
}
