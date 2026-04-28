using System;
using System.Collections.Generic;
using System.Text;

namespace messenger.DBModels
{
    internal class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public List<Chat> Chats { get; set; } = new();
    }
}
