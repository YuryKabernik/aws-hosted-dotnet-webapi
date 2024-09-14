using Amazon.S3;
using Amazon.S3.Model;
using dotnet_intermediate_mentoring_program.Infrastructure;
using dotnet_intermediate_mentoring_program.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dotnet_intermediate_mentoring_program.Images;

[Route("images")]
[ApiController]
public class ImagesController(
    ImagesDbContext imagesDbContext,
    IAmazonS3 simpleStorage,
    IOptions<BucketOptions> bucketOptions
) : ControllerBase
{
    private readonly BucketOptions _bucket = bucketOptions.Value;

    [HttpGet]
    public async Task<ActionResult> ListAll()
    {
        IList<string> keys = await simpleStorage.GetAllObjectKeysAsync(_bucket.Name, null, null);

        return this.Ok(keys);
    }

    /// <summary>
    /// download an image by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("{name}")]
    public async Task<ActionResult> Download(string name)
    {
        using GetObjectResponse objectResponse = await simpleStorage.GetObjectAsync(this._bucket.Name, name);
        await using var objectStream = objectResponse.ResponseStream;

        if (objectStream is not null)
        {
            return File(objectStream, objectResponse.Headers.ContentType, name);
        }

        return NotFound();
    }

    /// <summary>
    /// show metadata for the existing image by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("metadata/{name}")]
    public async Task<ActionResult> GetMetadata(string name)
    {
        var imageMetadata = await imagesDbContext.Images.FindAsync(name);

        if (imageMetadata is not null)
        {
            return Ok(imageMetadata);
        }

        return NotFound();
    }

    /// <summary>
    /// show metadata for a random image
    /// </summary>
    /// <returns></returns>
    [HttpGet("metadata/random")]
    public async Task<ActionResult> GetRandomMetadata()
    {
        var imagesMetadata = await imagesDbContext.Images.ToListAsync();

        if (imagesMetadata.Any())
        {
            var index = Random.Shared.Next(0, imagesMetadata.Count);

            return Ok(imagesMetadata[index]);
        }

        return NotFound();
    }

    /// <summary>
    /// delete an image by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<ActionResult> Delete(string name)
    {
        await simpleStorage.DeleteObjectAsync(this._bucket.Name, name);

        var imageMetadata = await imagesDbContext.Images.FindAsync(name);

        if (imageMetadata is not null)
        {
            imagesDbContext.Remove(imageMetadata);
            await imagesDbContext.SaveChangesAsync();
        }

        return NoContent();
    }
}