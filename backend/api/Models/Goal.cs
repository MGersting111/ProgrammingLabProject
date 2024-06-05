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
    public class Goal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CustomerGoal { get; set; }
        public double RevenueGoal { get; set; }
        public int SalesGoal { get; set; }
        public int ProductGoal { get; set; }

    }
}