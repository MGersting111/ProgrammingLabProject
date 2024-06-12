using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class ProductSale
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
    }
}