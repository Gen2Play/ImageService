using Application.Image;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Common;

public class PaginationFilter
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; } = int.MaxValue;
    public string Order { get; set; } = "DESC";
    public string? KeySearch { get; set; }
    public Orientation? Orientation { get; set; }
    public SearchSizeRequest? SearchSizeRequest { get; set; }
    public DateTime? CreateAT { get; set; }
    public CreatorType CreatorType { get; set; }
    public Guid? TypeID { get; set; }
    public Guid? CreatorID { get; set; }
}
