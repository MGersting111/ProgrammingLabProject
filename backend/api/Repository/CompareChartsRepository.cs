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
    public class CompareChartsRepository : ICompareChartsRepository
    {
        private readonly ApplicationDBContext _context;




        public CompareChartsRepository(ApplicationDBContext context)
        {
            _context = context;


        }


       
public async Task<List<ChartsInfo>> GetDiagramDataAsync(FilterCharts filter)
{
    var chartsInfos = new List<ChartsInfo>();

    // Filtern Sie die Stores nach dem angegebenen Zeitraum und begrenzen Sie die Anzahl der Stores, wenn ein Limit angegeben wurde
    var stores = await _context.Stores
        .Where(store => store.Orders.Any(order => order.OrderDate >= filter.StartTime && order.OrderDate <= filter.EndTime))
        .Take(filter.Limit ?? int.MaxValue)
        .ToListAsync();

    foreach (var store in stores)
    {
        // Berechnen Sie die Metriken für jeden Monat im angegebenen Zeitraum
        for (var date = filter.StartTime; date <= filter.EndTime; date = date.AddMonths(1))
        {
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month);

            var orders = _context.Orders
                .Where(order => order.StoreId == store.StoreId && order.OrderDate.Month == date.Month && order.OrderDate.Year == date.Year);

            var chartsInfo = new ChartsInfo 
            { 
                StoreId = store.StoreId, 
                Month = monthName,  // Hier setzen Sie den Monatsnamen
                Year = date.Year,
               
            };

            if (filter.Metrics.Contains("totalRevenue"))
            {
                chartsInfo.TotalRevenue = await orders.SumAsync(order => order.total);
            }
            if (filter.Metrics.Contains("customer"))
            {
                chartsInfo.CustomerCount = await orders.Select(order => order.CustomerId).Distinct().CountAsync();
            }
            if (filter.Metrics.Contains("revenuePerCustomer"))
            {
                chartsInfo.RevenuePerCustomer = chartsInfo.CustomerCount > 0 ? chartsInfo.TotalRevenue / chartsInfo.CustomerCount : 0;
            }
            if (filter.Metrics.Contains("sales"))
            {
                chartsInfo.OrderCount = await orders.CountAsync();
            }

            chartsInfos.Add(chartsInfo);
        }
    }

    return chartsInfos;
}






    }
}

