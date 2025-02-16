using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public abstract class AuditableEntity : AuditableEntity<Guid>
{
}

public abstract class AuditableEntity<T> : BaseEntity<T>
{
    public Guid CreatedBy { get; set; }
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    public DateTime CreatedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? LastModifiedOn { get; set; }
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }

    protected AuditableEntity()
    {
        CreatedOn = DateTime.UtcNow.AddHours(7);
        LastModifiedOn = DateTime.UtcNow.AddHours(7);
    }
}
