using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto
{
    public class OrderEntry
    {
        public int? OrderId { get; set; }
        public int? NItems { get; set; }
        public double? Total { get; set; }
    }
}