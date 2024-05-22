using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using api.Models;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace api.Models
{
    public class FilterRevenue
    {
        public string? StoreId { get; set; } = null;
        public double? StoreRevenues { get; set; }
        public int? StoreOrderCounts { get; set; }
        public int? StoreCustomerCounts { get; set; }
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
        public double? RevenuePerCustomer { get; set; }
        public string? Category { get; set; }
    }
}