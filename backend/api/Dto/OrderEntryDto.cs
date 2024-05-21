using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;


namespace api.Dto
{
    public class OrderEntryDto 
    {
        public int? OrderId { get; set; }
        public int? NItems { get; set; }
        public double? Total { get; set; }

        
    
    }
}