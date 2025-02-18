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
    Task<Response> GetPersonalImageAsync(Guid creatorID);
}
