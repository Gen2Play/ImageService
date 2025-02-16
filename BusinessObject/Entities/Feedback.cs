using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Feedback : AuditableEntity
{
    public Guid UserFeedbackID {  get; set; }
    public Guid ImageID { get; set; }
    [Required]
    public string FeedbackTitle { get; set; }
}
