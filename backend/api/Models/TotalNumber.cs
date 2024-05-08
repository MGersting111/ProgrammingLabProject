using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class TotalNumbers
    {
        public float TotalOrder { get; set; }
        public float TotalCustomer { get; set; }
        public float TotalOrderItem { get; set; }
        public float TotalProduct  { get; set; }
        public float TotalStore { get; set; }
    }
}