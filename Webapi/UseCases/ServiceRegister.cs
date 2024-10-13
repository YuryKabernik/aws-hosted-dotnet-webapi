using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Storage;
using UseCases.Images.Delete;
using UseCases.Images.Download;
using UseCases.Images.List;
using UseCases.Images.Metadata;
using UseCases.Images.Upload;

namespace UseCases;

public static class ServiceRegister
{
    public static IServiceCollection RegisterUseCases(this IServiceCollection service, IConfiguration configuration)
    {
        service.RegisterStorage(configuration);

        service.TryAddScoped<UploadImage>();
        service.TryAddScoped<DownloadImage>();
        service.TryAddScoped<ListImagesByName>();
        service.TryAddScoped<MetadataByName>();
        service.TryAddScoped<MetadataRandom>();
        service.TryAddScoped<DeleteImageByName>();

        return service;
    }
}
