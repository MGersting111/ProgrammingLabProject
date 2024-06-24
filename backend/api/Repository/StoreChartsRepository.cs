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

        public async Task<List<StoreChartsInfo>> GetStoreChartsInfoAsync(DateTime StartDate, DateTime EndDate)
        {
            
            var orders = await _context.Orders
                .Where(o => o.OrderDate >= StartDate && o.OrderDate <= EndDate)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();

            var storeChartsInfo = orders
                .GroupBy(o => o.StoreId)
                 .AsParallel()
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
                                        ProductName = p.First().Product.Name,
                                        TotalSales = p.Select(oi => oi.OrderId).Distinct().Count(),
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
                                           .GroupBy(oi => new { oi.Product.SKU, oi.Product.Name, oi.Order.OrderDate.Year, oi.Order.OrderDate.Month })
                                           .Select(mpg => new
                                           {
                                               SKU = mpg.Key.SKU,
                                               ProductName = mpg.Key.Name,
                                               Year = mpg.Key.Year,
                                               Month = mpg.Key.Month,
                                               Sales = mpg.Sum(oi => oi.Order.NItems)
                                           })
                                           .ToList()
                })
                .ToList();

            var result = storeChartsInfo.Select(store => new StoreChartsInfo
            {
                StoreId = store.StoreId,
                StartDate = StartDate,
                EndDate = EndDate,
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
                    .GroupBy(mps => new { mps.SKU, mps.ProductName })
                    .Select(g => new MonthlyProductSalesInfo
                    {
                        ProductSKU = g.Key.SKU,
                        ProductName = g.Key.ProductName,
                        Sales = g.ToDictionary(
                            ms => $"Sales {new DateTime(ms.Year, ms.Month, 1).ToString("MMMM")}",
                            ms => ms.Sales
                        )
                    })
                    .ToList()
            }).ToList();

            return result;
}
    
}
   
    }

