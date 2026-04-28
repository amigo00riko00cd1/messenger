using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace messenger.Models
{
    public class ModelClient
    {
        public IPEndPoint EndPoint { get; set; }
        public WebSocket WebSocket { get; set; }
    }
}
