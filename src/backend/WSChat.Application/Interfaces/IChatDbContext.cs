using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WSChat.Domain.Entities;

namespace WSChat.Application.Interfaces;

public interface IChatDbContext
{
    DbSet<User> Users { get; }
    DbSet<Message> Messages { get; }
    DbSet<Chat> Chats { get; }
    DbSet<ChatUser> ChatUsers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}