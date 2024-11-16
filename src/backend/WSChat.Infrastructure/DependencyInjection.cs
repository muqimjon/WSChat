namespace WSChat.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WSChat.Application.Interfaces;
using WSChat.Infrastructure.Persistence.EntityFramework;
using WSChat.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ChatDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IChatDbContext, ChatDbContext>();
        services.AddSingleton<WebSocketManager>();
        services.AddScoped<IWebSocketService, WebSocketService>();

        return services;
    }
}
