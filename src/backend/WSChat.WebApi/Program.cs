using WSChat.Application;
using WSChat.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable WebSocket support
app.UseWebSockets();

// Add UseStaticFiles middleware
app.UseStaticFiles(); // Static fayllarga xizmat ko'rsatish

app.UseRouting(); // Routingni sozlashdan oldin qo'yiladi

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();