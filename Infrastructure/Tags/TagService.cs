using Application.Image.Interface;
using CloudinaryDotNet.Actions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Tags
{
    public class TagService : ITagService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TagService> _logger;

        public TagService(ApplicationDbContext context, ILogger<TagService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Tag> CreateTagAsync(string name, Guid creator)
        {
            try
            {
                var entity = _context.Tags.Add(new Tag
                {
                    Name = name,
                    CreatedBy = creator,
                    CreatedOn = DateTime.UtcNow,
                }).Entity;
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex, ex.Message);
                throw;
            }
        }

        public Task<List<Tag>> GetAllTagsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> TagExistAsync(string name)
        {
            try
            {
                var result = await _context.Tags.FirstOrDefaultAsync(x => x.Name == name);
                return result != null;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex, ex.Message);
                throw;
            }
        }
    }
}
