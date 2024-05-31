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

    var stores = await _context.Stores
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

    return chartsInfos;
}






    }
}

