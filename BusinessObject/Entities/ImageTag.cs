using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class ImageTag : AuditableEntity
{
    public Guid ImageID { get; set; }
    public Guid TagID { get; set; }
}
