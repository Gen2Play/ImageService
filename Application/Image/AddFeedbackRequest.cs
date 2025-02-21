using System.ComponentModel.DataAnnotations;

namespace Application.Image;

public class AddFeedbackRequest
{
    [Required(ErrorMessage = "Thiếu thông tin người dùng")]
    public Guid UserID { get; set; }
    [Required(ErrorMessage = "Thiếu thông tin hình ảnh")]
    public Guid ImageID { get; set; }
    [Required(ErrorMessage = "Thiếu thông tin đánh giá")]
    public string Title { get; set; }
}
