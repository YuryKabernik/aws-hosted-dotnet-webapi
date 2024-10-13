using Microsoft.EntityFrameworkCore;
using Storage.DbContexts;
using UseCases.Images.Metadata.Responses;

namespace UseCases.Images.Metadata;

public class MetadataRandom(ImagesDbContext context)
{
    private readonly ImagesDbContext context = context;

    public async Task<MetadataResponse?> HandleAsync(CancellationToken cancellation)
    {
        var imagesMetadataCount = await context.Images.CountAsync(cancellation);

        if (imagesMetadataCount <= 0)
        {
            return null;
        }

        var lastIndex = imagesMetadataCount - 1;
        var randonIndex = Random.Shared.Next(0, lastIndex);
        var imageMetadata = await context.Images.ElementAtAsync(randonIndex, cancellation);

        return new MetadataResponse(
            imageMetadata.Name,
            imageMetadata.Size,
            imageMetadata.Extension,
            imageMetadata.LastUpdate
        );
    }
}
