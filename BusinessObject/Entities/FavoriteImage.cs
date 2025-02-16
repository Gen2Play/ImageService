using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class FavoriteImage : AuditableEntity
{
    public Guid UserID { get; set; }
    public Guid ImageID { get; set; }
}
