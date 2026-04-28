using System;
using System.Collections.Generic;
using System.Text;

namespace messenger.DBModels
{
    internal class Chat
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public List<Message> Messages { get; set; } = new();
        public List<User> Users { get; set; } = new();
    }
}
