using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BookStore.Data;
using System.IO;

namespace BookStore.Data
{

    /// <summary>
    /// Контекст базы данных приложения.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;

        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            string databasePath =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Orders.db");

            optionsBuilder.UseSqlite(
                $"Data Source={databasePath}");
        }

    }
}
