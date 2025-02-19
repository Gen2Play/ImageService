using Application.Common;
using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image.Interface;

public interface IImageService
{
    Task<Response> UploadImageAsync(UploadImageRequest request);
    Task<Response> GetPersonalImageAsync(PaginationFilter filter, Guid id);
    Task<Response> DeletePersonalImageAsync(DeleteImageRequest request);
    Task<bool> IsFavoriteImage(Guid creatorID, Guid imageID);
    Task<Response> GetAllImageImageAsync(PaginationFilter request, Guid userID);
    Task<Response> GetImageInCollectionAsync(PaginationFilter request, Guid userID, Guid collectionID);
    Task<Response> GetFavoriteImageAsync(PaginationFilter request, Guid userID);
    Task<Response> GetTypeAsync();
    Task<Response> GetImageByTypeAsync(PaginationFilter request, Guid id, Guid userID);
    Task<Response> GetImageByIDAsync(Guid id);
    Task<Response> GetAllTagsync(PaginationFilter request);
    Task<Response> GetAllByTagAsync(PaginationFilter request, Guid id);
    Task<Response> CreateCollectorAsync(CreateCollectorRequest request);
    Task<Response> GetCollectorAsync(Guid id);
    Task<Response> GetCollectorByIDAsync(Guid id);
    Task<Response> RemoveImageFromCollectorAsync(Guid id, Guid image);
    //Task<bool> IsInCollection(Guid creatorID, Guid imageID);
}
