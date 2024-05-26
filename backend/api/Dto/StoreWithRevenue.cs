using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using api.Models;
using api.Interfaces;

namespace api.Dto
{
    public class StoreWithRevenue
    {
        public string StoreId { get; set; } = string.Empty;
        public int Zipcode { get; set; }
        public string State_abbr { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public double Distance { get; set; }
        public double TotalRevenue { get; set; }
        public int CustomerCount { get; set; }
    }
}