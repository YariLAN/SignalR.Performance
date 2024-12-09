using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRChatApp.Hubs;

public class ClientLatency
{
    public long Time { get; set; } // Время в формате Unix Timestamp
    public double Latency { get; set; }
}

public class ChatHub : Hub
{
    public static List<(DateTime Time, double Latency)> GlobalLatencies = new List<(DateTime, double)>();

    // Отправка сообщения в определенную комнату
    public async Task Send(string message)
    {
        await this.Clients.All.SendAsync("Receive", message);
    }

    // Метод для получения данных от клиентов
    public async Task SendLatencies(List<ClientLatency> latencies)
    {
        lock (GlobalLatencies) // Потокобезопасность
        {
            foreach (var latency in latencies)
            {
                GlobalLatencies.Add((DateTimeOffset.FromUnixTimeMilliseconds(latency.Time).UtcDateTime, latency.Latency));
            }
        }
    }

    // Подключение пользователя к комнате
    public async Task JoinRoom(string room)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, room);
        await Clients.Group(room).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} joined {room}");
    }

    // Отключение пользователя от комнаты
    public async Task LeaveRoom(string room)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        await Clients.Group(room).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} left {room}");
    }
}