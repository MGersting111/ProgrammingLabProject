using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using api.Models;
using api.Interfaces;

namespace api.Models
{
    public class Customer
    {
      public string CustomerId { get; set; } = string.Empty;
    
      public double Latitude { get; set; }

      public double Longitude { get; set; }

      public List<Order> Orders { get; set; } = new List<Order>();

    }
}