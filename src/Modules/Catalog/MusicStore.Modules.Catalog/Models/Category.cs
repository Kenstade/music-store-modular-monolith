﻿using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MusicStore.Modules.Catalog.Models;

internal sealed class Category : Entity<int>
{
    public Category(string name)
    {
        Name = name;
    }
    public string Name { get; private set; }
    public ICollection<SpecificationType> SpecificationTypes { get; set; } = null!;
}

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .HasMaxLength(50);
    }
}
