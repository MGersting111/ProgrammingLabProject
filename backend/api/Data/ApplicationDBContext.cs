using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {
            
        }

        public DbSet<Stores> Stores {get; set;}
       // public DbSet<customers> customers {get; set; }
       // public DbSet<orderItems> orderItems {get; set;}
       // public DbSet<products> products {get; set;}
        
    }
}