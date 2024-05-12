using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Order
    {
       public int OrderId { get; set; }
       public string CustomerId { get; set; } = string.Empty;
       public string StoreId { get; set; } = string.Empty;
       public string OrderDate { get; set; }
       public int NItems { get; set; }
       public double total { get; set; }

       //public string StoreId { get; set;} = string.Empty;
       public Store? Store { get; set;}

       //public string CustomerID { get; set; } = string.Empty;
       public Customer? customer { get; set; }

       //public List<OrderItem> OrderItems = new List<OrderItem>();
    }
}