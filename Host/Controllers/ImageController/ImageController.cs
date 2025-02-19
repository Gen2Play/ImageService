using Application.Common;
using Application.Image;
using Application.Image.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Responses;

namespace Host.Controllers.ImageController;

[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly IImageService _imageService;
    private readonly ILogger<ImageController> _logger;

    public ImageController(IImageService imageService, ILogger<ImageController> logger)
    {
        _imageService = imageService;
        _logger = logger;
    }

    [HttpPost("getall")]
    public async Task<Response> GetAllImageAsync([FromBody] PaginationFilter request)
    {
        return await _imageService.GetAllImageAsync(request);
    }

    [HttpPost("get-by-collection/user={userID:Guid}/collection={collectionID:Guid}")]
    public async Task<Response> GetByCollectionAsync([FromBody] PaginationFilter request, Guid userID, Guid collectionID)
    {
        return await _imageService.GetImageInCollectionAsync(request, userID, collectionID);
    }

    [HttpGet("type")]
    public async Task<Response> GetTypeAsync()
    {
        return await _imageService.GetTypeAsync();
    }

    [HttpGet("get/{id:Guid}")]
    public async Task<Response> GetImageByIDAsync(Guid id)
    {
        return await _imageService.GetImageByIDAsync(id);
    }

    [HttpPost("get-favorite-image/user={userID:guid}")]
    public async Task<Response> GetfavoriteImageAsync([FromBody] PaginationFilter request, Guid userID)
    {
        return await _imageService.GetFavoriteImageAsync(request, userID);
    }
    [HttpPost("upload")]
    public async Task<Response> UploadImageAsync([FromForm] UploadImageRequest request)
    {
        return await _imageService.UploadImageAsync(request);
    }

    [HttpDelete("personal/delete")]
    public async Task<Response> DeleteImageAsync([FromBody] DeleteImageRequest request)
    {
        return await _imageService.DeletePersonalImageAsync(request);
    }

    [HttpPost("tag/getall")]
    public async Task<Response> GetAllTagAsync([FromBody] PaginationFilter request)
    {
        return await _imageService.GetAllTagsync(request);
    }

    [HttpPost("collection/create")]
    public async Task<Response> CreateCollectorAsync([FromBody] CreateCollectorRequest request)
    {
        return await _imageService.CreateCollectorAsync(request);
    }

    [HttpGet("collection/user={id:guid}")]
    public async Task<Response> GetCollectorAsync(Guid id)
    {
        return await _imageService.GetCollectorAsync(id);
    }

    [HttpGet("collection/id={id:guid}")]
    public async Task<Response> GetCollectorByIDAsync(Guid id)
    {
        return await _imageService.GetCollectorByIDAsync(id);
    }

    [HttpDelete("collection={id:guid}/remove/image={image:guid}")]
    public async Task<Response> RemoveImageFromCollectorAsync(Guid id, Guid image)
    {
        return await _imageService.RemoveImageFromCollectorAsync(id, image);
    }
}
