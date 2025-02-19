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

    [HttpPost("get-by-creator/{id:Guid}")]
    public async Task<Response> GetByCreatorAsync([FromBody] PaginationFilter request, Guid id)
    {
        return await _imageService.GetPersonalImageAsync(request, id);
    }

    [HttpPost("get-by-collection/{userID:Guid}/{collectionID:Guid}")]
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

    [HttpPost("get-favorite-image/{userID:guid}")]
    public async Task<Response> GetfavoriteImageAsync([FromBody] PaginationFilter request, Guid userID)
    {
        return await _imageService.GetFavoriteImageAsync(request, userID);
    }

    [HttpPost("get-by-type/{typeId:Guid}/{userID:Guid}")]
    public async Task<Response> GetImageByTypeAsync([FromBody] PaginationFilter request, Guid typeId, Guid userID)
    {
        return await _imageService.GetImageByTypeAsync(request, typeId, userID);
    }

    [HttpPost("getall/{userID:guid}")]
    public async Task<Response> GetAllAsync([FromBody] PaginationFilter request, Guid userID)
    {
        return await _imageService.GetAllImageImageAsync(request, userID);
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

    [HttpPost("get-by-tag/{id:guid}")]
    public async Task<Response> GetAllByTagAsync([FromBody] PaginationFilter request, Guid id)
    {
        return await _imageService.GetAllByTagAsync(request, id);
    }

    [HttpPost("collection/create")]
    public async Task<Response> CreateCollectorAsync([FromBody] CreateCollectorRequest request)
    {
        return await _imageService.CreateCollectorAsync(request);
    }

    [HttpGet("collection/get-by-userid/{id:guid}")]
    public async Task<Response> GetCollectorAsync(Guid id)
    {
        return await _imageService.GetCollectorAsync(id);
    }

    [HttpGet("collection/get-by-id/{id:guid}")]
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
