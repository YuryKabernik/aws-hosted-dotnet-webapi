using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.Serialization.SystemTextJson;

// The function handler that will be called for each Lambda event
var handler = (S3Event inputEvent, ILambdaContext context) =>
{
    var records = inputEvent.Records ?? new List<S3Event.S3EventNotificationRecord>();

    foreach (var record in records)
    {
        if (record.S3 == null)
        {
            context.Logger.LogLine($"S3 property in an S3Event record is missing.");
            continue;
        }

        var recordEventName = record.EventName;
        var objectKey = record.S3.Object.Key;
        var bucketName = record.S3.Bucket.Name;

        context.Logger.LogLine(
            $"S3Logger has received '{recordEventName}' event about created '{objectKey}' in '{bucketName}'.");
    }
};

// Build the Lambda runtime client passing in the handler to call for each
// event and the JSON serializer to use for translating Lambda JSON documents
// to .NET types.
await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
    .Build()
    .RunAsync();