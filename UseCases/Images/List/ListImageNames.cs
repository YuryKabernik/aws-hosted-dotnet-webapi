using Storage.Buckets;

namespace UseCases.Images.List;

public class ListImagesByName(ImagesBucket bucket)
{
    public async Task<IList<string>> Handle(CancellationToken cancellation) => await bucket.ListImageNamesAsync(cancellation);
}
