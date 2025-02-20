using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image;

public class UpdateImageStatus
{
    [Required]
    public Guid ImageId { get; set; }
    public ImageStatus Status { get; set; }
}
