using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Models;

namespace api.Dto
{
    public class ProductSaleInfo
    {
        public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Dictionary<string, SortedDictionary<string, int>> ProductSalesByMonth { get; set; }
    public Dictionary<string, SortedDictionary<string, double>> ProductRevenue { get; set; } // Neu
    public Dictionary<string, Dictionary<string, int>> ProductSalesBySize { get; set; }
    public Dictionary<string, Dictionary<string, int>> ProductSalesByCategory { get; set; }
    public int TotalSales { get; set; }
    public double TotalRevenue { get; set; }
    }
}