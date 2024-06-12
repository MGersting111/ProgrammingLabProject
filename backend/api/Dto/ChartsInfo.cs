using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto
{
    public class ChartsInfo
    {
        public string? StoreId { get; set; }

        public Dictionary<int, YearlyMetrics> MetricsByYear { get; set; }

        public int Total { get; set; }
    }

    public class YearlyMetrics
    {
        public Dictionary<string, double> Metrics { get; set; }

        public int Total { get; set; }
    }
}
