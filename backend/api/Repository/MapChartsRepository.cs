// Datei: api.Repository/MapChartsRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace api.Repository
{
    public class MapChartsRepository : IMapChartsRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<MapChartsRepository> _logger;

        public MapChartsRepository(ApplicationDBContext context, ILogger<MapChartsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Dictionary<string, object>>> GetMapChartDataAsync(string attribute, DateTime startTime, DateTime endTime)
        {
            var cacheKey = $"MapChart-{attribute}-{startTime.ToString("yyyyMMdd")}-{endTime.ToString("yyyyMMdd")}";
            List<Dictionary<string, object>> cachedData = await GetCachedDataAsync(cacheKey);

            if (cachedData != null)
            {
                _logger.LogInformation("Cache-Eintrag {CacheKey} gefunden. Daten werden aus dem Cache geladen.", cacheKey);
                return cachedData;
            }

            _logger.LogInformation("Cache-Eintrag {CacheKey} nicht gefunden oder abgelaufen. Daten werden neu berechnet.", cacheKey);

            _context.Database.SetCommandTimeout(300); // Timeout für lange Abfragen erhöhen

            var storeIds = await _context.Orders
                .Where(o => o.OrderDate >= startTime && o.OrderDate <= endTime)
                .Select(o => o.StoreId)
                .Distinct()
                .ToListAsync()
                .ConfigureAwait(false); // Asynchronität optimieren

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

            var result = new List<Dictionary<string, object>>();

            foreach (var store in storeData)
            {
                var item = new Dictionary<string, object>
                {
                    { "State", store.State },
                    { "City", store.City },
                    { "Latitude", store.Latitude },
                    { "Longitude", store.Longitude }
                };

                var values = await GetAttributeValues(store.StoreId, startTime, endTime, attribute);

                foreach (var kvp in values)
                {
                    item[kvp.Key] = kvp.Value;
                }

                result.Add(item);
            }

            await SetCachedDataAsync(cacheKey, result, DateTime.UtcNow.AddDays(365)); // Daten im Cache speichern
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
                            .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                            .Select(g => new { g.Key.Year, g.Key.Month, TotalRevenue = g.Sum(o => o.total) })
                            .ToDictionaryAsync(g => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Month)} {g.Year}", g => (decimal)g.TotalRevenue);

                    case "totalcustomers":
                        return await _context.Orders
                            .Where(o => o.StoreId == storeId && o.OrderDate >= startTime && o.OrderDate <= endTime)
                            .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                            .Select(g => new { g.Key.Year, g.Key.Month, TotalCustomers = g.Count() })
                            .ToDictionaryAsync(g => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Month)} {g.Year}", g => (decimal)g.TotalCustomers);

                    default:
                        throw new ArgumentException($"Ungültiges Attribut angegeben: {attribute}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fehler beim Abrufen von Attributwerten für {attribute}: {ex.Message}", ex);
            }
        }

        private async Task<List<Dictionary<string, object>>> GetCachedDataAsync(string cacheKey)
        {
            var cacheEntry = await _context.CacheEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(ce => ce.CacheKey == cacheKey && ce.ExpirationDate > DateTime.UtcNow);

            if (cacheEntry != null && !string.IsNullOrEmpty(cacheEntry.JsonValue))
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(cacheEntry.JsonValue);
                }
                catch (JsonSerializationException ex)
                {
                    _logger.LogError(ex, "Fehler bei der Deserialisierung von Cache-Daten: {CacheKey}", cacheKey);
                    return null;
                }
            }
            else
            {
                _logger.LogInformation("Cache-Eintrag {CacheKey} nicht gefunden oder abgelaufen.", cacheKey);
                return null;
            }
        }

        private async Task SetCachedDataAsync(string cacheKey, List<Dictionary<string, object>> data, DateTime expirationDate)
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
