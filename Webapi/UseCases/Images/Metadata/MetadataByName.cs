using Storage.DbContexts;
using UseCases.Images.Metadata.Responses;

namespace UseCases.Images.Metadata;

public class MetadataByName(ImagesDbContext context)
{
    public async Task<MetadataResponse?> HandleAsync(string name, CancellationToken cancellation)
    {
        var imageMetadata = await context.Images.FindAsync([name], cancellation);

        if (imageMetadata == null)
        {
            return null;
        }

        return new MetadataResponse(
            imageMetadata.Name,
            imageMetadata.Size,
            imageMetadata.Extension,
            imageMetadata.LastUpdate
        );
    }
}
