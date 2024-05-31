using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto
{
    public class ChartsInfo
    {

        public string? StoreId { get; set; }

        public Dictionary<string, double> Metrics { get; set; }
        //public List<DateTime> OrderDates { get; set; }
        
    }
}