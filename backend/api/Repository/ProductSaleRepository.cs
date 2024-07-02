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
                ProductSalesBySize = new Dictionary<string, SortedDictionary<string, SortedDictionary<string, int>>>(),
                ProductSalesByCategory = new Dictionary<string, SortedDictionary<string, SortedDictionary<string, int>>>(),
                TotalSales = 0,
                TotalRevenue = 0
            };

            // Number of months in the given period
            int numMonths = ((toDate.Year - fromDate.Year) * 12) + toDate.Month - fromDate.Month + 1;

            // Fetching sales data for months, sizes, and categories in one go
            var salesData = await _context.OrderItems
                .Where(orderItem => orderItem.Order.OrderDate >= fromDate && orderItem.Order.OrderDate <= toDate)
                .Select(orderItem => new
                {
                    Year = orderItem.Order.OrderDate.Year,
                    Month = orderItem.Order.OrderDate.Month,
                    Size = orderItem.Product.Size,
                    Category = orderItem.Product.Category,
                    TotalSales = 1, // Count each order item as one sale
                    TotalRevenue = orderItem.Order.total, // Assuming this is the revenue for the order item
                })
                .ToListAsync();

            var totalSalesByYear = new Dictionary<string, int>();

            foreach (var data in salesData)
            {
                var year = data.Year.ToString();
                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(data.Month);

                // Populate ProductSalesByMonth and ProductRevenue
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

                // Populate ProductSalesBySize
                if (!productSaleInfo.ProductSalesBySize.ContainsKey(data.Size))
                {
                    productSaleInfo.ProductSalesBySize[data.Size] = new SortedDictionary<string, SortedDictionary<string, int>>();
                }
                if (!productSaleInfo.ProductSalesBySize[data.Size].ContainsKey(year))
                {
                    productSaleInfo.ProductSalesBySize[data.Size][year] = new SortedDictionary<string, int>(Comparer<string>.Create((x, y) => monthsOrder.IndexOf(x).CompareTo(monthsOrder.IndexOf(y))));
                }
                productSaleInfo.ProductSalesBySize[data.Size][year][monthName] = productSaleInfo.ProductSalesBySize[data.Size][year].GetValueOrDefault(monthName, 0) + data.TotalSales;

                // Populate ProductSalesByCategory (with month sorting)
                if (!productSaleInfo.ProductSalesByCategory.ContainsKey(data.Category))
                {
                    productSaleInfo.ProductSalesByCategory[data.Category] = new SortedDictionary<string, SortedDictionary<string, int>>();
                }
                if (!productSaleInfo.ProductSalesByCategory[data.Category].ContainsKey(year))
                {
                    productSaleInfo.ProductSalesByCategory[data.Category][year] = new SortedDictionary<string, int>(Comparer<string>.Create((x, y) => monthsOrder.IndexOf(x).CompareTo(monthsOrder.IndexOf(y))));
                }
                productSaleInfo.ProductSalesByCategory[data.Category][year][monthName] = productSaleInfo.ProductSalesByCategory[data.Category][year].GetValueOrDefault(monthName, 0) + data.TotalSales;
                     productSaleInfo.TotalSales += data.TotalSales;
            productSaleInfo.TotalRevenue += (int)data.TotalRevenue;

            // Update TotalSize and TotalCategory
            productSaleInfo.TotalSize[data.Size] = productSaleInfo.TotalSize.GetValueOrDefault(data.Size, 0) + (int)data.TotalSales;
            productSaleInfo.TotalCategory[data.Category] = productSaleInfo.TotalCategory.GetValueOrDefault(data.Category, 0) + (int)data.TotalSales;
            }

       

            // Add totals to ProductSalesByMonth and ProductRevenue
            foreach (var year in productSaleInfo.ProductSalesByMonth.Keys)
            {
                productSaleInfo.ProductSalesByMonth[year]["Total"] = totalSalesByYear[year];
                productSaleInfo.ProductRevenue[year]["TotalRevenue"] = productSaleInfo.ProductRevenue[year].Values.Sum();
            }

            // Add totals to ProductSalesBySize
            foreach (var size in productSaleInfo.ProductSalesBySize.Keys)
            {
                foreach (var year in productSaleInfo.ProductSalesBySize[size].Keys)
                {
                    productSaleInfo.ProductSalesBySize[size][year]["Total"] = productSaleInfo.ProductSalesBySize[size][year].Values.Sum();
                }
            }

            // Add totals to ProductSalesByCategory
            foreach (var category in productSaleInfo.ProductSalesByCategory.Keys)
            {
                foreach (var year in productSaleInfo.ProductSalesByCategory[category].Keys)
                {
                    productSaleInfo.ProductSalesByCategory[category][year]["Total"] = productSaleInfo.ProductSalesByCategory[category][year].Values.Sum();
                }
            }

            return productSaleInfo;
        }
    }
}
