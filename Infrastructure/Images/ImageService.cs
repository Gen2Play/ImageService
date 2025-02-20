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

    public async Task<Response> AddImageToFavAsync(AddImageToFavRequest request)
    {
        var transaction = _context.Database.BeginTransaction();
        try
        {
            var exist = await _context.Images.FirstOrDefaultAsync(p => p.Id == request.ImageID);
            if (exist == null)
            {
                throw new Exception("Hình ảnh không tồn tại");
            }

            var fav_e = await _context.FavoriteImages.FirstOrDefaultAsync(p => p.UserID == request.UserID && p.ImageID == request.ImageID);
            if (fav_e != null) {
                return new Response
                {
                    status = StatusCodes.Status200OK,
                    data = null,
                    message = "Hình ảnh đã được thêm vào yêu thích"
                };
            }

            await _context.FavoriteImages.AddAsync(new FavoriteImage
            {
                UserID = request.UserID,
                ImageID = request.ImageID,
                CreatedBy = request.UserID,
                CreatedOn = DateTime.Now,
            });
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new Response
            {
                status = StatusCodes.Status201Created,
                data = null,
                message = "Success"
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            await transaction.RollbackAsync();
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message
            };
        }
    }

    public async Task<Response> ChangeStatusAsync(UpdateImageStatus request)
    {
        var transaction = _context.Database.BeginTransaction();
        try
        {
            var exist = await _context.Images.FirstOrDefaultAsync(p => p.Id == request.ImageId);
            if (exist != null) {
                exist.Status = request.Status;
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new Response
            {
                data = null,
                message = string.Empty,
                status = 200
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            await transaction.RollbackAsync();
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message
            };
        }
    }

    public async Task<Response> CreateCollectorAsync(CreateCollectorRequest request)
    {
        var transaction = _context.Database.BeginTransaction();
        try
        {
            var exist = await _context.Collections.FirstOrDefaultAsync(p => p.Name ==  request.Name && p.UserID == request.UserID);

            if (exist != null) {
                throw new Exception("Collection đã tồn tại");
            }

            var col = _context.Collections.Add(new Collection
            {
                UserID = request.UserID,
                Name = request.Name,
                CreatedBy = request.UserID,
                CreatedOn = DateTime.Now,
                isPublic = request.isPublic
            }).Entity;

            if (request.ImageID != default)
            {
                var iExist = await _context.Images.FirstOrDefaultAsync(p => p.Id == request.ImageID);
                if (iExist == null)
                {
                    throw new Exception("Image không tồn tại");
                }
                iExist.CollectionID = col.Id;
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new Response
            {
                status = StatusCodes.Status200OK,
                data = null,
                message = "Xóa thành công"
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            await transaction.RollbackAsync();
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
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
                    status = StatusCodes.Status404NotFound,
                    data = null,
                    message = "Không thể tìm thấy ảnh"
                };
            }
            var cloudDelete = await _cloudInterface.DeletionAsync(image.ImagePublicID);
            if (cloudDelete.Error != null)
            {
                return new Response
                {
                    status = (int)cloudDelete.StatusCode,
                    data = null,
                    message = cloudDelete.Error.Message
                };
            }
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new Response
            {
                status = StatusCodes.Status200OK,
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
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<Response> GetAllImageAsync(PaginationFilter request)
    {
        try
        {
            List<Image> images = new List<Image>();

            if (!String.IsNullOrEmpty(request.KeySearch))
            {
                images = await GetImageByTagNameAsync(request.KeySearch);
            }
            else
            {
                images = await _context.Images.ToListAsync();
            }
            var query = images.Where(p => p.Status == ImageStatus.Accept);
            if(request.CreatorID != default)
            {
                query = query.Where(p => p.CreatorID == request.CreatorID);
            }
            if(request.Order != null)
            {
                if (request.Order.Equals("DESC"))
                {
                    query = query.OrderByDescending(p => p.CreatedOn);
                }
                else
                {
                    query = query.OrderBy(p => p.CreatedOn);
                }
            }
            if(request.Orientation != Orientation.None)
            {
                query = query.Where(p => p.Orientation == request.Orientation);
            }

            if (request.SearchSizeRequest != null) { 
                query = query.Where(p => p.Height >= request.SearchSizeRequest.Height && p.Width >= request.SearchSizeRequest.Width);
            }
            if(request.CreateAT != default)
            {
                query = query.Where(p => p.CreatedOn >= request.CreateAT);
            }

            if(request.CreatorType != CreatorType.None)
            {
                query = query.Where(p => request.CreatorType == CreatorType.AI ? p.isAIGen : !p.isAIGen);
            }
            if (request.PageNumber <= 0)
            {
                request.PageNumber = 1;
            }
            if (request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            query = query.Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);
            List<ImageListResponse> imageListResponses = new List<ImageListResponse>();
            foreach (var item in query)
            {
                var type = await _context.Types.FirstOrDefaultAsync(p => p.Id == item.TypeID);
                imageListResponses.Add(new ImageListResponse
                {
                    ImageID = item.Id,
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
                status = 200,
                data = imageListResponses,
                message = string.Empty
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<Response> GetAllTagsync(PaginationFilter filter)
    {
        try
        {
            var query = _context.Tags.AsNoTracking();

            if (query != null) {
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

                return new Response
                {
                    status = 200,
                    data = await query.ToListAsync(),
                    message = string.Empty
                };

            }

            return new Response
            {
                status = 200,
                data = null,
                message = string.Empty
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<Response> GetCollectorAsync(Guid id)
    {
        try
        {
            if(id == default)
            {
                throw new Exception("Thông tin user không hợp lệ");
            }
            var result = new List<CollectionByUserResponse>();
            var collections = await _context.Collections
                .Where(p => p.UserID == id).ToListAsync();

            if(collections == null)
            {
                throw new Exception("Thông tin Collection không hợp lệ");
            }

            foreach( var collection in collections)
            {
                var images = await _context.Images.Where(p => p.CollectionID == collection.Id).Take(3).ToListAsync();

                var r = new CollectionByUserResponse
                {
                    BackGroundImages = images.Select(p => p.Link).ToList(),
                    CollectionID = collection.Id,
                    IsPublic = collection.isPublic,
                    Name = collection.Name,
                    UserID = collection.UserID
                };
                result.Add(r);
            }
            return new Response
            {
                status = 200,
                data = result,
                message = string.Empty
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<Response> GetCollectorByIDAsync(Guid id)
    {
        try
        {
            var collection = await _context.Collections
                .Where(p => p.UserID == id).FirstOrDefaultAsync();

            if (collection == null)
            {
                throw new Exception("Thông tin Collection không hợp lệ");
            }

            var images = await _context.Images.Where(p => p.CollectionID == collection.Id).ToListAsync();
            List<ImageListResponse> imageListResponses = new List<ImageListResponse>();
            foreach (var item in images) {
                var type = await _context.Types.FirstOrDefaultAsync(p => p.Id == item.TypeID);
                imageListResponses.Add(new ImageListResponse
                {
                    ImageID = item.Id,
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
            var r = new CollectionByIDResponse
            {
                CollectionID = collection.Id,
                IsPublic = collection.isPublic,
                Name = collection.Name,
                UserID = collection.UserID,
                Images = imageListResponses,
            };
            return new Response
            {
                status = 200,
                data = r,
                message = string.Empty
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<Response> GetFavoriteImageAsync(PaginationFilter filter, Guid userID)
    {
        try
        {
            if (userID == default)
            {
                return new Response
                {
                    status = StatusCodes.Status400BadRequest,
                    data = null,
                    message = "Thiếu thông tin người dùng",
                };
            }
            List<ImageListResponse> imageListResponses = new List<ImageListResponse>();
            var favs = await _context.FavoriteImages
                .AsNoTracking()
                .Where(p => p.UserID == userID)
                .ToListAsync();
            if (favs.Count > 0) {
                foreach (var fa in favs) { 
                    var item = await _context.Images.FirstOrDefaultAsync(p => p.Id == fa.ImageID);
                    var type = await _context.Types.FirstOrDefaultAsync(p => p.Id == item.TypeID);
                    imageListResponses.Add(new ImageListResponse
                    {
                        ImageID = item.Id,
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
                status = 200,
                data = imageListResponses,
                message = string.Empty
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
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
                    status = StatusCodes.Status400BadRequest,
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
                    ImageID = image.Id,
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
                    status = 200,
                    data = i,
                    message = string.Empty
                };
            }
            return new Response
            {
                status = 200,
                data = null,
                message = string.Empty
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<List<Image>> GetImageByTagNameAsync(string tag)
    {
        try
        {
            var tags = tag.Split(" ");
            HashSet<Guid> imageIds = new HashSet<Guid>();
            List<Image> images = new List<Image>();
            foreach(var t in tags)
            {
                var query = await _context.Tags.Where(p => p.Name.Contains(t) || p.Name.Contains(tag)).ToListAsync();

                if(query.Count > 0)
                {
                    foreach(var item in query)
                    {
                        var its = await _context
                            .ImageTags
                            .Where(p => p.TagID == item.Id)
                            .Select(it => it.ImageID).ToListAsync();
                        foreach (var imageId in its)
                        {
                            if (!imageIds.Contains(imageId))
                            {
                                var image = await _context.Images.FirstOrDefaultAsync(p => p.Id == imageId);
                                if (image != null)
                                {
                                    images.Add(image);
                                    imageIds.Add(imageId);
                                }
                            }
                        }
                    }
                }
            }
            return images;
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            throw;
        }
    }

    public async Task<Response> GetImageInCollectionAsync(PaginationFilter filter, Guid userID, Guid collectionID)
    {
        try
        {
            List<ImageListResponse> imageListResponses = new List<ImageListResponse>();
            var query = _context.Images.AsNoTracking();

            if (query.Any())
            {
                if (userID == default || collectionID == default)
                {
                    return new Response
                    {
                        status = StatusCodes.Status400BadRequest,
                        data = null,
                        message = "Thiếu thông tin Collection hoặc thông tin Creator",
                    };
                }
                query = query.Where(p => p.CreatorID == userID && p.CollectionID == collectionID);
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
                        ImageID = item.Id,
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
                status = 200,
                data = imageListResponses,
                message = string.Empty
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
    }

    public async Task<Response> GetImageToVerifyAsync()
    {
        try
        {
            var result = await _context.Images.Where(p => p.Status == ImageStatus.Waiting).ToListAsync();

            return new Response
            {
                data = result,
                message = string.Empty,
                status = 200
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message
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
                status = StatusCodes.Status201Created,
                data = types,
                message = "Success"
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
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

    public async Task<Response> RemoveImageFromCollectorAsync(Guid id, Guid image)
    {
        var transaction = _context.Database.BeginTransaction();
        try
        {
            if (id == default || image == default)
            {
                throw new Exception("Thông tin không hợp lệ");
            }

            var exist = await _context.Images.Where(p => p.Id == image && p.CollectionID == id).FirstOrDefaultAsync();
            
            if(exist == null)
            {
                throw new Exception("Ảnh không có trong Collection");
            }

            exist.CollectionID = default;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new Response
            {
                status = 200,
                data = null,
                message = "Xóa thành công"
            };
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex, ex.Message);
            transaction.RollbackAsync();
            return new Response
            {
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message,
            };
        }
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
                    status = StatusCodes.Status400BadRequest,
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
                Status = ImageStatus.Waiting,
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
                status = StatusCodes.Status201Created,
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
                status = StatusCodes.Status500InternalServerError,
                data = null,
                message = ex.Message
            };
        }
    }
}
