using Microsoft.EntityFrameworkCore;
using WSChat.Infrastructure.Persistence.EntityFramework;

namespace WSChat.WebSocketApi.Extensions;

public static class MigrationHelper
{
    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        context.Database.Migrate();
    }
}