using System.Net.WebSockets;
using messenger.Data;
using messenger.Models;

namespace messenger
{
    internal class Program
    {
        static async Task Main(string[] args)
        {   

            EFDB db = new EFDB();

            foreach (var user in db.Users)
            {
                Console.WriteLine($"User: {user.Name}");
            }

            WebSocketServer webSocketServer = new WebSocketServer(8080);

            webSocketServer.Events.OnClientConnected += (client) => 
            {
                Console.WriteLine($"Client connected: {client.EndPoint.Address}:{client.EndPoint.Port}");
            };

            webSocketServer.Events.OnClientDisconnected += (client) => 
            {
                Console.WriteLine($"Client disconnected: {client.EndPoint.Address}:{client.EndPoint.Port}");
            };

            webSocketServer.Events.OnMessageFromClient += async (client, clients, message) => 
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<ModelMessageForServer>(message);

                var dataForClietn = new ModelMessageForClient()
                {
                    Sender = client.EndPoint.Address.ToString(),
                    Message = data.Message
                };

                string newData = System.Text.Json.JsonSerializer.Serialize(dataForClietn);

                byte[] buffer = new byte[1024];
                foreach (var c in clients)
                {
                    buffer = System.Text.Encoding.UTF8.GetBytes(newData);
                    await c.WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            };

            await webSocketServer.Start();
        }
    }
}
