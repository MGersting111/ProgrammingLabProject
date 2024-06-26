using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Dashboard
    {
       public string StoreId { get; set; }
        public double TotalRevenue { get; set; }
        public string SKU { get; set; } 
        public int TotalOrders { get; set; }
        public int CustomerId { get; set; }
        public double TotalSpent { get; set; }
    }
}