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
            Metrics = new Dictionary<string, double>()
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
            if (filter.Metrics.Contains("totalRevenue"))
            {
                chartsInfo.Metrics.Add(monthNameMetric + " Total Revenue", orderMetric.TotalRevenue);
            }
            if (filter.Metrics.Contains("customer"))
            {
                chartsInfo.Metrics.Add(monthNameMetric + " Customer Count", orderMetric.CustomerCount);
            }
            if (filter.Metrics.Contains("revenuePerCustomer"))
            {
                var revenuePerCustomer = orderMetric.CustomerCount > 0 ? orderMetric.TotalRevenue / orderMetric.CustomerCount : 0;
                chartsInfo.Metrics.Add(monthNameMetric + " Revenue Per Customer", revenuePerCustomer);
            }
            if (filter.Metrics.Contains("sales"))
            {
                chartsInfo.Metrics.Add(monthNameMetric + " Order Count", orderMetric.OrderCount);
            }
        }

        // Berechnen Sie das Gesamttotal für den Store
        chartsInfo.Total = (int)chartsInfo.Metrics.Values.Sum();

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
    var consolidatedMetrics = new Dictionary<string, Dictionary<string, double>>();

    foreach (var productMetric in productMetrics)
    {
        if (!consolidatedMetrics.ContainsKey(productMetric.ProductName))
        {
            consolidatedMetrics[productMetric.ProductName] = new Dictionary<string, double>();
        }

        var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(productMetric.Month);
        consolidatedMetrics[productMetric.ProductName][monthName] = productMetric.TotalOrders;
    }

    // Erstellen der ChartsInfo-Objekte
    foreach (var item in consolidatedMetrics)
    {
        var metrics = new Dictionary<string, double>();
        for (int month = 1; month <= 12; month++)
        {
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
            if (item.Value.ContainsKey(monthName))
            {
                metrics[monthName] = item.Value[monthName];
            }
            else
            {
                metrics[monthName] = 0;
            }
        }

        var sortedMetrics = metrics.OrderBy(m => DateTime.ParseExact(m.Key, "MMM", CultureInfo.CurrentCulture)).ToDictionary(m => m.Key, m => m.Value);

        var chartsInfo = new ChartsInfo
        {
            StoreId = item.Key,
            Metrics = sortedMetrics,
            Total = (int)sortedMetrics.Values.Sum()
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
            Metrics = new Dictionary<string, double>()
        };

        for (var date = filter.StartTime; date <= filter.EndTime; date = date.AddMonths(1))
        {
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month);

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
                chartsInfo.Metrics[monthNameMetric] = orderItemsMetric.TotalOrders;
            }
        }

        // Berechnen Sie das Gesamttotal für die Kategorie
        chartsInfo.Total = (int)chartsInfo.Metrics.Values.Sum();

        chartsInfos.Add(chartsInfo);
    }
    break;


    }

    return chartsInfos;
}




    }
}

