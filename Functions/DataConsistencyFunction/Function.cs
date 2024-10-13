using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Runtime;
using Amazon.S3;
using DataConsistencyFunction.Commands;
using DataConsistencyFunction.Options;
using Npgsql;

var bucketName = Environment.GetEnvironmentVariable(BucketOptions.EnvironmentKey) ??
                 throw new InvalidOperationException(
                     $"The bucket name is missing by key '{BucketOptions.EnvironmentKey}'.");

IAmazonS3 amazonS3 = new AmazonS3Client(FallbackRegionFactory.GetRegionEndpoint());

var connectionString =
    Environment.GetEnvironmentVariable("Postgres") ??
    throw new InvalidOperationException("Connection string to the Postgres database can't be found.");

await using var dataSource = NpgsqlDataSource.Create(connectionString);

// Ensure table created
await using var cmd = dataSource.CreateCommand(SqlCommands.CreateIfNotExists);
await cmd.ExecuteNonQueryAsync();

// The function handler that will be called for each Lambda event
// Some basic validation that verifies that DB metadata corresponds to the images in the S3 bucket.
// The lambda should return a result that shows if data is consistent or not.
var handler = async (object request, ILambdaContext context) =>
{
    context.Logger.LogInformation($"Executing lambda function from {request}.");

    await using var command = dataSource.CreateCommand(SqlCommands.CountRows);

    var imagesCount = Convert.ToInt32(await command.ExecuteScalarAsync());
    var objectKeys = await amazonS3.GetAllObjectKeysAsync(bucketName, null, null);

    var isConsistent = imagesCount == objectKeys.Count;
    var responseMessage = isConsistent ? "Data consistency check: PASSED" : "Data consistency check: FAILED";

    if (!isConsistent)
        context.Logger.LogCritical("The number of images in RDS is different from S3 bucket keys.");

    return new APIGatewayProxyResponse
    {
        IsBase64Encoded = false,
        StatusCode = (int)HttpStatusCode.OK,
        Headers = new Dictionary<string, string>(),
        Body = responseMessage
    };
};

// Build the Lambda runtime client passing in the handler to call for each
// event and the JSON serializer to use for translating Lambda JSON documents
// to .NET types.
await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
    .Build()
    .RunAsync();