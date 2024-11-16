using EcoLink.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using WSChat.Application;
using WSChat.Infrastructure;
using WSChat.WebSocketApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MigrateDatabase();
app.UseCors("AllowAll");
app.UseWebSockets();
app.UseStaticFiles();

app.UseRouting();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();