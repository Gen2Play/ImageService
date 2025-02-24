using Application.Common;
using Application.Image;
using Application.Image.Interface;
using Microsoft.Extensions.Options;
using RestSharp;
using Shared.Cloud;
using Shared.Responses;
using System.ComponentModel;

namespace Infrastructure.Images;

public class GenerateImageService : IGenerateImageService
{
    private readonly string Key;
    public GenerateImageService(IOptions<ImageGenSetting> option)
    {
        Key = option.Value.Key;
    }

    public async Task<Response> GenerateImageAsync(GenerateImageRequest r)
    {

        var client = new RestClient("https://api.vyro.ai/v2/image/generations/transparent");

        var request = new RestRequest(resource: "" ,method: Method.Post);
        request.AddHeader("authorization", $"Bearer {Key}");
        request.AlwaysMultipartFormData = true;
        request.AddParameter("prompt", r.Prompt);

        string aspectRatioValue = EnumHelper.GetEnumDescription(r.Ratio);
        request.AddParameter("aspect_ratio", aspectRatioValue);

        request.AddParameter("seed", r.seed);
        RestResponse response = await client.ExecuteAsync(request);
        return new Response
        {
            data = response.Content,
            message = string.Empty,
            status = 200
        };
    }
}
