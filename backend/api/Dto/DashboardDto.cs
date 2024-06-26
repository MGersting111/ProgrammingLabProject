using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Dto
{
    public class DashboardDto
    {
          public Dictionary<string, double> RevenueByMonth { get; set; }
        public double AvgRevenuePerStore { get; set; }
        public string BestSellingProduct { get; set; }
        public string MostPurchasedSize { get; set; }
        public string MostPurchasedCategory { get; set; }
        public double AverageCustomers { get; set; }
        public double AverageSales { get; set; }
        public List<Dashboard> Top3Stores { get; set; }
        public List<Dashboard> Top3Products { get; set; }
        public List<Dashboard> Top3Customers { get; set; }
    }
}