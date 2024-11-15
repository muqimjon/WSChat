using WSChat.Application;
using WSChat.Infrastructure;
using System.Net.WebSockets;
using System.Text;

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

app.Map("/ws", async (HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await HandleWebSocketCommunication(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


// WebSocket kommunikatsiyasi
async Task HandleWebSocketCommunication(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];
    WebSocketReceiveResult result = null;

    try
    {
        while (webSocket.State == WebSocketState.Open)
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Yangi xabar: {message}");

                // Mijozga xabar yuborish
                var messageBytes = Encoding.UTF8.GetBytes($"Serverdan xabar: {message}");
                var segment = new ArraySegment<byte>(messageBytes);
                await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Xatolik: {ex.Message}");
    }
    finally
    {
        if (webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Yakunlandi", CancellationToken.None);
        }
    }
}















//using WSChat.Infrastructure;
//using WSChat.Application;

//var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


//builder.Services.AddInfrastructure(builder.Configuration);
//builder.Services.AddApplication();
//builder.Services.AddHttpContextAccessor();


//var app = builder.Build();

////app.UseWebSockets();
////app.MapGet("/ws", async context =>
////{
////    if (context.WebSockets.IsWebSocketRequest)
////    {
////            using var ws = await context.WebSockets.AcceptWebSocketAsync();
////        while(true)
////        {
////            var message = "Hello World" + DateTime.Now.ToString("HH-mm-ss");
////            var bytes = Encoding.UTF8.GetBytes(message);
////            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
////            if(ws.State == System.Net.WebSockets.WebSocketState.Closed ||
////                ws.State == System.Net.WebSockets.WebSocketState.Aborted)
////            {
////                break;
////            }
////            Thread.Sleep(1000);
////            await ws.SendAsync(arraySegment,
////                System.Net.WebSockets.WebSocketMessageType.Text,
////                true,
////                CancellationToken.None);
////        }
////    }
////    {
////        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
////    }
////});


//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.UseStaticFiles();

//app.MapControllers();

//app.Run();