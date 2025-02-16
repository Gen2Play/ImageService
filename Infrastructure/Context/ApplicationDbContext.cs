using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Image> Images { get; set; }
    public DbSet<Domain.Entities.Type> Types { get; set; }
    public DbSet<ImageTag> ImageTags { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<FavoriteImage> FavoriteImages { get; set; }
    public DbSet<Collection> Collections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
