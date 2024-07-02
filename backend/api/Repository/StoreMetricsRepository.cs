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
    public class StoreMetricsRepository : IStoreMetricsRepository
    {
        private readonly ApplicationDBContext _context;

        public StoreMetricsRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<StoreMetrics>> GetAllStoreMetricsAsync(DateTime startDate, DateTime endDate)
{
    var storeMetricsList = new List<StoreMetrics>();

    var storeGroups = await _context.Orders
        .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
        .Include(o => o.Store) // Assuming there's a navigation property to Store
        .GroupBy(o => o.StoreId)
        .ToListAsync();

    foreach (var storeGroup in storeGroups)
    {
        var store = storeGroup.First().Store;
        var storeMetrics = new StoreMetrics
        {
            StoreId = storeGroup.Key,
            State = store.State,
            City = store.City,
            TotalSales = storeGroup.Count(),
            TotalRevenue = storeGroup.Sum(o => o.total),
            AvgRevenue = storeGroup.Average(o => o.total),
            AvgSales = storeGroup.Count() > 0 ? storeGroup.Average(o => o.NItems) : 0,
            TotalCustomers = storeGroup.Select(o => o.CustomerId).Distinct().Count(),
            MonthlySales = storeGroup
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .GroupBy(g => g.Key.Year)
                .ToDictionary(
                    yearGroup => yearGroup.Key.ToString(),
                    yearGroup => yearGroup.ToDictionary(
                        monthGroup => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthGroup.Key.Month),
                        monthGroup => monthGroup.Count())),
            MonthlyRevenue = storeGroup
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .GroupBy(g => g.Key.Year)
                .ToDictionary(
                    yearGroup => yearGroup.Key.ToString(),
                    yearGroup => yearGroup.ToDictionary(
                        monthGroup => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthGroup.Key.Month),
                        monthGroup => monthGroup.Sum(o => o.total))),
            MonthlyAvgRevenuePerSale = storeGroup
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .GroupBy(g => g.Key.Year)
                .ToDictionary(
                    yearGroup => yearGroup.Key.ToString(),
                    yearGroup => yearGroup.ToDictionary(
                        monthGroup => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthGroup.Key.Month),
                        monthGroup => monthGroup.Average(o => o.total))),
            MonthlyCustomers = storeGroup
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .GroupBy(g => g.Key.Year)
                .ToDictionary(
                    yearGroup => yearGroup.Key.ToString(),
                    yearGroup => yearGroup.ToDictionary(
                        monthGroup => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthGroup.Key.Month),
                        monthGroup => monthGroup.Select(o => o.CustomerId).Distinct().Count())),
            ProductSales = storeGroup
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi != null && oi.Product != null)
                .GroupBy(oi => oi.Product.SKU)
                .Select(g => new ProductSales
                {
                    ProductSKU = g.Key,
                    ProductName = g.First().Product.Name,
                    TotalSales = g.Select(oi => oi.OrderId).Distinct().Count(),
                    TotalRevenue = g.Sum(oi => oi.Order.total)
                    // Add additional fields as necessary
                }).ToList(),


            MonthlyProductSales = storeGroup
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi != null && oi.Product != null)
                .GroupBy(oi => new { oi.Product.SKU, oi.Product.Name })
                .Select(g => new MonthlyProductSales
                {
                    ProductSKU = g.Key.SKU,
                    ProductName = g.Key.Name,
                    Sales = g.GroupBy(oi => new { oi.Order.OrderDate.Year, oi.Order.OrderDate.Month })
                             .OrderBy(oiGroup => oiGroup.Key.Year).ThenBy(oiGroup => oiGroup.Key.Month)
                             .GroupBy(oiGroup => oiGroup.Key.Year)
                             .ToDictionary(
                                 yearGroup => yearGroup.Key.ToString(),
                                 yearGroup => yearGroup.ToDictionary(
                                     monthGroup => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthGroup.Key.Month),
                                     monthGroup => monthGroup.Count()))
                }).ToList()
        };

        storeMetricsList.Add(storeMetrics);
    }

    return storeMetricsList;
}
    }
}