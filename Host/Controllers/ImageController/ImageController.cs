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

    [HttpPost("get-by-creator")]
    public async Task<Response> GetByCreatorAsync([FromBody] PaginationFilter request)
    {
        return await _imageService.GetPersonalImageAsync(request);
    }

    [HttpPost("get-by-collection")]
    public async Task<Response> GetByCollectionAsync([FromBody] PaginationFilter request)
    {
        return await _imageService.GetImageInCollectionAsync(request);
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

    [HttpPost("get-favorite-image")]
    public async Task<Response> GetfavoriteImageAsync([FromBody] PaginationFilter request)
    {
        return await _imageService.GetFavoriteImageAsync(request);
    }

    [HttpPost("get-by-type")]
    public async Task<Response> GetImageByTypeAsync([FromBody] PaginationFilter request)
    {
        return await _imageService.GetImageByTypeAsync(request);
    }

    [HttpPost("getall")]
    public async Task<Response> GetAllAsync([FromBody] PaginationFilter request)
    {
        return await _imageService.GetAllImageImageAsync(request);
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
}
