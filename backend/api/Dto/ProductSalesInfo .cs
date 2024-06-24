using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Models;


namespace api.Dto
{
    public class ProductSalesInfo 
    {
        public string ProductSKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int TotalSales { get; set; }
    public double TotalRevenue { get; set; }
    }
    
    
}