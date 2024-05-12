using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Customer
    {
      public string CustomerID { get; set; } = string.Empty;
    
      public double Latitude { get; set; }

      public double Longitude { get; set; }

      public List<Order> Orders { get; set; } = new List<Order>();

    }
}