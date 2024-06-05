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

        for (var date = filter.StartTime; date <= filter.EndTime; date = date.AddMonths(1))
        {
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month);

            var orders = _context.Orders
                .Where(order => order.StoreId == store.StoreId && order.OrderDate.Month == date.Month && order.OrderDate.Year == date.Year);

            if (filter.Metrics.Contains("totalRevenue"))
            {
                var totalRevenue = await orders.SumAsync(order => order.total);
                chartsInfo.Metrics.Add(monthName + " Total Revenue", totalRevenue);
            }
            if (filter.Metrics.Contains("customer"))
            {
                var customerCount = await orders.Select(order => order.CustomerId).Distinct().CountAsync();
                chartsInfo.Metrics.Add(monthName + " Customer Count", customerCount);
            }
            if (filter.Metrics.Contains("revenuePerCustomer"))
            {
                var customerCount = await orders.Select(order => order.CustomerId).Distinct().CountAsync();
                var revenuePerCustomer = customerCount > 0 ? await orders.SumAsync(order => order.total) / customerCount : 0;
                chartsInfo.Metrics.Add(monthName + " Revenue Per Customer", revenuePerCustomer);
            }
            if (filter.Metrics.Contains("sales"))
            {
                var orderCount = await orders.CountAsync();
                chartsInfo.Metrics.Add(monthName + " Order Count", orderCount);
            }
        }

        chartsInfos.Add(chartsInfo);
    }
            break;
   case ComparisonType.Product:
    // Neuer Code zur Vergleichung von Produkten
    var productGroups = await _context.Products
        .Where(product => product.OrderItems.Any(orderItem => orderItem.Order.OrderDate >= filter.StartTime && orderItem.Order.OrderDate <= filter.EndTime))
        .GroupBy(product => product.Name)
        .ToListAsync();

    foreach (var productGroup in productGroups)
    {
        var chartsInfo = new ChartsInfo 
        { 
            StoreId = productGroup.Key,  // Verwenden Sie den Produktnamen als Schlüssel
            Metrics = new Dictionary<string, double>()
        };

        for (var date = filter.StartTime; date <= filter.EndTime; date = date.AddMonths(1))
        {
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month);

            var totalOrderCount = 0;
            foreach (var product in productGroup)
            {
                var orderItems = _context.OrderItems
                    .Where(orderItem => orderItem.SKU == product.SKU && orderItem.Order.OrderDate.Month == date.Month && orderItem.Order.OrderDate.Year == date.Year);

                if (filter.Metrics.Contains("sales"))
                {
                    var orderCount = await orderItems.CountAsync();
                    totalOrderCount += orderCount;
                }
            }
            chartsInfo.Metrics.Add(monthName, totalOrderCount);  // Verwenden Sie nur den Monatsnamen als Schlüssel
        }

        // Berechnen Sie das Gesamttotal für das Produkt
        chartsInfo.Total = (int)chartsInfo.Metrics.Values.Sum();

        chartsInfos.Add(chartsInfo);
    }
    break;


  case ComparisonType.Category:
    // Neuer Code zur Vergleichung von Kategorien
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

            var orderItems = _context.OrderItems
                .Where(orderItem => orderItem.Product.Category == category && orderItem.Order.OrderDate.Month == date.Month && orderItem.Order.OrderDate.Year == date.Year);

            if (filter.Metrics.Contains("sales"))
            {
                var orderCount = await orderItems.CountAsync();
                chartsInfo.Metrics.Add(monthName, orderCount);  // Verwenden Sie nur den Monatsnamen als Schlüssel
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

