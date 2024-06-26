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
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDBContext _context;

        public DashboardRepository(ApplicationDBContext context)
        {
            _context = context;
        }

       public async Task<DashboardDto> GetDashboardDataAsync(DateTime startDate, DateTime endDate)
{
    _context.Database.SetCommandTimeout(300);

    var dashboardDto = new DashboardDto();
    var monthsOrder = new List<string> { "Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez" };

    // Fetching order statistics grouped by Year, Month, and StoreId
    var orderStats = await _context.Orders
        .AsNoTracking()
        .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
        .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month, o.StoreId })
        .Select(g => new
        {
            Year = g.Key.Year,
            Month = g.Key.Month,
            g.Key.StoreId,
            Total = g.Sum(o => o.total)
        })
        .ToListAsync();

    // 1. Revenue by Year and Month:
   dashboardDto.RevenueByYearAndMonth = orderStats
    .GroupBy(o => o.Year)
    .ToDictionary(
        yearGroup => yearGroup.Key,
        yearGroup => yearGroup
            .GroupBy(monthGroup => monthGroup.Month)
            .OrderBy(monthGroup => monthsOrder.IndexOf(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(monthGroup.Key)))
            .ToDictionary(
                monthGroup => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(monthGroup.Key),
                monthGroup => monthGroup.Sum(o => o.Total)
            )
    );

    // Adding yearly totals to the dictionary
    foreach (var year in dashboardDto.RevenueByYearAndMonth.Keys.ToList())
{
    var total = dashboardDto.RevenueByYearAndMonth[year].Sum(x => x.Value);
    dashboardDto.RevenueByYearAndMonth[year].Add("Total", total);
}


            // 2. Average Revenue Per Store:
            dashboardDto.AvgRevenuePerStore = orderStats
    .GroupBy(o => o.StoreId)
    .Select(g => g.Average(o => o.Total))
    .DefaultIfEmpty(0)
    .Average();

            // 3. Best Selling Product (by SKU):
            // Simplified the query by removing unnecessary OrderByDescending and FirstOrDefault
            dashboardDto.BestSellingProduct = await _context.OrderItems.AsNoTracking()
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .GroupBy(oi => oi.Product.SKU)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            // 4. Most Purchased Size and Category:
            // Combined two separate queries into one to reduce database round trips
            var sizeAndCategory = await _context.OrderItems.AsNoTracking()
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .GroupBy(oi => new { oi.Product.Size, oi.Product.Category })
                .OrderByDescending(g => g.Count())
                .Select(g => new { g.Key.Size, g.Key.Category })
                .FirstOrDefaultAsync();

            if (sizeAndCategory != null)
            {
                dashboardDto.MostPurchasedSize = sizeAndCategory.Size;
                dashboardDto.MostPurchasedCategory = sizeAndCategory.Category;
            }

            // 5. Average Customers and Average Sales:
            // Simplified the query by calculating averages directly in the query
            var orderStatsAvg = await _context.Orders.AsNoTracking()
         .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
         .GroupBy(o => 1) // Gruppierung nach einem konstanten Wert, um Aggregationen über das gesamte Set durchzuführen
         .Select(g => new
         {
             AverageCustomers = g.Select(o => o.CustomerId).Distinct().Count() / (double)g.Count(),
             AverageSales = g.Count() / (endDate - startDate).TotalDays
         })
         .FirstOrDefaultAsync();

            dashboardDto.AverageCustomers = orderStatsAvg?.AverageCustomers ?? 0;
            dashboardDto.AverageSales = orderStatsAvg?.AverageSales ?? 0;

            // 6. Top 3 Stores, Products, and Customers:
            dashboardDto.Top3Stores = await _context.Orders
                 .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                 .GroupBy(o => o.StoreId)
                 .OrderByDescending(g => g.Sum(o => o.total))
                 .Take(3)
                 .Select(g => new Dashboard { StoreId = g.Key, TotalRevenue = g.Sum(o => o.total) })
                 .ToListAsync();

            dashboardDto.Top3Products = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .GroupBy(oi => oi.Product.SKU)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => new Dashboard { SKU = g.Key, TotalOrders = g.Count() })
                .ToListAsync();

            dashboardDto.Top3Customers = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => o.CustomerId)
                .OrderByDescending(g => g.Sum(o => o.total))
                .Take(3)
                .Select(g => new Dashboard { CustomerId = g.Key, TotalSpent = g.Sum(o => o.total) })
                .ToListAsync();

            return dashboardDto;
        }
    }
}
