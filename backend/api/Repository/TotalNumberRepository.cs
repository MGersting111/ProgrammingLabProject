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



        public List<StoreInfo> GetFilteredStoreInfo(FilterRevenue filter)
        {
            var storeInfos = new List<StoreInfo>();

            var stores = _context.Stores.ToList();
            foreach (var store in stores)
            {

                var orders = _context.Orders.Where(order => order.StoreId == store.StoreId);
                //.Include(order => order.OrderItems) // Stellen Sie sicher, dass die OrderItems der Bestellungen geladen werden
                //.ThenInclude(orderItem => orderItem.Product) // Stellen Sie sicher, dass die Produkte der OrderItems geladen werden
                //.Where(order => order.StoreId == store.StoreId);

                //if (!string.IsNullOrEmpty(filter.Category))
               // {
                    
                 //   orders = orders.Where(order => order.OrderItems.Any(orderItem => orderItem.Product.Category.Equals(filter.Category, StringComparison.OrdinalIgnoreCase)));
               // }

                if (filter.OrderDateFrom.HasValue)
                {
                    orders = orders.Where(order => order.OrderDate >= filter.OrderDateFrom.Value);
                }
                if (filter.OrderDateTo.HasValue)
                {
                    orders = orders.Where(order => order.OrderDate <= filter.OrderDateTo.Value);
                }

                int orderCount = orders.Count();
                double totalRevenue = orders.Sum(order => order.total);
                int customerCount = orders.Select(order => order.CustomerId).Distinct().Count();
                double revenuePerCustomer = customerCount > 0 ? (double)totalRevenue / customerCount : 0;
                storeInfos.Add(new StoreInfo { StoreId = store.StoreId, OrderCount = orderCount, TotalRevenue = totalRevenue, CustomerCount = customerCount, RevenuePerCustomer = revenuePerCustomer });
            }

            if (!string.IsNullOrEmpty(filter.StoreId))
            {
                storeInfos = storeInfos.Where(info => info.StoreId == filter.StoreId).ToList();
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
}