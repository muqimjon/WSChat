namespace WSChat.Infrastructure.Persistance.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WSChat.Domain.Entities;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("Chats");

        builder.HasKey(c => c.Id);

        ConfigureProperties(builder);
        ConfigureRelationships(builder);
    }

    private static void ConfigureProperties(EntityTypeBuilder<Chat> builder)
    {
        builder.Property(c => c.ChatName)
               .IsRequired()
               .HasMaxLength(100);
    }

    private static void ConfigureRelationships(EntityTypeBuilder<Chat> builder)
    {
        builder.HasMany(c => c.Messages)
               .WithOne(m => m.Chat)
               .HasForeignKey(m => m.ChatId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.ChatUsers)
               .WithOne()
               .HasForeignKey(cu => cu.ChatId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
