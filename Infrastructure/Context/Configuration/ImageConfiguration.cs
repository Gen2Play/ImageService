using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context.Configuration;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder
            .ToTable("Image");

        builder
            .HasMany<Feedback>()
            .WithOne()
            .HasForeignKey(b => b.ImageID).IsRequired(true);

        builder
            .HasMany<FavoriteImage>()
            .WithOne()
            .HasForeignKey(b => b.ImageID).IsRequired(true);

        builder
            .HasMany<ImageTag>()
            .WithOne()
            .HasForeignKey(b => b.ImageID).IsRequired(true);
    }
}

public class TypeConfiguration : IEntityTypeConfiguration<Domain.Entities.Type>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Type> builder)
    {
        builder
            .ToTable("Type");

        builder
            .HasMany<Image>()
            .WithOne()
            .HasForeignKey(b => b.TypeID).IsRequired(true);
    }
}

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder
            .ToTable("Tag");

        builder
            .HasMany<ImageTag>()
            .WithOne()
            .HasForeignKey(b => b.TagID).IsRequired(true);
    }
}

public class CollectionConfiguration : IEntityTypeConfiguration<Collection>
{
    public void Configure(EntityTypeBuilder<Collection> builder)
    {
        builder
            .ToTable("Collection");

        builder
            .HasMany<Image>()
            .WithOne()
            .HasForeignKey(b => b.CollectionID).IsRequired(false);
    }
}
