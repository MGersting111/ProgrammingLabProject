namespace api.Dto
{
    public class StoreMetrics
    {
        public string StoreId { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int TotalSales { get; set; }
        public double TotalRevenue { get; set; }
        public double AvgRevenue { get; set; }
        public double AvgSales { get; set; }
        public int TotalCustomers { get; set; }
        public List<ProductSales> ProductSales { get; set; } = new List<ProductSales>();
        public Dictionary<string, Dictionary<string, int>> MonthlySales { get; set; } = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, Dictionary<string, double>> MonthlyRevenue { get; set; } = new Dictionary<string, Dictionary<string, double>>();
        public Dictionary<string, Dictionary<string, double>> MonthlyAvgRevenuePerSale { get; set; } = new Dictionary<string, Dictionary<string, double>>();
        public Dictionary<string, Dictionary<string, int>> MonthlyCustomers { get; set; } = new Dictionary<string, Dictionary<string, int>>();
        public List<MonthlyProductSales> MonthlyProductSales { get; set; } = new List<MonthlyProductSales>();
    }

    public class ProductSales
    {
        public string ProductSKU { get; set; }
        public string ProductName { get; set; }
        public int TotalSales { get; set; }
        public double TotalRevenue { get; set; }
    }

    public class MonthlyProductSales
    {
        public string ProductSKU { get; set; }
        public string ProductName { get; set; }
        public Dictionary<string, Dictionary<string, int>> Sales { get; set; } = new Dictionary<string, Dictionary<string, int>>();
    }
}