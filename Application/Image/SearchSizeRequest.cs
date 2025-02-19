using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image;

public class SearchSizeRequest
{
    [Range(0, int.MaxValue, ErrorMessage = "Width phải lớn hơn 0")]
    public int Width { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Width phải lớn hơn 0")]
    public int Height { get; set; }
}
