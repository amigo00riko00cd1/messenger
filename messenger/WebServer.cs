using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using messenger.Models;

namespace messenger
{
    internal class WebServer
    {
        HttpListener listener;
        List<ModelClient> clients = new();

        public WebServer(int port)
        {
            listener = new HttpListener();
            listener.Prefixes.Add($"http://+:{port}/");
        }

        public async Task Start()
        {
            listener.Start();
            Console.WriteLine("WebSocket server started.");
            while (true)
            {
                var context = listener.GetContext();
                if (context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    var webSocket = wsContext.WebSocket;

                    ModelClient client = new()
                    {
                        EndPoint = context.Request.RemoteEndPoint,
                        WebSocket = webSocket
                    };

                    lock (clients)
                    {
                        clients.Add(client);
                    }

                    Console.WriteLine($"connected: {context.User?.Identity?.Name}");
                    _ = Task.Run(() => HandleWebSocket(client));

                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async Task HandleWebSocket(ModelClient client)
        {
            byte[] buffer = new byte[1024];

            while(client.WebSocket.State == WebSocketState.Open)
            {
                var result = await client.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    lock (clients)
                    {
                        clients.Remove(client);
                    }
                    await client.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine(message);

                try
                {
                    var data = System.Text.Json.JsonSerializer.Deserialize<ModelMessageForServer>(message);

                    if (data != null)
                        _ = Task.Run(() => BroadcastMessage(data, client));

                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    
        private async Task BroadcastMessage(ModelMessageForServer message, ModelClient sender)
        {   
            message.Sender = sender.EndPoint.Address.ToString();

            var jsonMessage = System.Text.Json.JsonSerializer.Serialize(message);


            byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);
            List<ModelClient> clientsCopy;
            lock (clients)
            {
                clientsCopy = new List<ModelClient>(clients);
            }
            foreach (var client in clientsCopy)
            {
                if (client.WebSocket.State == WebSocketState.Open)
                {
                    await client.WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
