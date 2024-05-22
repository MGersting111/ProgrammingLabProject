using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace api.Dto
{
    public class StoreInfo
    {
        public string StoreId { get; set; }
        public int OrderCount { get; set; }
        public double TotalRevenue { get; set; }
        public int CustomerCount { get; set; }

        public double RevenuePerCustomer { get; set; }

    }
}