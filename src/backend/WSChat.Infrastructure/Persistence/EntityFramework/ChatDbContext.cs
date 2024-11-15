namespace WSChat.Infrastructure.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options), IChatDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatUser> ChatUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChatDbContext).Assembly);
    }
}
