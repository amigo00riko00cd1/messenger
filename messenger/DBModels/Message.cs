using System;
using System.Collections.Generic;
using System.Text;

namespace messenger.DBModels
{
    internal class Message
    {
        public int Id { get; set; }
        
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public  int RecipienterId { get; set; }
        public User Recipienter { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
