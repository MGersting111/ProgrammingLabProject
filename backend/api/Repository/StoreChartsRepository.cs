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
    public class StoreChartsRepository : IStoreChartsRepository
    {
        private readonly ApplicationDBContext _context;

        public StoreChartsRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<StoreChartsInfo>> GetStoreChartsInfoAsync(DateTime fromDate, DateTime toDate)
    {
        var orders = await _context.Orders
                .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
                .Include(o => o.OrderItems)
                .ToListAsync();

        var storeChartsInfo = await _context.Orders
            .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
            .GroupBy(o => o.StoreId)
            .Select(g => new 
            {
                StoreId = g.Key,
                TotalRevenue = g.Sum(o => o.total),
                TotalSales = g.Count(),
                AvgRevenue = g.Average(o => o.total),
                AvgSales = g.Count() > 0 ? g.Average(o => o.NItems) : 0,
                TotalCustomers = g.Select(o => o.CustomerId).Distinct().Count(),
                ProductSales = g.SelectMany(o => o.OrderItems)
                                .GroupBy(oi => oi.Product.SKU)
                                .Select(p => new ProductSalesInfo
                                {
                                    ProductSKU = p.Key,
                                    TotalSales = p.Sum(oi => oi.Order.NItems),
                                    TotalRevenue = p.Sum(oi => oi.Order.total)
                                }).ToList(),
                MonthlySales = g.GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                                .Select(mg => new 
                                {
                                    Year = mg.Key.Year,
                                    Month = mg.Key.Month,
                                    Sales = mg.Count()
                                })
                                .OrderBy(ms => ms.Year)
                                .ThenBy(ms => ms.Month)
                                .ToList(),

                MonthlyRevenue = g.GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                                      .Select(mg => new 
                                      {
                                          Year = mg.Key.Year,
                                          Month = mg.Key.Month,
                                          Revenue = mg.Sum(o => o.total)
                                      })
                                      .OrderBy(mr => mr.Year)
                                      .ThenBy(mr => mr.Month)
                                      .ToList(),

                MonthlyAvgRevenuePerSale = g.GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                                                .Select(mg => new
                                                {
                                                    Year = mg.Key.Year,
                                                    Month = mg.Key.Month,
                                                    AvgRevenuePerSale = mg.Average(o => o.total)
                                                })
                                                .OrderBy(mar => mar.Year)
                                                .ThenBy(mar => mar.Month)
                                                .ToList(),

                MonthlyCustomers = g.GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                                        .Select(mg => new
                                        {
                                            Year = mg.Key.Year,
                                            Month = mg.Key.Month,
                                            Customers = mg.Select(o => o.CustomerId).Distinct().Count()
                                        })
                                        .OrderBy(mc => mc.Year)
                                        .ThenBy(mc => mc.Month)
                                        .ToList(),
                MonthlyProductSales = g.SelectMany(o => o.OrderItems)
                                           .GroupBy(oi => new { oi.Product.SKU, oi.Order.OrderDate.Year, oi.Order.OrderDate.Month })
                                           .Select(mpg => new 
                                           {
                                               SKU = mpg.Key.SKU,
                                               Year = mpg.Key.Year,
                                               Month = mpg.Key.Month,
                                               Sales = mpg.Sum(oi => oi.Order.NItems)
                                           })
                                           .ToList()

                
                
            })
            .ToListAsync();

        var result = storeChartsInfo.Select(store => new StoreChartsInfo
        {
            StoreId = store.StoreId,
            FromDate = fromDate,
            ToDate = toDate,
            TotalRevenue = store.TotalRevenue,
            TotalSales = store.TotalSales,
            AvgRevenue = store.AvgRevenue,
            AvgSales = store.AvgSales,
            TotalCustomers = store.TotalCustomers,
            ProductSales = store.ProductSales,
            MonthlySales = store.MonthlySales
                .ToDictionary(ms => $"Sales {new DateTime(ms.Year, ms.Month, 1).ToString("MMMM")}", ms => ms.Sales),
             MonthlyRevenue = store.MonthlyRevenue
                    .ToDictionary(mr => $"Revenue {new DateTime(mr.Year, mr.Month, 1).ToString("MMMM")}", mr => mr.Revenue),
            MonthlyAvgRevenuePerSale = store.MonthlyAvgRevenuePerSale
                    .ToDictionary(mar => $"AvgRevenuePerSale {new DateTime(mar.Year, mar.Month, 1).ToString("MMMM")}", mar => mar.AvgRevenuePerSale),
            MonthlyCustomers = store.MonthlyCustomers
                    .ToDictionary(mc => $"Customers {new DateTime(mc.Year, mc.Month, 1).ToString("MMMM")}", mc => mc.Customers),
            MonthlyProductSales = store.MonthlyProductSales
                    .GroupBy(mps => mps.SKU)
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToDictionary(
                            ms => $"Sales {new DateTime(ms.Year, ms.Month, 1).ToString("MMMM")}", 
                            ms => ms.Sales
                        )
                    )
            
        }).ToList();

        return result;
    }
}
   
    }

