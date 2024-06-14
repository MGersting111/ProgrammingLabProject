using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class CompareCharts
    {

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Metrics { get; set; } = string.Empty;

        public int Limit { get; set; }

    }
}