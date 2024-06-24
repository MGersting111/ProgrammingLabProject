using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Models;

namespace api.Dto
{
    public class MonthlyProductSales
    {
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string Month { get; set; }
        public int Sales { get; set; }
    }
//     public class MonthlyProductSalesInfo
// {
//     public string ProductSKU { get; set; } = string.Empty;
//     public string ProductName { get; set; } = string.Empty;
//     public Dictionary<string, int> Sales { get; set; } = new Dictionary<string, int>();
// }
 }