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
            var monthsOrder = new List<string> { "Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez" };

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Store) // Include store to get State and City
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync();

            var groupedByStore = orders.GroupBy(o => o.StoreId);

            foreach (var storeGroup in groupedByStore)
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
                        }).ToList(),
                    MonthlySales = storeGroup
                        .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                        .Select(mg => new
                        {
                            Year = mg.Key.Year,
                            Month = mg.Key.Month,
                            Sales = mg.Count()
                        })
                        .OrderBy(ms => ms.Year)
                        .ThenBy(ms => ms.Month)
                        .ToDictionary(ms => $"Sales {new DateTime(ms.Year, ms.Month, 1).ToString("MMMM")}", ms => ms.Sales),
                    MonthlyRevenue = storeGroup
                        .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                        .Select(mg => new
                        {
                            Year = mg.Key.Year,
                            Month = mg.Key.Month,
                            Revenue = mg.Sum(o => o.total)
                        })
                        .OrderBy(mr => mr.Year)
                        .ThenBy(mr => mr.Month)
                        .ToDictionary(mr => $"Revenue {new DateTime(mr.Year, mr.Month, 1).ToString("MMMM")}", mr => mr.Revenue),
                    MonthlyAvgRevenuePerSale = storeGroup
                        .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                        .Select(mg => new
                        {
                            Year = mg.Key.Year,
                            Month = mg.Key.Month,
                            AvgRevenuePerSale = mg.Average(o => o.total)
                        })
                        .OrderBy(mar => mar.Year)
                        .ThenBy(mar => mar.Month)
                        .ToDictionary(mar => $"AvgRevenuePerSale {new DateTime(mar.Year, mar.Month, 1).ToString("MMMM")}", mar => mar.AvgRevenuePerSale),
                    MonthlyCustomers = storeGroup
                        .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                        .Select(mg => new
                        {
                            Year = mg.Key.Year,
                            Month = mg.Key.Month,
                            Customers = mg.Select(o => o.CustomerId).Distinct().Count()
                        })
                        .OrderBy(mc => mc.Year)
                        .ThenBy(mc => mc.Month)
                        .ToDictionary(mc => $"Customers {new DateTime(mc.Year, mc.Month, 1).ToString("MMMM")}", mc => mc.Customers),
                    MonthlyProductSales = storeGroup
                        .SelectMany(o => o.OrderItems)
                        .GroupBy(oi => new { oi.Product.SKU, oi.Product.Name })
                        .Select(g => new MonthlyProductSales
                        {
                            ProductSKU = g.Key.SKU,
                            ProductName = g.Key.Name,
                            Sales = g
                                .GroupBy(oi => new { oi.Order.OrderDate.Year, oi.Order.OrderDate.Month })
                                .ToDictionary(
                                    ms => $"Sales {new DateTime(ms.Key.Year, ms.Key.Month, 1).ToString("MMMM")}",
                                    ms => ms.Sum(oi => oi.Order.NItems)
                                )
                        }).ToList()
                };

                storeMetricsList.Add(storeMetrics);
            }

            return storeMetricsList;
        }
    }
}