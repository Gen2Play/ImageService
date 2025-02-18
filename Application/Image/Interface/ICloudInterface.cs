using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image.Interface;

public interface ICloudInterface
{
    Task<ImageUploadResult> UploadImageAsync(IFormFile file);
    Task<DeletionResult> DeletionAsync(string publicID);
}
