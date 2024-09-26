using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using UseCases.Images.Delete;
using UseCases.Images.Download;
using UseCases.Images.List;
using UseCases.Images.Metadata;
using UseCases.Images.Upload;

namespace dotnet_intermediate_mentoring_program.Endpoints.Images;

[Route("images")]
[ApiController]
public class ImagesController(
    UploadImage uploadImage,
    ListImagesByName imagesByName,
    DownloadImage downloadImage,
    MetadataByName metadataByName,
    MetadataRandom metadataRandom,
    DeleteImageByName deleteImageByName,
    LinkGenerator linkGenerator
    ) : ControllerBase
{
    /// <summary>
    /// save image to the bucket and metadata to the RDS service
    /// </summary>
    /// <param name="formFile"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    public async Task<ActionResult> Save(IFormFile formFile, CancellationToken cancellation)
    {
        if (formFile is not null && ModelState.IsValid)
        {
            using var stream = formFile.OpenReadStream();

            string? resource = linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(Download),
                values: new { name = formFile.FileName });

            if (Uri.TryCreate(resource, UriKind.Absolute, out Uri? address))
            {
                FileData file = new(formFile.FileName, formFile.ContentType, stream, address);

                await uploadImage.HandleAsync(file, cancellation);

                return Created(address, new { name = file.Name, resourse = address });
            }
        }

        return BadRequest();
    }

    /// <summary>
    /// list all available images by name
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult> ListAll(CancellationToken cancellation)
    {
        IList<string> names = await imagesByName.Handle(cancellation);

        if (names is not null)
        {
            return Ok(names);
        }

        return NoContent();
    }

    /// <summary>
    /// download an image by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("{name}")]
    public async Task<ActionResult> Download(string name, CancellationToken cancellation)
    {
        using var image = await downloadImage.HandleAsync(name, cancellation);

        if (image is not null && image.Stream is not null)
        {
            return File(image.Stream, image.ContentType, name);
        }

        return NotFound();
    }

    /// <summary>
    /// show metadata for the existing image by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("metadata/{name}")]
    public async Task<ActionResult> GetMetadata(string name, CancellationToken cancellation)
    {
        var imageMetadata = await metadataByName.HandleAsync(name, cancellation);

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
    public async Task<ActionResult> GetRandomMetadata(CancellationToken cancellation)
    {
        var metadata = await metadataRandom.HandleAsync(cancellation);

        if (metadata is not null)
        {
            return Ok(metadata);
        }

        return NotFound();
    }

    /// <summary>
    /// delete an image by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<ActionResult> Delete(string name, CancellationToken cancellation)
    {
        try
        {
            await deleteImageByName.HandleAsync(name, cancellation);
        }
        catch
        {
            return BadRequest();
        }

        return NoContent();
    }
}
