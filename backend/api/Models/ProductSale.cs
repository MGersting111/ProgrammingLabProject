using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class ProductSale
    {
        public int Year { get; set; }
    public int Month { get; set; }
    public string Size { get; set; }
    public string Category { get; set; }
    public int TotalSales { get; set; }
    }
}