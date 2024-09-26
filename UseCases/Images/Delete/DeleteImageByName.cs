using Storage.Buckets;
using Storage.DbContexts;
using Storage.DbModels;

namespace UseCases.Images.Delete;

public class DeleteImageByName(ImagesDbContext context, ImagesBucket bucket)
{
    public async Task HandleAsync(string name, CancellationToken cancellation)
    {
        ImageMetadata? imageMetadata = null;

        try
        {
            imageMetadata = await context.Images.FindAsync([name], cancellation);

            if (imageMetadata is not null)
            {
                context.Images.Remove(imageMetadata);
                await context.SaveChangesAsync(cancellation);
            }

            await bucket.RemoveAsync(name, cancellation);
        }
        catch (System.Exception)
        {
            if (imageMetadata is not null)
            {
                await context.Images.AddAsync(imageMetadata, cancellation);
                await context.SaveChangesAsync(cancellation);
            }
            
            throw;
        }
    }
}
