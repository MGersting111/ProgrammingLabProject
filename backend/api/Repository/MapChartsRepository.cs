using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace api.Repository
{
    public class MapChartsRepository : IMapChartsRepository
    {
        private readonly ApplicationDBContext _context;
        public MapChartsRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<dynamic>> GetMapChartDataAsync(string attribute, DateTime startTime, DateTime endTime)
        {
            var storeIds = await _context.Orders
                .Where(o => o.OrderDate >= startTime && o.OrderDate <= endTime)
                .Select(o => o.StoreId)
                .Distinct()
                .ToListAsync()
                .ConfigureAwait(false);

            var storeData = await _context.Stores
                .Where(s => storeIds.Contains(s.StoreId))
                .Select(s => new
                {
                    s.State,
                    s.City,
                    s.Latitude,
                    s.Longitude,
                    StoreId = s.StoreId
                })
                .ToListAsync();

            var result = storeData.Select(store =>
            {
                dynamic item = new System.Dynamic.ExpandoObject();
                item.State = store.State;
                item.City = store.City;
                item.Latitude = store.Latitude;
                item.Longitude = store.Longitude;

                var values = GetAttributeValues(store.StoreId, startTime, endTime, attribute).Result;

                var months = Enumerable.Range(0, (endTime.Year - startTime.Year) * 12 + endTime.Month - startTime.Month + 1)
                                        .Select(offset => new DateTime(startTime.Year, startTime.Month, 1).AddMonths(offset))
                                        .ToList();

                foreach (var month in months)
                {
                    if (month >= startTime && month <= endTime)
                    {
                        var monthValue = values.FirstOrDefault(v => v.Key == month.ToString("MMMM"));
                        if (monthValue.Key != null)
                        {
                            ((IDictionary<string, object>)item).Add(month.ToString("MMMM"), monthValue.Value);
                        }
                    }
                }

                return item;
            }).ToList();

            return result;
        }

        private async Task<Dictionary<string, decimal>> GetAttributeValues(string storeId, DateTime startTime, DateTime endTime, string attribute)
        {
            switch (attribute.ToLower())
            {
                case "revenue":
                    var totalRevenues = await _context.Orders
                        .Where(o => o.StoreId == storeId && o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .GroupBy(o => o.OrderDate.Month)
                        .Select(g => new { Month = g.Key, TotalRevenue = g.Sum(o => o.total) })
                        .ToDictionaryAsync(g => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Month), g => (decimal)g.TotalRevenue)
                        .ConfigureAwait(false);
                    return totalRevenues;

                case "totalcustomers":
                    var totalCustomers = await _context.Orders
                        .Where(o => o.StoreId == storeId && o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .GroupBy(o => o.OrderDate.Month)
                        .Select(g => new { Month = g.Key, TotalCustomers = g.Count() })
                        .ToDictionaryAsync(g => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Month), g => (decimal)g.TotalCustomers)
                        .ConfigureAwait(false);
                    return totalCustomers;

                default:
                    throw new ArgumentException($"Invalid attribute specified: {attribute}");
            }
        }
    }
}