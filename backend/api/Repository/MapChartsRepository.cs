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

    var result = new List<dynamic>();

    foreach (var store in storeData)
    {
        dynamic item = new System.Dynamic.ExpandoObject();
        item.State = store.State;
        item.City = store.City;
        item.Latitude = store.Latitude;
        item.Longitude = store.Longitude;

        var values = await GetAttributeValues(store.StoreId, startTime, endTime, attribute);

        var months = Enumerable.Range(0, (endTime.Year - startTime.Year) * 12 + endTime.Month - startTime.Month + 1)
                                .Select(offset => new DateTime(startTime.Year, startTime.Month, 1).AddMonths(offset))
                                .ToList();

        foreach (var month in months)
        {
            if (month >= startTime && month <= endTime)
            {
                var monthKey = $"{month:MMMM yyyy}"; // Änderung hier, um das Jahr einzuschließen
                var monthValue = values.FirstOrDefault(v => v.Key == month.ToString("MMMM yyyy")); // Änderung hier, um das Jahr einzuschließen
                if (monthValue.Key != null)
                {
                    ((IDictionary<string, object>)item).Add(monthKey, monthValue.Value); // Verwendung von monthKey
                }
            }
        }

        result.Add(item);
    }

    return result;
}

private async Task<Dictionary<string, decimal>> GetAttributeValues(string storeId, DateTime startTime, DateTime endTime, string attribute)
{
    try
    {
        switch (attribute.ToLower())
        {
            case "revenue":
                return await _context.Orders
                    .Where(o => o.StoreId == storeId && o.OrderDate >= startTime && o.OrderDate <= endTime)
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month }) // Gruppierung nach Jahr und Monat
                    .Select(g => new { g.Key.Year, g.Key.Month, TotalRevenue = g.Sum(o => o.total) })
                    .ToDictionaryAsync(g => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Month)} {g.Year}", g => (decimal)g.TotalRevenue); // Schlüssel mit Monat und Jahr

            case "totalcustomers":
                return await _context.Orders
                    .Where(o => o.StoreId == storeId && o.OrderDate >= startTime && o.OrderDate <= endTime)
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month }) // Gruppierung nach Jahr und Monat
                    .Select(g => new { g.Key.Year, g.Key.Month, TotalCustomers = g.Count() })
                    .ToDictionaryAsync(g => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Month)} {g.Year}", g => (decimal)g.TotalCustomers); // Schlüssel mit Monat und Jahr

            default:
                throw new ArgumentException($"Invalid attribute specified: {attribute}");
        }
    }
    catch (Exception ex)
    {
        // Geeignete Fehlerbehandlung
        throw new InvalidOperationException($"Error fetching attribute values for {attribute}: {ex.Message}", ex);
    }
}
    }
}