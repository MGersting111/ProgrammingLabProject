using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using api.Models;

namespace api.Models
{
    public class Store
    {
        public string StoreId { get; set; } = string.Empty;
        public int Zipcode { get; set; }
        public string State_abbr { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public double Distance {get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}