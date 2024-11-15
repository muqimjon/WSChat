namespace WSChat.Infrastructure.Persistance.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WSChat.Domain.Entities;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");
        builder.HasKey(m => m.Id);
        ConfigureProperties(builder);
        ConfigureRelationships(builder);
    }

    private static void ConfigureProperties(EntityTypeBuilder<Message> builder)
    {
        builder.Property(m => m.Content)
               .IsRequired(false)
               .HasMaxLength(500);

        builder.Property(m => m.FilePath)
               .HasMaxLength(500);
    }

    private static void ConfigureRelationships(EntityTypeBuilder<Message> builder)
    {
        builder.HasOne(m => m.Sender)
               .WithMany()
               .HasForeignKey(m => m.SenderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Chat)
               .WithMany(c => c.Messages)
               .HasForeignKey(m => m.ChatId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.ReplyToMessage)
               .WithMany()
               .HasForeignKey(m => m.ReplyToMessageId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
