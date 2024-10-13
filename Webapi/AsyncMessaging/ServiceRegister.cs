using Amazon.SimpleNotificationService;
using Amazon.SQS;
using AsyncMessaging.Notifications;
using AsyncMessaging.Options;
using AsyncMessaging.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AsyncMessaging;

public static class ServiceRegister
{
    public static IServiceCollection RegisterMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SnsTopicOptions>(configuration.GetSection(SnsTopicOptions.SectionKey));
        services.Configure<QueueOptions>(configuration.GetSection(QueueOptions.SectionKey));

        services.AddAWSService<IAmazonSimpleNotificationService>();
        services.AddAWSService<IAmazonSQS>();
        
        services.TryAddScoped<EmailNotificationService>();
        services.TryAddScoped<ImageUploadedProducer>();
        services.TryAddScoped<QueueService>();
        
        return services;
    }
}