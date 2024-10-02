using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using Amazon.SimpleNotificationService;
using AsyncMessaging.Notifications;
using AsyncMessaging.Options;
using Microsoft.Extensions.Options;

var topicOptions = new SnsTopicOptions
{
    ArnName = Environment.GetEnvironmentVariable(SnsTopicOptions.EnvironmentArnKey),
};

// Services required to run Lambda
using IAmazonSimpleNotificationService client = new AmazonSimpleNotificationServiceClient();
var notificationService = new EmailNotificationService(client, Options.Create(topicOptions));

// The function handler that will be called for each Lambda event
async Task Handler(SQSEvent input, ILambdaContext context)
{
    var messages = input.Records.Select(record =>
        JsonSerializer.Deserialize<EmailNotificationMessage>(record.Body, JsonSerializerOptions.Default)!);

    foreach (var message in messages)
    {
        await notificationService.PushNotificationAsync(message, CancellationToken.None);
    }
}

var functionHandler = Handler;

// Build the Lambda runtime client passing in the handler to call for each
// event and the JSON serializer to use for translating Lambda JSON documents
// to .NET types.
await LambdaBootstrapBuilder
    .Create(functionHandler, new DefaultLambdaJsonSerializer())
    .Build()
    .RunAsync();