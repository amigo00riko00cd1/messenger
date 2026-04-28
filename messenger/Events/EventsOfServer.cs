using System;
using System.Collections.Generic;
using System.Text;
using messenger.Models;

namespace messenger.Events
{
    public class EventsOfServer
    {
        public delegate void EventConnection(ModelClient client);
        public delegate void EventDisconnection(ModelClient client);
        public delegate void EventMessageFromClient(ModelClient client, List<ModelClient> clients, string message);

        public event EventConnection OnClientConnected;
        public event EventDisconnection OnClientDisconnected;
        public event EventMessageFromClient OnMessageFromClient;


        public void TriggerClientConnected(ModelClient client)
        {
            OnClientConnected?.Invoke(client);
        }
        public void TriggerClientDisconnected(ModelClient client)
        {
            OnClientDisconnected?.Invoke(client);
        }
        public void TriggerMessageFromClient(ModelClient client, List<ModelClient> clients, string message)
        {
            OnMessageFromClient?.Invoke(client, clients, message);
        }
    }
}
