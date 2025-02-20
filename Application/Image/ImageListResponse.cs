using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image;

public class ImageListResponse
{
    public Guid ImageID { get; set; }
    public Guid CreatorID { get; set; }
    public Guid TypeID { get; set; }
    public string TypeName { get; set; }
    public Guid CollectionID { get; set; }
    public bool IsFavorite { get; set; }
    public bool HasCollection { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Link { get; set; }
    public string ImagePublicID { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public long size { get; set; }
    public int Download { get; set; }
    public int View { get; set; }
    public bool isAIGen { get; set; }
    public ImageStatus Status { get; set; }
    public Orientation Orientation { get; set; }
}
