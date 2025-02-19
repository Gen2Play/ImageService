using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image;

public class CreateCollectorRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public Guid UserID { get; set; }
    public bool isPublic { get; set; } = true;
    public Guid? ImageID { get; set; }
}
