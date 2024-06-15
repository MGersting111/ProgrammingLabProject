using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using api.Models;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;



namespace api.Dto
{
    public class CorrelationRequestDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string FirstModel { get; set; }
        public string XAttribute { get; set; }
        public string YAttribute { get; set; }

    public string? Size { get; set; }

    public string? Category { get; set; }
    
    }
}