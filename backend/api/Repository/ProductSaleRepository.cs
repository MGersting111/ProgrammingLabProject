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
            var monthsOrder = new List<string> { "Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez" };

            var productSaleInfo = new ProductSaleInfo
            {
                FromDate = fromDate,
                ToDate = toDate,
                ProductSalesByMonth = new Dictionary<string, SortedDictionary<string, int>>(),
                ProductRevenue = new Dictionary<string, SortedDictionary<string, int>>(),
                ProductRevenueBySize = new Dictionary<string, SortedDictionary<string, SortedDictionary<string, int>>>(), 
        ProductRevenueByCategory = new Dictionary<string, SortedDictionary<string, SortedDictionary<string, int>>>(), 
                ProductSalesBySize = new Dictionary<string, SortedDictionary<string, SortedDictionary<string, int>>>(),
                ProductSalesByCategory = new Dictionary<string, SortedDictionary<string, SortedDictionary<string, int>>>(),
                TotalSales = 0,
                TotalRevenue = 0,
                TotalSize = new Dictionary<string, int>(),
                TotalCategory = new Dictionary<string, int>(),
                AvgSalesBySize = new Dictionary<string, double>(),
                AvgSalesByCategory = new Dictionary<string, double>(),
                TotalRevenueBySize = new Dictionary<string, int>(),
                TotalRevenueByCategory = new Dictionary<string, int>(),
                AvgRevenueBySize = new Dictionary<string, double>(),
                AvgRevenueByCategory = new Dictionary<string, double>()
            
            };

            int numMonths = ((toDate.Year - fromDate.Year) * 12) + toDate.Month - fromDate.Month + 1;

            var salesData = await _context.OrderItems
                .Where(orderItem => orderItem.Order.OrderDate >= fromDate && orderItem.Order.OrderDate <= toDate)
                .Select(orderItem => new
                {
                    Year = orderItem.Order.OrderDate.Year,
                    Month = orderItem.Order.OrderDate.Month,
                    Size = orderItem.Product.Size,
                    Category = orderItem.Product.Category,
                    TotalSales = 1,
                    TotalRevenue = orderItem.Order.total,
                })
                .ToListAsync();

            var totalSalesByYear = new Dictionary<string, int>();

             foreach (var data in salesData)
    {
        var year = data.Year.ToString();
        var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(data.Month);

        if (!productSaleInfo.ProductSalesByMonth.ContainsKey(year))
        {
            productSaleInfo.ProductSalesByMonth[year] = new SortedDictionary<string, int>(Comparer<string>.Create((x, y) => monthsOrder.IndexOf(x).CompareTo(monthsOrder.IndexOf(y))));
        }
        productSaleInfo.ProductSalesByMonth[year][monthName] = productSaleInfo.ProductSalesByMonth[year].GetValueOrDefault(monthName, 0) + data.TotalSales;

        if (!productSaleInfo.ProductRevenue.ContainsKey(year))
        {
            productSaleInfo.ProductRevenue[year] = new SortedDictionary<string, int>(Comparer<string>.Create((x, y) => monthsOrder.IndexOf(x).CompareTo(monthsOrder.IndexOf(y))));
        }
        productSaleInfo.ProductRevenue[year][monthName] = productSaleInfo.ProductRevenue[year].GetValueOrDefault(monthName, 0) + (int)data.TotalRevenue;
        totalSalesByYear[year] = totalSalesByYear.GetValueOrDefault(year, 0) + data.TotalSales;

        // Process ProductSalesBySize and ProductRevenueBySize
        if (!productSaleInfo.ProductSalesBySize.ContainsKey(data.Size))
        {
            productSaleInfo.ProductSalesBySize[data.Size] = new SortedDictionary<string, SortedDictionary<string, int>>();
        }
        if (!productSaleInfo.ProductSalesBySize[data.Size].ContainsKey(year))
        {
            productSaleInfo.ProductSalesBySize[data.Size][year] = new SortedDictionary<string, int>(Comparer<string>.Create((x, y) => monthsOrder.IndexOf(x).CompareTo(monthsOrder.IndexOf(y))));
        }
        productSaleInfo.ProductSalesBySize[data.Size][year][monthName] = productSaleInfo.ProductSalesBySize[data.Size][year].GetValueOrDefault(monthName, 0) + data.TotalSales;

        if (!productSaleInfo.ProductRevenueBySize.ContainsKey(data.Size))
        {
            productSaleInfo.ProductRevenueBySize[data.Size] = new SortedDictionary<string, SortedDictionary<string, int>>();
        }
        if (!productSaleInfo.ProductRevenueBySize[data.Size].ContainsKey(year))
        {
            productSaleInfo.ProductRevenueBySize[data.Size][year] = new SortedDictionary<string, int>(Comparer<string>.Create((x, y) => monthsOrder.IndexOf(x).CompareTo(monthsOrder.IndexOf(y))));
        }
        productSaleInfo.ProductRevenueBySize[data.Size][year][monthName] = productSaleInfo.ProductRevenueBySize[data.Size][year].GetValueOrDefault(monthName, 0) + (int)data.TotalRevenue;

        // Process ProductSalesByCategory and ProductRevenueByCategory
        if (!productSaleInfo.ProductSalesByCategory.ContainsKey(data.Category))
        {
            productSaleInfo.ProductSalesByCategory[data.Category] = new SortedDictionary<string, SortedDictionary<string, int>>();
        }
        if (!productSaleInfo.ProductSalesByCategory[data.Category].ContainsKey(year))
        {
            productSaleInfo.ProductSalesByCategory[data.Category][year] = new SortedDictionary<string, int>(Comparer<string>.Create((x, y) => monthsOrder.IndexOf(x).CompareTo(monthsOrder.IndexOf(y))));
        }
        productSaleInfo.ProductSalesByCategory[data.Category][year][monthName] = productSaleInfo.ProductSalesByCategory[data.Category][year].GetValueOrDefault(monthName, 0) + data.TotalSales;

        if (!productSaleInfo.ProductRevenueByCategory.ContainsKey(data.Category))
        {
            productSaleInfo.ProductRevenueByCategory[data.Category] = new SortedDictionary<string, SortedDictionary<string, int>>();
        }
        if (!productSaleInfo.ProductRevenueByCategory[data.Category].ContainsKey(year))
        {
            productSaleInfo.ProductRevenueByCategory[data.Category][year] = new SortedDictionary<string, int>(Comparer<string>.Create((x, y) => monthsOrder.IndexOf(x).CompareTo(monthsOrder.IndexOf(y))));
        }
        productSaleInfo.ProductRevenueByCategory[data.Category][year][monthName] = productSaleInfo.ProductRevenueByCategory[data.Category][year].GetValueOrDefault(monthName, 0) + (int)data.TotalRevenue;

        productSaleInfo.TotalSales += data.TotalSales;
        productSaleInfo.TotalRevenue += (int)data.TotalRevenue;

        productSaleInfo.TotalSize[data.Size] = productSaleInfo.TotalSize.GetValueOrDefault(data.Size, 0) + (int)data.TotalSales;
        productSaleInfo.TotalCategory[data.Category] = productSaleInfo.TotalCategory.GetValueOrDefault(data.Category, 0) + (int)data.TotalSales;
        productSaleInfo.TotalRevenueBySize[data.Size] = productSaleInfo.TotalRevenueBySize.GetValueOrDefault(data.Size, 0) + (int)data.TotalRevenue;
        productSaleInfo.TotalRevenueByCategory[data.Category] = productSaleInfo.TotalRevenueByCategory.GetValueOrDefault(data.Category, 0) + (int)data.TotalRevenue;
    }

    foreach (var year in productSaleInfo.ProductSalesByMonth.Keys)
    {
        productSaleInfo.ProductSalesByMonth[year]["Total"] = totalSalesByYear[year];
        productSaleInfo.ProductRevenue[year]["TotalRevenue"] = productSaleInfo.ProductRevenue[year].Values.Sum();
    }

    foreach (var size in productSaleInfo.ProductSalesBySize.Keys)
    {
        foreach (var year in productSaleInfo.ProductSalesBySize[size].Keys)
        {
            productSaleInfo.ProductSalesBySize[size][year]["Total"] = productSaleInfo.ProductSalesBySize[size][year].Values.Sum();
            productSaleInfo.ProductRevenueBySize[size][year]["Total"] = productSaleInfo.ProductRevenueBySize[size][year].Values.Sum();
        }
    }

    foreach (var category in productSaleInfo.ProductSalesByCategory.Keys)
    {
        foreach (var year in productSaleInfo.ProductSalesByCategory[category].Keys)
        {
            productSaleInfo.ProductSalesByCategory[category][year]["Total"] = productSaleInfo.ProductSalesByCategory[category][year].Values.Sum();
            productSaleInfo.ProductRevenueByCategory[category][year]["Total"] = productSaleInfo.ProductRevenueByCategory[category][year].Values.Sum();
        }
    }

    // Calculate average revenue per size
    foreach (var size in productSaleInfo.TotalRevenueBySize.Keys)
    {
        productSaleInfo.AvgRevenueBySize[size] = productSaleInfo.TotalRevenueBySize[size] / (double)productSaleInfo.TotalSize[size];
    }

    // Calculate average revenue per category
    foreach (var category in productSaleInfo.TotalRevenueByCategory.Keys)
    {
        productSaleInfo.AvgRevenueByCategory[category] = productSaleInfo.TotalRevenueByCategory[category] / (double)productSaleInfo.TotalCategory[category];
    }

    // Calculate average sales per size
    foreach (var size in productSaleInfo.TotalSize.Keys)
    {
        productSaleInfo.AvgSalesBySize[size] = productSaleInfo.TotalSize[size] / (double)numMonths;
    }

    // Calculate average sales per category
    foreach (var category in productSaleInfo.TotalCategory.Keys)
    {
        productSaleInfo.AvgSalesByCategory[category] = productSaleInfo.TotalCategory[category] / (double)numMonths;
    }

    return productSaleInfo;
}
    }
}
