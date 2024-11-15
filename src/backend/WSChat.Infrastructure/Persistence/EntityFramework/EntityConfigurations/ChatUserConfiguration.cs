namespace WSChat.Infrastructure.Persistance.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WSChat.Domain.Entities;

public class ChatUserConfiguration : IEntityTypeConfiguration<ChatUser>
{
    public void Configure(EntityTypeBuilder<ChatUser> builder)
    {
        builder.ToTable("ChatUsers");

        builder.HasKey(cu => new { cu.ChatId, cu.UserId });

        ConfigureRelationships(builder);
    }

    private static void ConfigureRelationships(EntityTypeBuilder<ChatUser> builder)
    {
        builder.HasOne(cu => cu.Chat)
               .WithMany(c => c.ChatUsers)
               .HasForeignKey(cu => cu.ChatId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cu => cu.User)
               .WithMany()
               .HasForeignKey(cu => cu.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
