using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Models;

namespace api.Dto
{
    public class ProductSaleInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<ProductSale> ProductSales { get; set; }

        
    }
}