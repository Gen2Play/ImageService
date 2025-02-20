using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image;

public class AddImageToFavRequest
{
    [Required(ErrorMessage = "Thiếu thông tin người dùng")]
    public Guid UserID { get; set; }
    [Required(ErrorMessage = "Thiếu thông tin hình ảnh")]
    public Guid ImageID { get; set; }
}
