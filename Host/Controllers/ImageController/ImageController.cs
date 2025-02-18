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

    [HttpPost("upload")]
    public Task<Response> UploadImageAsync([FromForm] UploadImageRequest request)
    {
        return _imageService.UploadImageAsync(request);
    }
}
