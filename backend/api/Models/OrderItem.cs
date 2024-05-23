using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace api.Models
{
    public class OrderItem
   {
       public int OrderId { get; set; }
       public string SKU { get; set; } = string.Empty;
       public Product? Product { get; set;}
       public Order? Order { get; set; }

      


    }
 }