using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using messenger.Events;
using messenger.Models;

namespace messenger
{
    internal class WebSocketServer
    {
        private HttpListener listener;
        private List<ModelClient> clients;
        private EventsOfServer events;

        public EventsOfServer Events { get => events; }
        

        public WebSocketServer(int port)
        {
            listener = new HttpListener();
            listener.Prefixes.Add($"http://+:{port}/");
            clients = new List<ModelClient>();
            events = new EventsOfServer();
        }

        public async Task Start()
        {
            listener.Start();
            Console.WriteLine("Server started...");
            
            await Accept();

            Console.WriteLine("Server stopped.");
        }

        private async Task Accept()
        {
            while (true)
            {
                var context = await listener.GetContextAsync();

                if(context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    var webSocket = wsContext.WebSocket;

                    var client = new ModelClient
                    {
                        EndPoint = context.Request.RemoteEndPoint,
                        WebSocket = webSocket
                    };

                    lock (clients)
                    {
                        clients.Add(client);
                    }
                    Events.TriggerClientConnected(client);

                    _ = Task.Run(() => HandlerClisnt(client));
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }
    
        private async Task HandlerClisnt(ModelClient client)
        {
            byte[] buffer = new byte[1024];
            while (client.WebSocket.State == WebSocketState.Open)
            {   
                try
                {
                    var result = await client.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if(result.MessageType == WebSocketMessageType.Close) break;


                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine(message);

                    Events.TriggerMessageFromClient(client, clients, message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
              
            }

            lock (clients)
            {
                clients.Remove(client);
            }
            Events.TriggerClientDisconnected(client);
        }

        
    }
}
