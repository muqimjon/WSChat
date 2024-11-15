﻿namespace WSChat.Infrastructure.Persistance.EntityFramework.EntityConfigurations;

using WSChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        ConfigureProperties(builder);
        ConfigureRelationships(builder);
    }

    private static void ConfigureProperties(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.Username)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(u => u.PasswordHash)
               .IsRequired();
    }

    private static void ConfigureRelationships(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(u => u.Messages)
               .WithOne(m => m.Sender)
               .HasForeignKey(m => m.SenderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ChatUsers)
               .WithOne(cu => cu.User)
               .HasForeignKey(cu => cu.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
