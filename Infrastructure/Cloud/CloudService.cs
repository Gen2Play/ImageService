﻿using Application.Image.Interface;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Cloud;
using Shared.Logs;
using SixLabors.ImageSharp;

namespace Infrastructure.Cloud;

public class CloudService : ICloudInterface
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<CloudService> _logger;
    public CloudService(IOptions<CloudinarySetting> option, ILogger<CloudService> logger)
    {
        var acc = new Account(
            option.Value.CloudName,
            option.Value.ApiKey,
            option.Value.ApiSecret
            );
        _cloudinary = new Cloudinary(acc);
        _logger = logger;
    }

    public async Task<DeletionResult> DeletionAsync(string publicID)
    {
        var delete = new DeletionResult();
        try
        {
            var deleteParams = new DelResParams()
            {
                PublicIds = new List<string> { publicID },
                Type = "upload",
                ResourceType = ResourceType.Image
            };
            var result = _cloudinary.DeleteResources(deleteParams);
            _logger.LogInformation($"Delete Image {publicID}");
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
        }
        return delete;
    }

    public Task<RestoreResult> RestoreAsync(string publicID)
    {
        try
        {
            var restoreParams = new RestoreParams() { 
                PublicIds = new List<string> { publicID },
                ResourceType= ResourceType.Image,
                Type = AssetType.Upload                
            };
            var restoreResult = _cloudinary.RestoreAsync(restoreParams);

            _logger.LogInformation($"Restore Image {publicID}");
            return restoreResult;
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            throw;
        }
    }

    public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
    {
        var upload = new ImageUploadResult();
        try
        {
            if (file.Length > 0) { 
                await using var stream = file.OpenReadStream();
                using var image = Image.Load(stream);
                stream.Position = 0;
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(image.Height).Width(image.Width).Crop("fill").Gravity("face")
                };
                upload = await _cloudinary.UploadAsync(uploadParams);
            }
        }
        catch (Exception ex) { 
            LogException.LogExceptions(ex, ex.Message);
        }
        return upload;
    }
}
