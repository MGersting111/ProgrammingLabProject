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
    public class StoreDataDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Attribute { get; set; }

        // public string State { get; set; }
        // public string City { get; set; }
        // public double Latitude { get; set; }
        // public double Longitude { get; set; }
        // public decimal  Revenue { get; set; }
        // public int TotalCustomers { get; set; }
    }
}