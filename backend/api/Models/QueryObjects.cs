using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class QueryObject
    {
        public string? Symbol { get; set; } = null;

        public string? StoreiD { get; set; } = null;

        public float Revenue { get; set; }
    }
}