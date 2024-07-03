using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.EntityFrameworkCore;
using api.Dto;
using api.Data;

namespace api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
            Options = options;
        }

        public DbSet<CacheEntry> CacheEntries { get; set; }
        public DbContextOptions<ApplicationDBContext> Options { get; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Goal> Goals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "server=localhost;user=root;password=password;database=testdb;port=3306";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));


        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<CacheEntry>()
                .HasKey(c => c.CacheKey);

            modelBuilder.Entity<CacheEntry>().ToTable("CacheEntries", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Customer>().ToTable("Customers", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Store>().ToTable("Stores", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Product>().ToTable("Products", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Order>().ToTable("Orders", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<Product>().HasKey(p => p.SKU);
            modelBuilder.Entity<Goal>().HasKey(g => g.Id);
            modelBuilder.Entity<OrderItem>().HasKey(oi => new { oi.OrderId, oi.SKU }); // Composite key
            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
            modelBuilder.Entity<Store>().HasKey(s => s.StoreId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .IsRequired();

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.SKU)
                .IsRequired();

            modelBuilder.Entity<Order>().HasIndex(o => o.StoreId);
            modelBuilder.Entity<OrderItem>().HasIndex(oi => oi.OrderId);
            modelBuilder.Entity<OrderItem>().HasIndex(oi => oi.SKU);

        }
    }
}
