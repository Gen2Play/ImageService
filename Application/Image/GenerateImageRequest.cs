using Application.Image.Generate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image
{
    public class GenerateImageRequest
    {
        [Required(ErrorMessage = "Message is empty")]
        public string Prompt { get; set; }
        public AspectRatio Ratio { get; set; } = AspectRatio.Default;
        public int seed { get; set; } = 12;
    }
}
