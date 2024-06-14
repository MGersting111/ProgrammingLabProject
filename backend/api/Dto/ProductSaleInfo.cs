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

        // Verkaufszahlen nach Monat und Jahr
        public Dictionary<int, Dictionary<string, int>> ProductSalesByMonth { get; set; }

        // Verkaufszahlen nach Größe
        public Dictionary<string, int> ProductSalesBySize { get; set; }

        // Verkaufszahlen nach Kategorie
        public Dictionary<string, int> ProductSalesByCategory { get; set; }
    }
}