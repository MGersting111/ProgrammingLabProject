using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class OrderItem
    {
        public string OrderID { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;

        public string ProductSKU { get; set; } = string.Empty;
        public Product? Product { get; set;}

        //public string OrderID { get; set; } = string.Empty;
        public Order? Order { get; set; }


    }
}