using System;
using Microsoft.EntityFrameworkCore;

namespace SignalRChatApp.Models
{
    public class SignalrDbContext : DbContext
    {
        public SignalrDbContext(DbContextOptions<SignalrDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}

