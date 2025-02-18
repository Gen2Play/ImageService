using System.ComponentModel.DataAnnotations;

namespace Application.Common;

public class PaginationFilter
{
    public Guid? UserID { get; set; }
    public Guid? CollectionID { get; set; }
    public Guid? TypeID { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; } = int.MaxValue;
    public string? Order { get; set; } = "DESC";
}
