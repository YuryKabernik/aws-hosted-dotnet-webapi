using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Storage.Options;

namespace Storage.Buckets;

public record ImageObjectMetadata(long ContentLength, DateTime LastModified);

public class ImagesBucket(IAmazonS3 storage, IOptions<BucketOptions> options)
{
    private readonly BucketOptions _bucket = options.Value;

    private readonly IAmazonS3 storage = storage;

    public async Task SaveAsync(string objectKey, Stream stream, CancellationToken cancellation)
    {
        await storage.UploadObjectFromStreamAsync(_bucket.Name, objectKey, stream, null, cancellation);
    }

    public async Task<ImageObjectMetadata> ScrapMetadataAsync(string objectKey, CancellationToken cancellation)
    {
        var metadataResponse = await storage.GetObjectMetadataAsync(_bucket.Name, objectKey, cancellation);

        return new ImageObjectMetadata(metadataResponse.ContentLength, metadataResponse.LastModified);
    }

    public async Task RemoveAsync(string objectKey, CancellationToken cancellation)
    {
        await storage.DeleteObjectAsync(_bucket.Name, objectKey, null, cancellation);
    }

    public async Task<IList<string>> ListImageNamesAsync(CancellationToken cancellation)
    {
        cancellation.ThrowIfCancellationRequested();

        var names = await storage.GetAllObjectKeysAsync(_bucket.Name, null, null);

        cancellation.ThrowIfCancellationRequested();

        return names;
    }

    public async Task<Stream> GetImageAsync(string name, CancellationToken cancellation)
    {
        GetObjectResponse objectResponse = await storage.GetObjectAsync(_bucket.Name, name, cancellation);

        return objectResponse.ResponseStream;
    }
}
