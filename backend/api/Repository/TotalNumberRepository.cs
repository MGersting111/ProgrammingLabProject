using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Data;
using api.Models;
using api.Dto;
using Microsoft.EntityFrameworkCore;


namespace api.Repository
{
    public class TotalNumberRepository : ITotalNumberRepository
    {
        private readonly ApplicationDBContext _context;




        public TotalNumberRepository(ApplicationDBContext context)
        {
            _context = context;


        }

        public double GetTotalStoreRevenue(string storeId)
        {
            var orders = _context.Orders.Where(order => order.StoreId == storeId);
            return orders.Sum(order => order.total);
        }



        public async Task<List<StoreInfo>> GetFilteredStoreInfoAsync(FilterRevenue filter)
        {
            var storeInfos = new List<StoreInfo>();

            var stores = await _context.Stores.AsNoTracking().ToListAsync();
            foreach (var store in stores)
            {
                var orders = _context.Orders
                    .AsNoTracking()
                    .Include(order => order.OrderItems)
                    .ThenInclude(orderItem => orderItem.Product)
                    .Where(order => order.StoreId == store.StoreId);

                if (!string.IsNullOrEmpty(filter.Category))
                {
                    orders = orders.Where(order => order.OrderItems.Any(orderItem => orderItem.Product.Category == filter.Category));
                }

                if (filter.OrderDateFrom.HasValue)
                {
                    orders = orders.Where(order => order.OrderDate >= filter.OrderDateFrom.Value);
                }
                if (filter.OrderDateTo.HasValue)
                {
                    orders = orders.Where(order => order.OrderDate <= filter.OrderDateTo.Value);
                }

                int orderCount = await orders.CountAsync();
                double totalRevenue = await orders.SumAsync(order => order.total);
                int customerCount = await orders.Select(order => order.CustomerId).Distinct().CountAsync();
                double revenuePerCustomer = customerCount > 0 ? totalRevenue / customerCount : 0;
                storeInfos.Add(new StoreInfo { StoreId = store.StoreId, OrderCount = orderCount, TotalRevenue = totalRevenue, CustomerCount = customerCount, RevenuePerCustomer = revenuePerCustomer });
            }


            bool isFilterApplied = !string.IsNullOrEmpty(filter.StoreId) || filter.StoreRevenues.HasValue || filter.StoreOrderCounts.HasValue || filter.StoreCustomerCounts.HasValue || filter.RevenuePerCustomer.HasValue || !string.IsNullOrEmpty(filter.Category) || filter.OrderDateFrom.HasValue || filter.OrderDateTo.HasValue;

            if (!isFilterApplied)
            {
                // Wenn kein Filter angewendet wurde, berechnen Sie den Gesamtumsatz aller Geschäfte und geben Sie ihn zurück
                double totalRevenueAllStores = storeInfos.Sum(info => info.TotalRevenue);
                int totalOrderCountAllStores = storeInfos.Sum(info => info.OrderCount);
                int totalCustomerCountAllStores = storeInfos.Sum(info => info.CustomerCount);
                double totalRevenuePerCustomerAllStores = totalCustomerCountAllStores > 0 ? totalRevenueAllStores / totalCustomerCountAllStores : 0;
                return new List<StoreInfo> { new StoreInfo { StoreId = "All Stores", TotalRevenue = totalRevenueAllStores, OrderCount = totalOrderCountAllStores, CustomerCount = totalCustomerCountAllStores, RevenuePerCustomer = totalRevenuePerCustomerAllStores } };
            }

            else
            {
                // Wenn ein Filter angewendet wurde, wenden Sie die Filter an und geben Sie die gefilterten Daten zurück

                if (!string.IsNullOrEmpty(filter.StoreId))
                {
                    var storeIdList = filter.StoreId.Split(',').ToList(); // Teilen Sie die StoreIds in eine Liste auf
                    storeInfos = storeInfos.Where(info => storeIdList.Contains(info.StoreId)).ToList();
                }



                if (filter.StoreRevenues.HasValue)
                {
                    storeInfos = storeInfos.Where(info => info.TotalRevenue >= filter.StoreRevenues.Value).ToList();
                }

                if (filter.StoreOrderCounts.HasValue)
                {
                    storeInfos = storeInfos.Where(info => info.OrderCount >= filter.StoreOrderCounts.Value).ToList();
                }
                if (filter.StoreCustomerCounts.HasValue)
                {
                    storeInfos = storeInfos.Where(info => info.CustomerCount >= filter.StoreCustomerCounts.Value).ToList();
                }
                if (filter.RevenuePerCustomer.HasValue)
                {
                    storeInfos = storeInfos.Where(info => info.RevenuePerCustomer >= filter.RevenuePerCustomer.Value).ToList();
                }



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
}

