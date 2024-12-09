using Microsoft.AspNetCore.SignalR.Client;

namespace WebSIgnalrApplication;

public class SignalRClient
{
    public async Task StartAsync(string serverUrl)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl($"{serverUrl}/chatHub", Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling)
            .Build();

        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            Console.WriteLine($"[{user}] {message}");
        });

        await connection.StartAsync();
        Console.WriteLine("Connected to SignalR server.");

        // Отправка сообщений каждые 2 секунды
        while (true)
        {
            await connection.InvokeAsync("SendMessage", "TestUser", "Hello from client!");
            await Task.Delay(2000);
        }
    }
}
