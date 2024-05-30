using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto
{
    public class StoreData
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string FirstModel { get; set; }
        public string XAttribute { get; set; }
        public string YAttribute { get; set; }
       
    }
}