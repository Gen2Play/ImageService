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
    Task<Response> GetPersonalImageAsync(PaginationFilter filter);
    Task<Response> DeletePersonalImageAsync(DeleteImageRequest request);
    Task<bool> IsFavoriteImage(Guid creatorID, Guid imageID);
    Task<Response> GetAllImageImageAsync(PaginationFilter request);
    Task<Response> GetImageInCollectionAsync(PaginationFilter request);
    Task<Response> GetFavoriteImageAsync(PaginationFilter request);
    Task<Response> GetTypeAsync();
    Task<Response> GetImageByTypeAsync(PaginationFilter request);
    Task<Response> GetImageByIDAsync(Guid id);
    //Task<bool> IsInCollection(Guid creatorID, Guid imageID);
}
