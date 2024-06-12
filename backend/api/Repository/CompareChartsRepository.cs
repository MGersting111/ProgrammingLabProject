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
    public enum ComparisonType
    {
        Store,
        Product,
        Category
    }

    public class CompareChartsRepository : ICompareChartsRepository
    {
        private readonly ApplicationDBContext _context;

        public CompareChartsRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<ChartsInfo>> GetDiagramDataAsync(FilterCharts filter, ComparisonType comparisonType)
        {
            var chartsInfos = new List<ChartsInfo>();

            switch (comparisonType)
            {
                case ComparisonType.Store:
                    // Erhöhung des Command Timeout
                    _context.Database.SetCommandTimeout(300);  // Setzt den Timeout auf 300 Sekunden

                    // Berechnung der Metriken direkt in der Datenbank
                    var stores = await _context.Stores.AsNoTracking()
                        .Where(store => store.Orders.Any(order => order.OrderDate >= filter.StartTime && order.OrderDate <= filter.EndTime))
                        .Take(filter.Limit ?? int.MaxValue)
                        .ToListAsync();

                    foreach (var store in stores)
                    {
                        var chartsInfo = new ChartsInfo 
                        { 
                            StoreId = store.StoreId, 
                            MetricsByYear = new Dictionary<int, YearlyMetrics>()
                        };

                        var orderMetrics = await _context.Orders
                            .AsNoTracking()
                            .Where(order => order.StoreId == store.StoreId && order.OrderDate >= filter.StartTime && order.OrderDate <= filter.EndTime)
                            .GroupBy(order => new { order.OrderDate.Year, order.OrderDate.Month })
                            .Select(group => new 
                            {
                                Year = group.Key.Year,
                                Month = group.Key.Month,
                                TotalRevenue = group.Sum(order => order.total),
                                CustomerCount = group.Select(order => order.CustomerId).Distinct().Count(),
                                OrderCount = group.Count()
                            })
                            .ToListAsync();

                        foreach (var orderMetric in orderMetrics)
                        {
                            var monthNameMetric = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(orderMetric.Month);
                            if (!chartsInfo.MetricsByYear.ContainsKey(orderMetric.Year))
                            {
                                chartsInfo.MetricsByYear[orderMetric.Year] = new YearlyMetrics { Metrics = new Dictionary<string, double>() };
                            }
                            if (filter.Metrics.Contains("totalRevenue"))
                            {
                                chartsInfo.MetricsByYear[orderMetric.Year].Metrics.Add(monthNameMetric + " Total Revenue", orderMetric.TotalRevenue);
                            }
                            if (filter.Metrics.Contains("customer"))
                            {
                                chartsInfo.MetricsByYear[orderMetric.Year].Metrics.Add(monthNameMetric + " Customer Count", orderMetric.CustomerCount);
                            }
                            if (filter.Metrics.Contains("revenuePerCustomer"))
                            {
                                var revenuePerCustomer = orderMetric.CustomerCount > 0 ? orderMetric.TotalRevenue / orderMetric.CustomerCount : 0;
                                chartsInfo.MetricsByYear[orderMetric.Year].Metrics.Add(monthNameMetric + " Revenue Per Customer", revenuePerCustomer);
                            }
                            if (filter.Metrics.Contains("sales"))
                            {
                                chartsInfo.MetricsByYear[orderMetric.Year].Metrics.Add(monthNameMetric + " Order Count", orderMetric.OrderCount);
                            }
                        }

                        // Berechnen Sie das Gesamttotal für das Jahr und den Store
                        foreach (var year in chartsInfo.MetricsByYear.Keys)
                        {
                            chartsInfo.MetricsByYear[year].Total = (int)chartsInfo.MetricsByYear[year].Metrics.Values.Sum();
                        }
                        chartsInfo.Total = (int)chartsInfo.MetricsByYear.Values.Select(y => y.Total).Sum();

                        chartsInfos.Add(chartsInfo);
                    }
                    break;

                case ComparisonType.Product:
                    // Erhöhung des Command Timeout
                    _context.Database.SetCommandTimeout(300);  // Setzt den Timeout auf 300 Sekunden

                    // Berechnung der Metriken direkt in der Datenbank
                    var productMetrics = await _context.OrderItems
                        .AsNoTracking()
                        .Include(orderItem => orderItem.Product)
                        .Include(orderItem => orderItem.Order)
                        .Where(orderItem => orderItem.Order.OrderDate >= filter.StartTime && orderItem.Order.OrderDate <= filter.EndTime)
                        .GroupBy(orderItem => new { orderItem.Product.Name, Year = orderItem.Order.OrderDate.Year, Month = orderItem.Order.OrderDate.Month })
                        .Select(group => new 
                        {
                            ProductName = group.Key.Name,
                            Year = group.Key.Year,
                            Month = group.Key.Month,
                            TotalOrders = group.Count()
                        })
                        .ToListAsync();

                    // Dictionary zum Speichern der konsolidierten Metriken
                    var consolidatedMetrics = new Dictionary<string, Dictionary<int, YearlyMetrics>>();

                    foreach (var productMetric in productMetrics)
                    {
                        if (!consolidatedMetrics.ContainsKey(productMetric.ProductName))
                        {
                            consolidatedMetrics[productMetric.ProductName] = new Dictionary<int, YearlyMetrics>();
                        }

                        var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(productMetric.Month);
                        if (!consolidatedMetrics[productMetric.ProductName].ContainsKey(productMetric.Year))
                        {
                            consolidatedMetrics[productMetric.ProductName][productMetric.Year] = new YearlyMetrics { Metrics = new Dictionary<string, double>() };
                        }
                        consolidatedMetrics[productMetric.ProductName][productMetric.Year].Metrics[monthName] = productMetric.TotalOrders;
                    }

                    // Erstellen der ChartsInfo-Objekte
                    foreach (var item in consolidatedMetrics)
                    {
                        var metricsByYear = new Dictionary<int, YearlyMetrics>();
                        for (int year = filter.StartTime.Year; year <= filter.EndTime.Year; year++)
                        {
                            var metrics = new Dictionary<string, double>();
                            for (int month = 1; month <= 12; month++)
                            {
                                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
                                if (item.Value.ContainsKey(year) && item.Value[year].Metrics.ContainsKey(monthName))
                                {
                                    metrics[monthName] = item.Value[year].Metrics[monthName];
                                }
                                else
                                {
                                    metrics[monthName] = 0;
                                }
                            }

                            var sortedMetrics = metrics.OrderBy(m => DateTime.ParseExact(m.Key, "MMM", CultureInfo.CurrentCulture)).ToDictionary(m => m.Key, m => m.Value);
                            metricsByYear[year] = new YearlyMetrics { Metrics = sortedMetrics, Total = (int)sortedMetrics.Values.Sum() };
                        }

                        var chartsInfo = new ChartsInfo
                        {
                            StoreId = item.Key,
                            MetricsByYear = metricsByYear,
                            Total = (int)metricsByYear.Values.Select(y => y.Total).Sum()
                        };

                        chartsInfos.Add(chartsInfo);
                    }
                    break;

                case ComparisonType.Category:
                    // Erhöhung des Command Timeout
                    _context.Database.SetCommandTimeout(300);  // Setzt den Timeout auf 300 Sekunden

                    // Berechnung der Metriken direkt in der Datenbank
                    var categories = new List<string> { "Classic", "Vegetarian", "Specialty" };
                    foreach (var category in categories)
                    {
                        var chartsInfo = new ChartsInfo 
                        { 
                            StoreId = category, 
                            MetricsByYear = new Dictionary<int, YearlyMetrics>()
                        };

                        for (var date = filter.StartTime; date <= filter.EndTime; date = date.AddMonths(1))
                        {
                            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month);
                            if (!chartsInfo.MetricsByYear.ContainsKey(date.Year))
                            {
                                chartsInfo.MetricsByYear[date.Year] = new YearlyMetrics { Metrics = new Dictionary<string, double>() };
                            }

                            var orderItemsMetrics = await _context.OrderItems
                                .AsNoTracking()
                                .Include(orderItem => orderItem.Product)
                                .Include(orderItem => orderItem.Order)
                                .Where(orderItem => orderItem.Product.Category == category && orderItem.Order.OrderDate.Month == date.Month && orderItem.Order.OrderDate.Year == date.Year)
                                .GroupBy(orderItem => new { orderItem.Order.OrderDate.Year, orderItem.Order.OrderDate.Month })
                                .Select(group => new 
                                {
                                    Year = group.Key.Year,
                                    Month = group.Key.Month,
                                    TotalOrders = group.Count()
                                })
                                .ToListAsync();

                            foreach (var orderItemsMetric in orderItemsMetrics)
                            {
                                var monthNameMetric = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(orderItemsMetric.Month);
                                chartsInfo.MetricsByYear[orderItemsMetric.Year].Metrics[monthNameMetric] = orderItemsMetric.TotalOrders;
                            }
                        }

                        // Berechnen Sie das Gesamttotal für die Kategorie
                        foreach (var year in chartsInfo.MetricsByYear.Keys)
                        {
                            chartsInfo.MetricsByYear[year].Total = (int)chartsInfo.MetricsByYear[year].Metrics.Values.Sum();
                        }
                        chartsInfo.Total = (int)chartsInfo.MetricsByYear.Values.Select(y => y.Total).Sum();

                        chartsInfos.Add(chartsInfo);
                    }
                    break;
            }

            return chartsInfos;
        }
    }
}
