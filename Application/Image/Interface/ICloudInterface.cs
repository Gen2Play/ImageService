using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Application.Image.Interface;

public interface ICloudInterface
{
    Task<ImageUploadResult> UploadImageAsync(IFormFile file);
    Task<DeletionResult> DeletionAsync(string publicID);
    Task<RestoreResult> RestoreAsync(string publicID);
}
