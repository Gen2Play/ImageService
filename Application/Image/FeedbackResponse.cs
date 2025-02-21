using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image;

public class FeedbackResponse
{
    public Guid FeedbackID { get; set; }
    public Guid UserID { get; set; }
    public string Title { get; set; }
    public DateTime CreateAt { get; set; }
}
