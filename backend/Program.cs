using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // Verbindung zur MySQL-Datenbank herstellen
            app.Services.AddDbContext<NamesContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 3, 0))));

            // Weitere Konfiguration und Endpunkte hinzufügen
            // ...

            app.Run();
        }
    }

    public class NamesContext : DbContext
    {
        public DbSet<Names> Names { get; set; }

        public NamesContext(DbContextOptions<NamesContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Names>().ToTable("Names", "db1");
        }
    }

    public class Names
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
