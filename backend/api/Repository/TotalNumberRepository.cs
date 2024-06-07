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
    public class TotalNumberRepository : ITotalNumberRepository
    {
        private readonly ApplicationDBContext _context;




        public TotalNumberRepository(ApplicationDBContext context)
        {
            _context = context;


        }

      //  public double GetTotalStoreRevenue(string storeId)
       // {
        //    var orders = _context.Orders.Where(order => order.StoreId == storeId);
         //   return orders.Sum(order => order.total);
      //  }



        public async Task<List<StoreInfo>> GetFilteredStoreInfoAsync(FilterRevenue filter)
{
    var query = _context.Orders.AsNoTracking();

    if (!string.IsNullOrEmpty(filter.StoreId))
    {
        var storeIdList = filter.StoreId.Split(',').ToList(); // Teilen Sie die StoreIds in eine Liste auf
        query = query.Where(order => storeIdList.Contains(order.StoreId));
    }

    if (!string.IsNullOrEmpty(filter.Category))
    {
        query = query.Where(order => order.OrderItems.Any(orderItem => orderItem.Product.Category == filter.Category));
    }

    if (filter.OrderDateFrom.HasValue)
    {
        query = query.Where(order => order.OrderDate >= filter.OrderDateFrom.Value);
    }
    if (filter.OrderDateTo.HasValue)
    {
        query = query.Where(order => order.OrderDate <= filter.OrderDateTo.Value);
    }

    query = query.Include(order => order.OrderItems).ThenInclude(orderItem => orderItem.Product);

    var storeInfos = await query
        .GroupBy(order => order.StoreId)
        .Select(group => new StoreInfo
        {
            StoreId = group.Key,
            OrderCount = group.Count(),
            TotalRevenue = group.Sum(order => order.total),
            CustomerCount = group.Select(order => order.CustomerId).Distinct().Count(),
            RevenuePerCustomer = group.Sum(order => order.total) / (double)group.Select(order => order.CustomerId).Distinct().Count()
        })
        .ToListAsync();

    return storeInfos;
}

    }


    //  public async Task<List<Order>> GetSortedPagedOrders(int page, int pageSize, string sortColumn, string sortOrder)
    // {
    //     IQueryable<Order> query = _context.Orders;

    //     // Sortierung anwenden
    //     if (sortOrder.ToLower() == "asc")
    //     {
    //         query = query.OrderBy(o => EF.Property<object>(o, sortColumn));
    //     }
    //     else
    //     {
    //         query = query.OrderByDescending(o => EF.Property<object>(o, sortColumn));
    //     }

    //     // Paginierung anwenden
    //     query = query.Skip((page - 1) * pageSize).Take(pageSize);

    //     return await query.ToListAsync();
    // }



}


