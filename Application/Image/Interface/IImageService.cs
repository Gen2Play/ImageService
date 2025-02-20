using Application.Common;
using Domain.Entities;
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
    Task<Response> DeletePersonalImageAsync(DeleteImageRequest request);
    Task<bool> IsFavoriteImage(Guid creatorID, Guid imageID);
    Task<Response> GetImageInCollectionAsync(PaginationFilter request, Guid userID, Guid collectionID);
    Task<Response> GetFavoriteImageAsync(PaginationFilter request, Guid userID);
    Task<Response> GetTypeAsync();
    Task<Response> GetImageByIDAsync(Guid id);
    Task<Response> GetAllTagsync(PaginationFilter request);
    Task<Response> CreateCollectorAsync(CreateCollectorRequest request);
    Task<Response> GetCollectorAsync(Guid id);
    Task<Response> GetCollectorByIDAsync(Guid id);
    Task<Response> RemoveImageFromCollectorAsync(Guid id, Guid image);
    Task<Response> GetAllImageAsync(PaginationFilter request);
    Task<List<Domain.Entities.Image>> GetImageByTagNameAsync(string tag);
    Task<Response> AddImageToFavAsync(AddImageToFavRequest request);
    Task<Response> GetImageToVerifyAsync();
    Task<Response> ChangeStatusAsync(UpdateImageStatus request);
    //Task<bool> IsInCollection(Guid creatorID, Guid imageID);
}
