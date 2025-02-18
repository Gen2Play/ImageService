using Application.Common;
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
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

    public async Task<Response> DeletePersonalImageAsync(DeleteImageRequest request)
    {
        var transaction = _context.Database.BeginTransaction();
        try
        {
            var image = await _context.Images.FirstOrDefaultAsync(p => p.CreatorID == request.CreatorID && p.ImagePublicID == request.publicID);
            if (image == null) {
                return new Response
                {
                    code = StatusCodes.Status404NotFound,
                    data = null,
                    message = "Không thể tìm thấy ảnh"
                };
            }
            var cloudDelete = await _cloudInterface.DeletionAsync(image.ImagePublicID);
            if (cloudDelete.Error != null)
            {
                return new Response
                {
                    code = (int)cloudDelete.StatusCode,
                    data = null,
                    message = cloudDelete.Error.Message
                };
            }
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new Response
            {
                code = StatusCodes.Status200OK,
                data = null,
                message = "Xóa thành công"
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            await _cloudInterface.RestoreAsync(request.publicID);
            await transaction.RollbackAsync();
            return new Response
            {
                code = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<Response> GetAllImageImageAsync(PaginationFilter filter)
    {
        try
        {
            List<ImageListResponse> imageListResponses = new List<ImageListResponse>();
            var query = _context.Images.AsNoTracking();

            if (query.Any())
            {
                if (filter.UserID != default)
                {
                    query = query.Where(p => p.CreatorID == filter.UserID);
                }
                if (filter.Order.Equals("DESC"))
                {
                    query = query.OrderByDescending(p => p.CreatedOn);
                }
                else
                {
                    query = query.OrderBy(p => p.CreatedOn);
                }
                if (filter.PageNumber <= 0)
                {
                    filter.PageNumber = 1;
                }
                if (filter.PageSize <= 0)
                {
                    filter.PageSize = 10;
                }
                query = query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize);

                foreach (var item in query)
                {
                    var type = await _context.Types.FirstOrDefaultAsync(p => p.Id == item.TypeID);
                    imageListResponses.Add(new ImageListResponse
                    {
                        CollectionID = item.CollectionID,
                        CreatorID = item.CreatorID,
                        Description = item.Description,
                        Download = item.Download,
                        HasCollection = item.CollectionID != default,
                        Height = item.Height,
                        ImagePublicID = item.ImagePublicID,
                        isAIGen = item.isAIGen,
                        IsFavorite = await IsFavoriteImage(item.CreatorID, item.Id),
                        Link = item.Link,
                        Name = item.Name,
                        Orientation = item.Orientation,
                        size = item.size,
                        Status = item.Status,
                        TypeID = item.TypeID,
                        TypeName = type.Name,
                        View = item.View,
                        Width = item.Width
                    });
                }
            }
            return new Response
            {
                code = 200,
                data = imageListResponses,
                message = string.Empty
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                code = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<Response> GetFavoriteImageAsync(PaginationFilter filter)
    {
        try
        {
            if (filter.UserID == default)
            {
                return new Response
                {
                    code = StatusCodes.Status400BadRequest,
                    data = null,
                    message = "Thiếu thông tin người dùng",
                };
            }
            List<ImageListResponse> imageListResponses = new List<ImageListResponse>();
            var favs = await _context.FavoriteImages
                .AsNoTracking()
                .Where(p => p.UserID == filter.UserID)
                .ToListAsync();
            if (favs.Count > 0) {
                foreach (var fa in favs) { 
                    var item = await _context.Images.FirstOrDefaultAsync(p => p.Id == fa.ImageID);
                    var type = await _context.Types.FirstOrDefaultAsync(p => p.Id == item.TypeID);
                    imageListResponses.Add(new ImageListResponse
                    {
                        CollectionID = item.CollectionID,
                        CreatorID = item.CreatorID,
                        Description = item.Description,
                        Download = item.Download,
                        HasCollection = item.CollectionID != default,
                        Height = item.Height,
                        ImagePublicID = item.ImagePublicID,
                        isAIGen = item.isAIGen,
                        IsFavorite = await IsFavoriteImage(item.CreatorID, item.Id),
                        Link = item.Link,
                        Name = item.Name,
                        Orientation = item.Orientation,
                        size = item.size,
                        Status = item.Status,
                        TypeID = item.TypeID,
                        TypeName = type.Name,
                        View = item.View,
                        Width = item.Width
                    });
                }
            }
            return new Response
            {
                code = 200,
                data = imageListResponses,
                message = string.Empty
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                code = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<Response> GetImageByIDAsync(Guid id)
    {
        try
        {
            if (id == default)
            {
                return new Response
                {
                    code = StatusCodes.Status400BadRequest,
                    data = null,
                    message = "Thiếu thông tin Image",
                };
            }
            var image = await _context.Images.FirstOrDefaultAsync(p => p.Id == id);
            if (image != null)
            {
                var type = await _context.Types.FirstOrDefaultAsync(p => p.Id == image.TypeID);
                var i = new ImageListResponse
                {
                    CollectionID = image.CollectionID,
                    CreatorID = image.CreatorID,
                    Description = image.Description,
                    Download = image.Download,
                    HasCollection = image.CollectionID != default,
                    Height = image.Height,
                    ImagePublicID = image.ImagePublicID,
                    isAIGen = image.isAIGen,
                    IsFavorite = await IsFavoriteImage(image.CreatorID, image.Id),
                    Link = image.Link,
                    Name = image.Name,
                    Orientation = image.Orientation,
                    size = image.size,
                    Status = image.Status,
                    TypeID = image.TypeID,
                    TypeName = type.Name,
                    View = image.View,
                    Width = image.Width
                };
                return new Response
                {
                    code = 200,
                    data = i,
                    message = string.Empty
                };
            }
            return new Response
            {
                code = 200,
                data = null,
                message = string.Empty
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                code = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<Response> GetImageByTypeAsync(PaginationFilter filter)
    {
        try
        {
            if(filter.TypeID == default)
            {
                return new Response
                {
                    code = StatusCodes.Status400BadRequest,
                    data = null,
                    message = "Thiếu thông tin Type"
                };
            }
            List<ImageListResponse> imageListResponses = new List<ImageListResponse>();
            var query = _context.Images.AsNoTracking();

            if (query.Any())
            {
                if (filter.UserID != default)
                {
                    query = query.Where(p => p.CreatorID == filter.UserID);
                }
                query = query.Where(p => p.TypeID == filter.TypeID);
                if (filter.Order.Equals("DESC"))
                {
                    query = query.OrderByDescending(p => p.CreatedOn);
                }
                else
                {
                    query = query.OrderBy(p => p.CreatedOn);
                }
                if (filter.PageNumber <= 0)
                {
                    filter.PageNumber = 1;
                }
                if (filter.PageSize <= 0)
                {
                    filter.PageSize = 10;
                }
                query = query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize);

                foreach (var item in query)
                {
                    var type = await _context.Types.FirstOrDefaultAsync(p => p.Id == item.TypeID);
                    imageListResponses.Add(new ImageListResponse
                    {
                        CollectionID = item.CollectionID,
                        CreatorID = item.CreatorID,
                        Description = item.Description,
                        Download = item.Download,
                        HasCollection = item.CollectionID != default,
                        Height = item.Height,
                        ImagePublicID = item.ImagePublicID,
                        isAIGen = item.isAIGen,
                        IsFavorite = await IsFavoriteImage(item.CreatorID, item.Id),
                        Link = item.Link,
                        Name = item.Name,
                        Orientation = item.Orientation,
                        size = item.size,
                        Status = item.Status,
                        TypeID = item.TypeID,
                        TypeName = type.Name,
                        View = item.View,
                        Width = item.Width
                    });
                }
                return new Response
                {
                    code = 200,
                    data = imageListResponses,
                    message = string.Empty
                };
            }

        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
        }
        return new Response
        {
            code = StatusCodes.Status200OK,
            data = null,
            message = string.Empty,
        };
    }

    public async Task<Response> GetImageInCollectionAsync(PaginationFilter filter)
    {
        try
        {
            List<ImageListResponse> imageListResponses = new List<ImageListResponse>();
            var query = _context.Images.AsNoTracking();

            if (query.Any())
            {
                if (filter.UserID == default || filter.CollectionID == default)
                {
                    return new Response
                    {
                        code = StatusCodes.Status400BadRequest,
                        data = null,
                        message = "Thiếu thông tin Collection hoặc thông tin Creator",
                    };
                }
                query = query.Where(p => p.CreatorID == filter.UserID && p.CollectionID == filter.CollectionID);
                if (filter.Order.Equals("DESC"))
                {
                    query = query.OrderByDescending(p => p.CreatedOn);
                }
                else
                {
                    query = query.OrderBy(p => p.CreatedOn);
                }
                if (filter.PageNumber <= 0)
                {
                    filter.PageNumber = 1;
                }
                if (filter.PageSize <= 0)
                {
                    filter.PageSize = 10;
                }
                query = query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize);

                foreach (var item in query)
                {
                    var type = await _context.Types.FirstOrDefaultAsync(p => p.Id == item.TypeID);
                    imageListResponses.Add(new ImageListResponse
                    {
                        CollectionID = item.CollectionID,
                        CreatorID = item.CreatorID,
                        Description = item.Description,
                        Download = item.Download,
                        HasCollection = item.CollectionID != default,
                        Height = item.Height,
                        ImagePublicID = item.ImagePublicID,
                        isAIGen = item.isAIGen,
                        IsFavorite = await IsFavoriteImage(item.CreatorID, item.Id),
                        Link = item.Link,
                        Name = item.Name,
                        Orientation = item.Orientation,
                        size = item.size,
                        Status = item.Status,
                        TypeID = item.TypeID,
                        TypeName = type.Name,
                        View = item.View,
                        Width = item.Width
                    });
                }
            }
            return new Response
            {
                code = 200,
                data = imageListResponses,
                message = string.Empty
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                code = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<Response> GetPersonalImageAsync(PaginationFilter filter)
    {
        try
        {
            List<ImageListResponse> imageListResponses = new List<ImageListResponse>();
            var query = _context.Images.AsNoTracking();

            if (query.Any()) {
                if(filter.UserID != default)
                {
                    query = query.Where(p => p.CreatorID == filter.UserID);
                }
                if (filter.Order.Equals("DESC"))
                {
                    query = query.OrderByDescending(p => p.CreatedOn);
                }
                else
                {
                    query = query.OrderBy(p => p.CreatedOn);
                }
                if (filter.PageNumber <= 0)
                {
                    filter.PageNumber = 1;
                }
                if (filter.PageSize <= 0)
                {
                    filter.PageSize = 10;
                }
                query = query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize);

                foreach (var item in query) {
                    var type = await _context.Types.FirstOrDefaultAsync(p => p.Id == item.TypeID);
                    imageListResponses.Add(new ImageListResponse
                    {
                        CollectionID = item.CollectionID,
                        CreatorID = item.CreatorID,
                        Description = item.Description,
                        Download = item.Download,
                        HasCollection = item.CollectionID != default,
                        Height = item.Height,
                        ImagePublicID = item.ImagePublicID,
                        isAIGen = item.isAIGen,
                        IsFavorite = await IsFavoriteImage(item.CreatorID, item.Id),
                        Link = item.Link,
                        Name = item.Name,
                        Orientation = item.Orientation,
                        size = item.size,
                        Status = item.Status,
                        TypeID = item.TypeID,
                        TypeName = type.Name,
                        View = item.View,
                        Width = item.Width
                    });
                }
            }
            return new Response
            {
                code = 200,
                data = imageListResponses,
                message = string.Empty
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                code = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<Response> GetTypeAsync()
    {
        try
        {
            var types = await _context.Types.ToListAsync();
            return new Response
            {
                code = StatusCodes.Status201Created,
                data = types,
                message = "Success"
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                code = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message
            };
        }
    }

    public async Task<bool> IsFavoriteImage(Guid creatorID, Guid imageID)
    {
        bool isFavorite = false;
        var fav = await _context.FavoriteImages.FirstOrDefaultAsync(p => p.UserID == creatorID && p.ImageID == imageID);
        if (fav != null) { 
            isFavorite = true;
        }
        return isFavorite;
    }

    //public async Task<bool> IsInCollection(Guid creatorID, Guid imageID)
    //{
    //    bool hasCollection = false;
    //    var col = await _context.Collections.FirstOrDefaultAsync(p = p => p.UserID == creatorID && p.i)
    //}

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
