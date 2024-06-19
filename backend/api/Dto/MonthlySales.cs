using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Models;

namespace api.Dto
{
    public class MonthlySales
    {
    public int  Month { get; set; } 
    public int Sales { get; set; }
    }
}