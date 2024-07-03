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
        public Dictionary<string, SortedDictionary<string, int>> ProductSalesByMonth { get; set; } = new();
        public Dictionary<string, SortedDictionary<string, int>> ProductRevenue { get; set; } = new();
        public Dictionary<string, SortedDictionary<string, SortedDictionary<string, int>>> ProductRevenueBySize { get; set; }
        public Dictionary<string, SortedDictionary<string, SortedDictionary<string, int>>> ProductRevenueByCategory { get; set; }
        public Dictionary<string, SortedDictionary<string, SortedDictionary<string, int>>> ProductSalesBySize { get; set; } = new();
        public Dictionary<string, SortedDictionary<string, SortedDictionary<string, int>>> ProductSalesByCategory { get; set; } = new();

        public int TotalSales { get; set; }
        public int TotalRevenue { get; set; }
        public Dictionary<string, int> TotalSize { get; set; } = new(); // Total sales per size
        public Dictionary<string, int> TotalCategory { get; set; } = new();
        public Dictionary<string, double> AvgSalesBySize { get; set; } 
        public Dictionary<string, double> AvgSalesByCategory { get; set; }
        public Dictionary<string, int> TotalRevenueBySize { get; set; }
        public Dictionary<string, int> TotalRevenueByCategory { get; set; }
        public Dictionary<string, double> AvgRevenueBySize { get; set; }
        public Dictionary<string, double> AvgRevenueByCategory { get; set; }
        
    }
}