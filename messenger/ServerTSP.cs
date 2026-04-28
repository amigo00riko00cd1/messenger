using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace messenger
{
    public class ServerTSP
    {
        private Socket _socket;
        private int _port;

        private List<Socket> clients = new();
       

        public ServerTSP(int port)
        {
            _port = port;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start(int listen = 1)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Any, _port));
            _socket.Listen(listen);
            Console.WriteLine($"server starting on port {_port}");

            AcceptClients();

            Console.ReadLine();
            Console.WriteLine("server stoping")
;
        }

        private void AcceptClients()
        {
            Socket client;

            Console.WriteLine("awaiting connected");
            
            while (true)
            {
                client = _socket.Accept();
                Console.WriteLine($"connection {client.RemoteEndPoint}");
                
                clients.Add(client);
                Task.Run(() => ManagementClient(client));

            }
        }

        private async Task ManagementClient(Socket client)
        {
            byte[] buffer = new byte[1024];
            int received;

            while ((received = await client.ReceiveAsync(buffer)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, received);
                Console.WriteLine(message);

                await client.SendAsync(Encoding.UTF8.GetBytes("Message received"));
            }
            
            clients.Remove(client);
            Console.WriteLine($"disconnect {client.RemoteEndPoint}");
        }

    }
}
