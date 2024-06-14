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
        ProductSalesBySize = new Dictionary<string, Dictionary<string, int>>(),
        ProductSalesByCategory = new Dictionary<string, Dictionary<string, int>>(),
        TotalSales = 0
    };

    // Verkaufszahlen nach Monat und Jahr
    var salesByMonth = await _context.OrderItems
        .Where(orderItem => orderItem.Order.OrderDate >= fromDate && orderItem.Order.OrderDate <= toDate)
        .GroupBy(orderItem => new { orderItem.Order.OrderDate.Year, orderItem.Order.OrderDate.Month })
        .Select(group => new
        {
            Year = group.Key.Year.ToString(),
            Month = group.Key.Month,
            TotalSales = group.Count()
        })
        .ToListAsync();

    foreach (var sale in salesByMonth)
    {
        if (!productSaleInfo.ProductSalesByMonth.ContainsKey(sale.Year))
        {
            productSaleInfo.ProductSalesByMonth[sale.Year] = new SortedDictionary<string, int>();
        }
        var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(sale.Month);
        productSaleInfo.ProductSalesByMonth[sale.Year][monthName] = sale.TotalSales;
        productSaleInfo.TotalSales += sale.TotalSales;
    }

    // Add yearly total to each year
    foreach (var year in productSaleInfo.ProductSalesByMonth.Keys.ToList())
    {
        var yearlyTotal = productSaleInfo.ProductSalesByMonth[year].Values.Sum();
        productSaleInfo.ProductSalesByMonth[year].Add("Total", yearlyTotal); // Add total as a separate key
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
