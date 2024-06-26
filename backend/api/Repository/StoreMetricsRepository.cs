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

        public async Task<List<StoreMetrics>> GetAllStoreMetricsAsync(DateTime fromDate, DateTime toDate)
        {
            var storeMetricsList = new List<StoreMetrics>();
            var monthsOrder = new List<string> { "Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez" };

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
                .ToListAsync();

            var groupedByStore = orders.GroupBy(o => o.StoreId);

            foreach (var storeGroup in groupedByStore)
        
            {
                var storeMetrics = new StoreMetrics
                {
                    StoreId = storeGroup.Key,
                    TotalSales = storeGroup.Count(),
                    TotalRevenue = storeGroup.Sum(o => o.OrderItems.Sum(oi => oi.Order.total)),
                    AvgRevenuePerSale = storeGroup.Average(o => o.OrderItems.Sum(oi => oi.Order.total)),
                    AvgSalesPerCustomer = storeGroup.GroupBy(o => o.CustomerId).Average(g => g.Count()),
                  CustomerCountPerMonth = storeGroup
    .Where(o => o != null) // Stelle sicher, dass o nicht null ist
    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
    .OrderBy(g => g.Key.Year)
    .ThenBy(g => monthsOrder.IndexOf(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month)))
    .ToDictionary(
        g => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month) + " " + g.Key.Year,
        g => g.Select(o => o.CustomerId).Distinct().Count()
    ),
ProductList = storeGroup
    .SelectMany(o => o.OrderItems)
    .Where(oi => oi != null && oi.Product != null && oi.Order != null) // Stelle sicher, dass oi, oi.Product und oi.Order nicht null sind
    .GroupBy(oi => oi.Product.SKU)
    .Select(g => new ProductSales
    {
        SKU = g.Key,
        SalesCount = g.Count(),
        Revenue = g.Sum(oi => oi.Order.total) // Hier könnte das Problem liegen, wenn oi.Order null ist
    }).ToList()
                };

                // Berechnen der Metriken pro Jahr und Monat
                foreach (var yearGroup in storeGroup.Where(o => o != null).GroupBy(o => o.OrderDate.Year)) // Ensure o is not null
                {
                    var yearMetrics = new YearMetrics
                    {
                        // Adjust the sorting of months according to monthsOrder
                        Metrics = yearGroup
                            .GroupBy(o => o.OrderDate.Month)
                            .OrderBy(g => monthsOrder.IndexOf(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key)))
                            .ToDictionary(
                                g => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key) + " Order Count",
                                g => g.Count()
                            ),
                        Total = yearGroup.Count()
                    };
                    storeMetrics.MetricsByYear.Add(yearGroup.Key.ToString(), yearMetrics);
                }

                storeMetricsList.Add(storeMetrics);
            }

            return storeMetricsList;
        }
    }
}