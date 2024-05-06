using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Order
    {
       public string OrderID { get; set; } = string.Empty;
       public string CustomerID { get; set; } = string.Empty;
       public string storeID { get; set; } = string.Empty;
       public DateTime OrderDate { get; set; }
       public int NItems { get; set; }
       public int total { get; set; }

       public string StoreStoreID { get; set;} = string.Empty;
       public Store? Store { get; set;}

       public string CustomerID { get; set; } = string.Empty;
       public Customer? customer { get; set; }

       public List<OrderItem> OrderItems = new List<OrderItem>();
    }
}