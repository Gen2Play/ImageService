using Application.Common;
using Application.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel.DataAnnotations;

namespace Application.Image;

public class UploadImageRequest
{
    [Required(ErrorMessage = "Không có thông tin của creator")]
    public Guid CreatorID { get; set; }
    [AllowedExtensions(FileType.Image)]
    [MaxFileSize(3, 0)]
    public IFormFile Image { get; set; }
    [Required(ErrorMessage = "Mô tả ảnh không được bỏ trống")]
    public string Description { get; set; }
    public bool IsAiGen { get; set; }
    //[MinLength(2, ErrorMessage = "Tag không được phép ít hơn 3")]
    public List<string> Tags { get; set; }
}
