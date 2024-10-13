using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Buckets;
using Storage.DbContexts;
using Storage.Options;

namespace Storage;

public static class ServiceRegister
{
    public static IServiceCollection RegisterStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNpgsql<ImagesDbContext>(configuration.GetConnectionString("Postgres"));
        services.Configure<BucketOptions>(configuration.GetSection(BucketOptions.SectionKey));
        
        services.AddAWSService<IAmazonS3>();
        services.AddScoped<ImagesBucket>();

        return services;
    }
}
