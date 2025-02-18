using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image
{
    public class DeleteImageRequest
    {
        [Required]
        public string publicID { get; set; }
        [Required]
        public Guid CreatorID { get; set; }
    }
}
