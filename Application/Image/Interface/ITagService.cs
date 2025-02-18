using Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Application.Image.Interface
{
    public interface ITagService
    {
        Task<Tag> CreateTagAsync(string name, Guid creator);
        Task<bool> TagExistAsync(string name);
        Task<List<Tag>> GetAllTagsAsync();
    }
}
