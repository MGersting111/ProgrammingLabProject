using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Dto
{
    public class StoreMetrics
    {
        public string StoreId { get; set; }
        public Dictionary<string, YearMetrics> MetricsByYear { get; set; } = new Dictionary<string, YearMetrics>();
        public int TotalSales { get; set; }
        public double TotalRevenue { get; set; }
        public double AvgRevenuePerSale { get; set; }
        public double AvgSalesPerCustomer { get; set; }
        public Dictionary<string, int> CustomerCountPerMonth { get; set; } = new Dictionary<string, int>();
        public List<ProductSales> ProductList { get; set; } = new List<ProductSales>();
    }

    public class YearMetrics
    {
        public Dictionary<string, int> Metrics { get; set; } = new Dictionary<string, int>();
        public int Total { get; set; }
    }

    public class ProductSales
    {
        public string SKU { get; set; }
        public int SalesCount { get; set; }
        public double Revenue { get; set; }
    }
}