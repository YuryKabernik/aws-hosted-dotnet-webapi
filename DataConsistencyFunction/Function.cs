using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Storage.DbContexts;
using Storage.Options;

var connectionString =
    Environment.GetEnvironmentVariable("Postgres") ??
    throw new InvalidOperationException("Connection string to the Postgres database can't be read.");

var optionsBuilder = new DbContextOptionsBuilder<ImagesDbContext>()
    .UseNpgsql(connectionString);

ImagesDbContext dbContext = new ImagesDbContext(optionsBuilder.Options);

var bucketName = Environment.GetEnvironmentVariable(BucketOptions.EnvironmentKey) ??
                 throw new InvalidOperationException(
                     $"The bucket name is missing by key {BucketOptions.EnvironmentKey}.");

IAmazonS3 amazonS3 = new AmazonS3Client();

// The function handler that will be called for each Lambda event
// Some basic validation that verifies that DB metadata corresponds to the images in the S3 bucket.
// The lambda should return a result that shows if data is consistent or not.
async Task<dynamic> Handler(string input, ILambdaContext context)
{
    var imagesCount = await dbContext.Images.CountAsync();
    var objectKeys = await amazonS3.GetAllObjectKeysAsync(bucketName, null, null);

    if (imagesCount == objectKeys.Count)
    {
        return new { StatusCode = HttpStatusCode.OK };
    }

    context.Logger.LogCritical("The number of images in RDS is different from S3 bucket keys.");

    return new { StatusCode = HttpStatusCode.InternalServerError };
}

var handler = Handler;

// Build the Lambda runtime client passing in the handler to call for each
// event and the JSON serializer to use for translating Lambda JSON documents
// to .NET types.
await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
    .Build()
    .RunAsync()
    .ContinueWith(async _ =>
    {
        await dbContext.DisposeAsync();
        amazonS3.Dispose();
    });