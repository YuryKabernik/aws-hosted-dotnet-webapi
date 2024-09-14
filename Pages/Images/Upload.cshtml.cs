using Amazon.S3;
using dotnet_intermediate_mentoring_program.Images.Responses;
using dotnet_intermediate_mentoring_program.Infrastructure;
using dotnet_intermediate_mentoring_program.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace dotnet_intermediate_mentoring_program.Pages.Images;

public class UploadModel(
    IAmazonS3 simpleStorage,
    ImagesDbContext imagesDbContext,
    IOptions<BucketOptions> bucket)
    : PageModel
{
    private readonly BucketOptions _bucket = bucket.Value;

    /// <summary>
    /// A file uploaded to the platform.
    /// </summary>
    [FromForm]
    public IFormFile? FormFile { get; set; }

    /// <summary>
    /// Render upload form.
    /// </summary>
    public void OnGet()
    {
    }

    /// <summary>
    /// Receive a file to upload.
    /// </summary>
    public async Task OnPostUploadAsync()
    {
        if (this.ModelState.IsValid)
        {
            await this.SaveImage();
            await this.ProduceMetadata();
        }
    }

    private async Task ProduceMetadata()
    {
        if (FormFile != null)
        {
            var metadataResponse = await simpleStorage.GetObjectMetadataAsync(_bucket.Name, FormFile.FileName);

            if (metadataResponse != null)
            {
                ImageMetadata metadata = new ImageMetadata
                {
                    Name = FormFile.FileName,
                    Size = metadataResponse.ContentLength,
                    Extension = FormFile.ContentType,
                    LastUpdate = metadataResponse.LastModified
                };

                imagesDbContext.Images.Add(metadata);
            }
        }

        await imagesDbContext.SaveChangesAsync();
    }

    private async Task SaveImage()
    {
        if (FormFile != null)
        {
            await using Stream readStream = FormFile.OpenReadStream();
            await simpleStorage.UploadObjectFromStreamAsync(_bucket.Name, FormFile.FileName, readStream, null);
        }
    }
}