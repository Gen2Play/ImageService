using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Image : AuditableEntity
{
    [Required]
    public string CreatorID { get; set; }
    public Guid TypeID { get; set; }
    public Guid CollectionID { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Link { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public int size { get; set; }
    public int Download {  get; set; }
    public int View {  get; set; }
    public bool isAIGen { get; set; }
    public ImageStatus Status { get; set; }
    public Orientation Orientation { get; set; }
    
}