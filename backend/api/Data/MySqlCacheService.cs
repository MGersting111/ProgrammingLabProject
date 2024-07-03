using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace api.Data
{
    public class MySqlCacheService
    {
         private readonly ApplicationDBContext _context;

    public MySqlCacheService(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<T> GetAsync<T>(string cacheKey)
    {
        var cacheEntry = await _context.CacheEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(ce => ce.CacheKey == cacheKey && ce.ExpirationDate > DateTime.UtcNow);

        if (cacheEntry != null)
        {
            return JsonConvert.DeserializeObject<T>(cacheEntry.JsonValue);
        }

        return default(T);
    }

    public async Task SetAsync<T>(string cacheKey, T value, TimeSpan expiration)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        var cacheEntry = new CacheEntry
        {
            CacheKey = cacheKey,
            JsonValue = jsonValue,
            ExpirationDate = DateTime.UtcNow.Add(expiration)
        };

        _context.CacheEntries.Update(cacheEntry);
        await _context.SaveChangesAsync();
    }
    }
}