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
using Newtonsoft.Json;

namespace api.Repository
{
    public class TotalNumberRepository : ITotalNumberRepository
    {
        private readonly ApplicationDBContext _context;




        public TotalNumberRepository(ApplicationDBContext context)
        {
            _context = context;


        }

        public async Task<List<StoreInfo>> GetFilteredStoreInfoAsync(FilterRevenue filter)
        {

            var startTimeString = filter.OrderDateFrom?.ToString("yyyyMMddHHmmss");
            var endTimeString = filter.OrderDateTo?.ToString("yyyyMMddHHmmss");
            var storeIdString = filter.StoreId ?? "no-store";
            var categoryString = String.Join("-", filter.Category);
            var cacheKey = $"TotalNumber-{storeIdString}-{startTimeString}-{endTimeString}--{categoryString}";
            var cachedData = await GetCachedDataAsync(cacheKey);
            if (cachedData != null)
            {
                return cachedData; // Daten aus dem Cache zurückgeben
            }

            _context.Database.SetCommandTimeout(300);
            var query = _context.Orders.AsNoTracking();

            if (!string.IsNullOrEmpty(filter.StoreId))
            {
                var storeIdList = filter.StoreId.Split(',').ToList();
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
            await SetCachedDataAsync(cacheKey, storeInfos, DateTime.UtcNow.AddDays(365));
            return storeInfos;
        }
        private async Task<List<StoreInfo>> GetCachedDataAsync(string cacheKey)
        {

            var cacheEntry = await _context.CacheEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(ce => ce.CacheKey == cacheKey && ce.ExpirationDate > DateTime.UtcNow);

            if (cacheEntry != null)
            {
                return JsonConvert.DeserializeObject<List<StoreInfo>>(cacheEntry.JsonValue);
            }

            return null;
        }

        private async Task SetCachedDataAsync(string cacheKey, List<StoreInfo> data, DateTime expirationDate)
        {

            var jsonData = JsonConvert.SerializeObject(data);
            var cacheEntry = await _context.CacheEntries.FindAsync(cacheKey);

            if (cacheEntry != null)
            {
                cacheEntry.JsonValue = jsonData;
                cacheEntry.ExpirationDate = expirationDate;
            }
            else
            {
                _context.CacheEntries.Add(new CacheEntry
                {
                    CacheKey = cacheKey,
                    JsonValue = jsonData,
                    ExpirationDate = expirationDate
                });
            }

            await _context.SaveChangesAsync();
        }

    }
}


