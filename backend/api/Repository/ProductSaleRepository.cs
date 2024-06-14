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
        ProductSalesByMonth = new Dictionary<int, Dictionary<string, int>>(),
        ProductSalesBySize = new Dictionary<string, int>(),
        ProductSalesByCategory = new Dictionary<string, int>()
    };

    // Verkaufszahlen nach Monat und Jahr
    var salesByMonth = await _context.OrderItems
        .Where(orderItem => orderItem.Order.OrderDate >= fromDate && orderItem.Order.OrderDate <= toDate)
        .GroupBy(orderItem => new { orderItem.Order.OrderDate.Year, orderItem.Order.OrderDate.Month })
        .Select(group => new 
        {
            Year = group.Key.Year,
            Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(group.Key.Month),
            TotalSales = group.Count()
        })
        .ToListAsync();

    foreach (var sale in salesByMonth)
    {
        if (!productSaleInfo.ProductSalesByMonth.ContainsKey(sale.Year))
        {
            productSaleInfo.ProductSalesByMonth[sale.Year] = new Dictionary<string, int>();
        }
        productSaleInfo.ProductSalesByMonth[sale.Year][sale.Month] = sale.TotalSales;
    }

    // Verkaufszahlen nach Größe
    var salesBySize = await _context.OrderItems
        .Where(orderItem => orderItem.Order.OrderDate >= fromDate && orderItem.Order.OrderDate <= toDate)
        .GroupBy(orderItem => orderItem.Product.Size)
        .Select(group => new 
        {
            Size = group.Key,
            TotalSales = group.Count()
        })
        .ToListAsync();

    foreach (var sale in salesBySize)
    {
        productSaleInfo.ProductSalesBySize[sale.Size] = sale.TotalSales;
    }

    // Verkaufszahlen nach Kategorie
    var salesByCategory = await _context.OrderItems
        .Where(orderItem => orderItem.Order.OrderDate >= fromDate && orderItem.Order.OrderDate <= toDate)
        .GroupBy(orderItem => orderItem.Product.Category)
        .Select(group => new 
        {
            Category = group.Key,
            TotalSales = group.Count()
        })
        .ToListAsync();

    foreach (var sale in salesByCategory)
    {
        productSaleInfo.ProductSalesByCategory[sale.Category] = sale.TotalSales;
    }

    return productSaleInfo;
}





}
}
