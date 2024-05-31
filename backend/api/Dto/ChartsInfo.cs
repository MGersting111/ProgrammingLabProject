using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto
{
    public class ChartsInfo
    {

        public string? StoreId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public double TotalRevenue { get; set; }
        public int CustomerCount { get; set; }
        public double RevenuePerCustomer { get; set; }
        public int OrderCount { get; set; }
    }
}