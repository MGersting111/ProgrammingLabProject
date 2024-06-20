using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Data;
using api.Models;
using api.Dto;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace api.Repository
{
    public class ProductSaleRepository : IProductSaleRepository
    {
        private readonly ApplicationDBContext _context;

        public ProductSaleRepository(ApplicationDBContext context)
        {
            _context = context;
        }




public async Task<ProductSaleInfo> GetProductSaleInfoAsync(DateTime fromDate, DateTime toDate)
{
    var productSaleInfo = new ProductSaleInfo
    {
        FromDate = fromDate,
        ToDate = toDate,
        ProductSalesByMonth = new Dictionary<string, SortedDictionary<string, int>>(),
        ProductRevenue = new Dictionary<string, SortedDictionary<string, double>>(),
        ProductSalesBySize = new Dictionary<string, Dictionary<string, int>>(),
        ProductSalesByCategory = new Dictionary<string, Dictionary<string, int>>(),
        TotalSales = 0,
        TotalRevenue = 0.0 
    };

    // Anzahl der Monate im angegebenen Zeitraum
    int numMonths = ((toDate.Year - fromDate.Year) * 12) + toDate.Month - fromDate.Month + 1;

    // Verkaufszahlen und Umsatz nach Monat und Jahr
    var salesByMonth = await _context.OrderItems
        .Include(oi => oi.Order)
        .Where(orderItem => orderItem.Order.OrderDate >= fromDate && orderItem.Order.OrderDate <= toDate)
        .GroupBy(orderItem => new { orderItem.Order.OrderDate.Year, orderItem.Order.OrderDate.Month })
        .Select(group => new
        {
            Year = group.Key.Year.ToString(),
            Month = group.Key.Month,
            TotalSales = group.Count(),
            TotalRevenue = group.Sum(oi => oi.Order.total)
        })
        .ToListAsync();


    foreach (var sale in salesByMonth)
    {
        string year = sale.Year;
        string monthName = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(sale.Month);

        // Verkaufszahlen
        if (!productSaleInfo.ProductSalesByMonth.ContainsKey(year))
            productSaleInfo.ProductSalesByMonth[year] = new SortedDictionary<string, int>();
        productSaleInfo.ProductSalesByMonth[year][monthName] = sale.TotalSales;

        // Umsatz
        if (!productSaleInfo.ProductRevenue.ContainsKey(year))
            productSaleInfo.ProductRevenue[year] = new SortedDictionary<string, double>();
        productSaleInfo.ProductRevenue[year][monthName] = sale.TotalRevenue;

        productSaleInfo.TotalSales += sale.TotalSales;
        productSaleInfo.TotalRevenue += sale.TotalRevenue;
    }
    
        // Durchschnittsberechnungen hinzufügen
    foreach (var year in productSaleInfo.ProductSalesByMonth.Keys.ToList())
    {
        productSaleInfo.ProductSalesByMonth[year]["Total"] = productSaleInfo.ProductSalesByMonth[year].Values.Sum();
        productSaleInfo.ProductSalesByMonth[year]["Durchschnitt Sales"] = (int)(productSaleInfo.ProductSalesByMonth[year]["Total"] / (double)numMonths); 

        productSaleInfo.ProductRevenue[year]["TotalRevenue"] = productSaleInfo.ProductRevenue[year].Values.Sum();
        productSaleInfo.ProductRevenue[year]["Durchschnittlicher Umsatz"] = productSaleInfo.ProductRevenue[year]["TotalRevenue"] / numMonths; 
    }


    // Verkaufszahlen nach Größe und Jahr
    var salesBySize = await _context.OrderItems
        .Where(orderItem => orderItem.Order.OrderDate >= fromDate && orderItem.Order.OrderDate <= toDate)
        .GroupBy(orderItem => new { orderItem.Order.OrderDate.Year, orderItem.Product.Size })
        .Select(group => new
        {
            Year = group.Key.Year.ToString(),
            Size = group.Key.Size,
            TotalSales = group.Count()
        })
        .ToListAsync();


    foreach (var sale in salesBySize)
    {
        if (!productSaleInfo.ProductSalesBySize.ContainsKey(sale.Year))
        {
            productSaleInfo.ProductSalesBySize[sale.Year] = new Dictionary<string, int>();
        }
        productSaleInfo.ProductSalesBySize[sale.Year][sale.Size] = sale.TotalSales;
    }

    // Verkaufszahlen nach Kategorie und Jahr
    var salesByCategory = await _context.OrderItems
        .Where(orderItem => orderItem.Order.OrderDate >= fromDate && orderItem.Order.OrderDate <= toDate)
        .GroupBy(orderItem => new { orderItem.Order.OrderDate.Year, orderItem.Product.Category })
        .Select(group => new
        {
            Year = group.Key.Year.ToString(),
            Category = group.Key.Category,
            TotalSales = group.Count()
        })
        .ToListAsync();

    foreach (var sale in salesByCategory)
    {
        if (!productSaleInfo.ProductSalesByCategory.ContainsKey(sale.Year))
        {
            productSaleInfo.ProductSalesByCategory[sale.Year] = new Dictionary<string, int>();
        }
        productSaleInfo.ProductSalesByCategory[sale.Year][sale.Category] = sale.TotalSales;
    }

    return productSaleInfo;
}





    }
}
