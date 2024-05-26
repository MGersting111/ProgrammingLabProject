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
    public class SortedPagedModelsRepository : ISortedPagedModelsRepository
    {
        private readonly ApplicationDBContext _context;

        public SortedPagedModelsRepository(ApplicationDBContext context)      
        {
            _context = context;
        }

        public async Task<List<Order>> GetSortedPagedOrders(int page, int pageSize, string sortColumn, string sortOrder)
        {
            IQueryable<Order> query = _context.Orders;

            // Sortierung anwenden
            if (sortOrder.ToLower() == "asc")
            {
                query = query.OrderBy(o => EF.Property<object>(o, sortColumn));
            }
            else
            {
                query = query.OrderByDescending(o => EF.Property<object>(o, sortColumn));
            }

            // Paginierung anwenden
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<List<StoreWithRevenue>> GetSortedPagedStoresWithRevenue(int page, int pageSize, string sortColumn, string sortOrder)
        {
        
            var stores = await _context.Stores
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var storeIds = stores.Select(s => s.StoreId).ToList();

            var totalRevenues = await _context.Orders
            .Where(o => storeIds.Contains(o.StoreId))
            .GroupBy(o => o.StoreId)
            .Select(g => new { StoreId = g.Key, TotalRevenue = g.Sum(o => o.total) })
            .ToListAsync();

            var customerCounts = await _context.Orders
            .Where(o => storeIds.Contains(o.StoreId))
            .GroupBy(o => o.StoreId)
            .Select(g => new { StoreId = g.Key, CustomerCount = g.Select(o => o.CustomerId).Distinct().Count() })
            .ToListAsync();

            var storesWithRevenue = stores.Select(store => new StoreWithRevenue
        {
            StoreId = store.StoreId,
            Zipcode = store.Zipcode,
            State_abbr = store.State_abbr,
            Latitude = store.Latitude,
            Longitude = store.Longitude,
            City = store.City,
            State = store.State,
            Distance = store.Distance,
            TotalRevenue = totalRevenues.FirstOrDefault(tr => tr.StoreId == store.StoreId)?.TotalRevenue ?? 0,
            CustomerCount = customerCounts.FirstOrDefault(cc => cc.StoreId == store.StoreId)?.CustomerCount ?? 0
        }).ToList();

    storesWithRevenue = sortOrder.ToLower() == "asc"
        ? storesWithRevenue.OrderBy(s => GetPropertyValue(s, sortColumn)).ToList()
        : storesWithRevenue.OrderByDescending(s => GetPropertyValue(s, sortColumn)).ToList();

            return storesWithRevenue;
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
        }





        public async Task<List<Customer>> GetSortedPagedCustomers(int page, int pageSize, string sortColumn, string sortOrder)
        {
            IQueryable<Customer> query = _context.Customers;

            if (sortOrder.ToLower() == "asc")
            {
                query = query.OrderBy(o => EF.Property<object>(o, sortColumn));
            }
            else
            {
                query = query.OrderByDescending(o => EF.Property<object>(o, sortColumn));
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }
        public async Task<List<Product>> GetSortedPagedProducts(int page, int pageSize, string sortColumn, string sortOrder)
        {
            IQueryable<Product> query = _context.Products;

            if (sortOrder.ToLower() == "asc")
            {
                query = query.OrderBy(o => EF.Property<object>(o, sortColumn));
            }
            else
            {
                query = query.OrderByDescending(o => EF.Property<object>(o, sortColumn));
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }
    }
}