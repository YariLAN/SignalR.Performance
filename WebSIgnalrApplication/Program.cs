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

builder.Services.AddSignalR();      // ���������� ������� SignalR

var app = builder.Build();
app.UseCors("AllowAll");

app.UseDefaultFiles();
app.UseStaticFiles();

// ���������� �������� ��� ������������
app.MapControllers();


app.MapHub<ChatHub>("/chat");   // ChatHub ����� ������������ ������� �� ���� /chat

app.Run();
