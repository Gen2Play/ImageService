using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image.Interface;

public interface IGenerateImageService
{
    public Task<Response> GenerateImageAsync(GenerateImageRequest request);
}
