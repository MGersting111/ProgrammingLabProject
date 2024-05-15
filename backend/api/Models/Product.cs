using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Product
    {

        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public string Launch { get; set; } = string.Empty;

        //public List<OrderItem> OrderItems { get; set;} = new List<OrderItem>();


    }
}