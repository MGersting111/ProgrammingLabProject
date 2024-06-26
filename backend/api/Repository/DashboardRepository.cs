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
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDBContext _context;

        public DashboardRepository(ApplicationDBContext context)
        {
            _context = context;
        }

  public async Task<DashboardDto> GetDashboardDataAsync(DateTime startDate, DateTime endDate)
{
    _context.Database.SetCommandTimeout(300);

    var dashboardDto = new DashboardDto();

    // 1. Revenue by Month:
    dashboardDto.RevenueByMonth = await _context.Orders
        .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
        .GroupBy(o => o.OrderDate.Month)
        .Select(g => new { Month = g.Key, Revenue = g.Sum(o => o.total) }) // Cast total to decimal here
        .ToDictionaryAsync(x => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(x.Month), x => x.Revenue);

    // 2. Average Revenue Per Store:
    var storeRevenues = await _context.Orders
        .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
        .GroupBy(o => o.StoreId)
        .Select(g => new
        {
            StoreId = g.Key,
            TotalRevenue = g.Sum(o => o.total)
        })
        .ToListAsync(); // Get the data in-memory

    dashboardDto.AvgRevenuePerStore = storeRevenues.Any() ? 
        storeRevenues.Average(s => s.TotalRevenue) : 0;


            // 3. Best Selling Product (by SKU):
            dashboardDto.BestSellingProduct = await _context.OrderItems
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .GroupBy(oi => oi.Product.SKU)
                .OrderByDescending(g => g.Count())
                .Select(g => g.FirstOrDefault().Product.SKU)
                .FirstOrDefaultAsync();

            // 4. Most Purchased Size and Category:
            dashboardDto.MostPurchasedSize = await _context.OrderItems
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .GroupBy(oi => oi.Product.Size)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            dashboardDto.MostPurchasedCategory = await _context.OrderItems
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .GroupBy(oi => oi.Product.Category)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();


            // 5. Average Customers and Average Sales:
            var orderStats = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => 1)
                .Select(g => new
                {
                    TotalCustomers = g.Select(o => o.CustomerId).Distinct().Count(),
                    TotalOrders = g.Count()
                })
                .FirstOrDefaultAsync();

            dashboardDto.AverageCustomers = orderStats != null ? orderStats.TotalCustomers / orderStats.TotalOrders : 0;
            dashboardDto.AverageSales = orderStats != null ? orderStats.TotalOrders / (endDate - startDate).Days : 0;

            // 6. Top 3 Stores, Products, and Customers:
   dashboardDto.Top3Stores = await _context.Orders
        .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
        .GroupBy(o => o.StoreId)
        .OrderByDescending(g => g.Sum(o => o.total))
        .Take(3)
        .Select(g => new Dashboard { StoreId = g.Key, TotalRevenue = g.Sum(o => o.total) })
        .ToListAsync();

    dashboardDto.Top3Products = await _context.OrderItems
        .Include(oi => oi.Product)
        .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
        .GroupBy(oi => oi.Product.SKU)
        .OrderByDescending(g => g.Count())
        .Take(3)
        .Select(g => new Dashboard { SKU = g.Key, TotalOrders = g.Count() })
        .ToListAsync();

    dashboardDto.Top3Customers = await _context.Orders
        .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
        .GroupBy(o => o.CustomerId)
        .OrderByDescending(g => g.Sum(o => o.total))
        .Take(3)
        .Select(g => new Dashboard { CustomerId = int.Parse(g.Key), TotalSpent = g.Sum(o => o.total) })
        .ToListAsync();

    return dashboardDto;
}
        }
    }
