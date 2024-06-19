using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Models;

namespace api.Dto
{
    public class StoreChartsInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StoreId { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public double TotalRevenue { get; set; }
        public int TotalSales { get; set; }
        public double AvgRevenue { get; set; }
        public double AvgSales { get; set; }
        public int TotalCustomers { get; set; }
         public List<ProductSalesInfo> ProductSales { get; set; } = new List<ProductSalesInfo>();

         public Dictionary<string, int> MonthlySales { get; set; } = new Dictionary<string, int>();
         public Dictionary<string, double> MonthlyRevenue { get; set; } = new Dictionary<string, double>();
         public Dictionary<string, double> MonthlyAvgRevenuePerSale { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, int> MonthlyCustomers { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, Dictionary<string, int>> MonthlyProductSales { get; set; } = new Dictionary<string, Dictionary<string, int>>();
    
    }
}