using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using api.Models;
using api.Interfaces;

namespace api.Models
{
    public class FilterRevenue
    {
        public string? StoreId { get; set; } = null;
        public double? StoreRevenues { get; set; } 
    }
}