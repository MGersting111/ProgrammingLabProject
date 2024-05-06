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
    }
}