using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using messenger.DBModels;

namespace messenger.Data
{
    internal class EFDB: DbContext
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Здесь указываешь строку подключения
            optionsBuilder.UseSqlite("Data Source=DataBaseOfMessanger.db");
            // или, например: optionsBuilder.UseSqlServer("connection_string");
        }

    }
}
