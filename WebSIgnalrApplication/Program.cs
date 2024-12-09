using SignalRChatApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

builder.Services.AddSignalR();      // подключены сервисы SignalR

var app = builder.Build();
app.UseCors("AllowAll");

app.UseDefaultFiles();
app.UseStaticFiles();

// Подключаем маршруты для контроллеров
app.MapControllers();


app.MapHub<ChatHub>("/chat");   // ChatHub будет обрабатывать запросы по пути /chat

app.Run();
