using Application.Image;
using Application.Image.Interface;
using CloudinaryDotNet.Actions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Logs;
using Shared.Responses;

namespace Infrastructure.Images;

public class ImageService : IImageService
{
    private readonly ICloudInterface _cloudInterface;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ImageService> _logger;
    private readonly ITagService _tagService;

    public ImageService(ICloudInterface cloudInterface, ApplicationDbContext context, ILogger<ImageService> logger, ITagService tagService)
    {
        _cloudInterface = cloudInterface;
        _context = context;
        _logger = logger;
        _tagService = tagService;
    }

    public Task<Response> GetPersonalImageAsync(Guid creatorID)
    {
        try
        {
            
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
        }
        throw new NotImplementedException();
    }

    public async Task<Response> UploadImageAsync(UploadImageRequest request)
    {
        var transaction = _context.Database.BeginTransaction();
        string publicid = "";
        try
        {
            var cloudUpload = await _cloudInterface.UploadImageAsync(request.Image);
            if (cloudUpload == null) {
                return new Response
                {
                    code = StatusCodes.Status400BadRequest,
                    data = null,
                    message = "Không thể upload ảnh"
                };
            }
            var type = await _context.Types.FirstOrDefaultAsync(b => b.Name == "Mới nhất");
            var image = new Image
            {
                CreatedBy = request.CreatorID,
                CreatorID = request.CreatorID,
                CreatedOn = DateTime.UtcNow,
                Description = request.Description,
                Download = 0,
                ImagePublicID = cloudUpload.PublicId,
                isAIGen = request.IsAiGen,
                Height = cloudUpload.Height,
                Link = cloudUpload.SecureUrl.OriginalString,
                Name = request.Image.Name,
                size = cloudUpload.Bytes,
                Width = cloudUpload.Width,
                Orientation = cloudUpload.Width > cloudUpload.Height ? Orientation.Horizontal : Orientation.Vertical,
                Status = ImageStatus.UnAccept,
                View = 0,
                TypeID = type.Id,
            };
            var entity = _context.Images.AddAsync(image).Result.Entity;
            publicid = image.ImagePublicID;
            foreach(var item in request.Tags)
            {
                var exist = await _tagService.TagExistAsync(item);
                if (exist)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(p => p.Name.Equals(item));
                    if (tag != null)
                    {
                        _context.ImageTags.AddAsync(
                            new ImageTag
                            {
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = entity.CreatedOn,
                                ImageID = entity.Id,
                                TagID = tag.Id,
                            }
                        );
                    }
                }
                else
                {
                    var tag = await _tagService.CreateTagAsync(item, request.CreatorID);
                    if (tag != null)
                    {
                        _context.ImageTags.AddAsync(
                            new ImageTag
                            {
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = entity.CreatedOn,
                                ImageID = entity.Id,
                                TagID = tag.Id,
                            }
                        );
                    }
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new Response
            {
                code = StatusCodes.Status201Created,
                data = null,
                message = "Upload ảnh thành công"
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            await transaction.RollbackAsync();
            await _cloudInterface.DeletionAsync(publicid);
            return new Response
            {
                code = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message
            };
        }
    }
}
