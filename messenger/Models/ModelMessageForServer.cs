using System;
using System.Collections.Generic;
using System.Text;

namespace messenger.Models
{
    public class ModelMessageForServer
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Message { get; set; }
    }
}
