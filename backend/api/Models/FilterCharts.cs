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
    public class FilterCharts
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? Limit { get; set; }
        public List<string> Metrics { get; set; }
    }
}