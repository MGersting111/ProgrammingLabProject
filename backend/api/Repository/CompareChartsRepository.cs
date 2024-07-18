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
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace api.Repository
{
    public enum ComparisonType
    {
        Store,
        Product,
        Category
    }

    public class CompareChartsRepository : ICompareChartsRepository
    {
        private readonly ApplicationDBContext _context;

        public CompareChartsRepository(ApplicationDBContext context)
        {
            _context = context;

        }

        public async Task<List<ChartsInfo>> GetDiagramDataAsync(FilterCharts filter, ComparisonType comparisonType)
        {
            var startTimeString = filter.StartTime.ToString("yyyyMMddHHmmss"); //metrics for the cache key
            var endTimeString = filter.EndTime.ToString("yyyyMMddHHmmss"); //metrics for the cache key
            var storeIdString = filter.StoreId ?? "no-store"; //metrics for the cache key
            var metricsString = String.Join("-", filter.Metrics); //metrics for the cache key
            var cacheKey = $"CompareCharts-{storeIdString}-{startTimeString}-{endTimeString}-{comparisonType.ToString()}-{metricsString}"; //how the key will look like
            var cachedData = await GetCachedDataAsync(cacheKey); //get the data from the cache
            if (cachedData != null) //if the data is in the cache
            {
                return cachedData; // return the data
            }

            _context.Database.SetCommandTimeout(300);
            var chartsInfos = new List<ChartsInfo>();


            switch (comparisonType)
            {
                case ComparisonType.Store:


                    // Abfrage aller Stores, wenn keine Store-ID angegeben ist
                    var storeQuery = _context.Stores.AsNoTracking();
                    if (!string.IsNullOrEmpty(filter.StoreId))
                    {
                        var storeIdList = filter.StoreId.Split(',').ToList();
                        storeQuery = storeQuery.Where(store => storeIdList.Contains(store.StoreId));
                    }

                    var stores = await storeQuery
                        .Where(store => store.Orders.Any(order => order.OrderDate >= filter.StartTime && order.OrderDate <= filter.EndTime))
                        .Take(filter.Limit ?? int.MaxValue)
                        .ToListAsync();

                    foreach (var store in stores)
                    {
                        var chartsInfo = new ChartsInfo
                        {
                            StoreId = store.StoreId,
                            MetricsByYear = new Dictionary<int, YearlyMetrics>()
                        };

                        var orderMetrics = await _context.Orders
                            .AsNoTracking()
                            .Where(order => order.StoreId == store.StoreId && order.OrderDate >= filter.StartTime && order.OrderDate <= filter.EndTime)
                            .GroupBy(order => new { order.OrderDate.Year, order.OrderDate.Month })
                            .Select(group => new
                            {
                                Year = group.Key.Year,
                                Month = group.Key.Month,
                                TotalRevenue = group.Sum(order => order.total),
                                CustomerCount = group.Select(order => order.CustomerId).Distinct().Count(),
                                OrderCount = group.Count()
                            })
                            .ToListAsync();

                        foreach (var orderMetric in orderMetrics)
                        {
                            var monthNameMetric = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(orderMetric.Month);
                            if (!chartsInfo.MetricsByYear.ContainsKey(orderMetric.Year))
                            {
                                chartsInfo.MetricsByYear[orderMetric.Year] = new YearlyMetrics { Metrics = new Dictionary<string, double>() };
                            }
                            if (filter.Metrics.Contains("totalRevenue"))
                            {
                                chartsInfo.MetricsByYear[orderMetric.Year].Metrics.Add(monthNameMetric, orderMetric.TotalRevenue);
                            }
                            if (filter.Metrics.Contains("customer"))
                            {
                                chartsInfo.MetricsByYear[orderMetric.Year].Metrics.Add(monthNameMetric, orderMetric.CustomerCount);
                            }
                            if (filter.Metrics.Contains("revenuePerCustomer"))
                            {
                                var revenuePerCustomer = orderMetric.CustomerCount > 0 ? orderMetric.TotalRevenue / orderMetric.CustomerCount : 0;
                                chartsInfo.MetricsByYear[orderMetric.Year].Metrics.Add(monthNameMetric, revenuePerCustomer);
                            }
                            if (filter.Metrics.Contains("sales"))
                            {
                                chartsInfo.MetricsByYear[orderMetric.Year].Metrics.Add(monthNameMetric, orderMetric.OrderCount);
                            }
                        }

                        // Berechnen Sie das Gesamttotal für das Jahr und den Store
                        foreach (var year in chartsInfo.MetricsByYear.Keys)
                        {
                            chartsInfo.MetricsByYear[year].Total = (int)chartsInfo.MetricsByYear[year].Metrics.Values.Sum();
                        }
                        chartsInfo.Total = (int)chartsInfo.MetricsByYear.Values.Select(y => y.Total).Sum();

                        chartsInfos.Add(chartsInfo);
                    }
                    break;

                case ComparisonType.Product:
                    _context.Database.SetCommandTimeout(300);

                    var productMetrics = await _context.OrderItems
                        .AsNoTracking()
                        .Where(orderItem => orderItem.Order.OrderDate >= filter.StartTime && orderItem.Order.OrderDate <= filter.EndTime)
                        .GroupBy(orderItem => new { orderItem.Product.Name, Year = orderItem.Order.OrderDate.Year, Month = orderItem.Order.OrderDate.Month })
                        .Select(group => new
                        {
                            ProductName = group.Key.Name,
                            Year = group.Key.Year,
                            Month = group.Key.Month,
                            TotalOrders = group.Count()
                        })
                        .ToListAsync();

                    // Dictionary zum Speichern der konsolidierten Metriken
                    var consolidatedMetrics = new Dictionary<string, Dictionary<int, YearlyMetrics>>();

                    foreach (var productMetric in productMetrics)
                    {
                        if (!consolidatedMetrics.ContainsKey(productMetric.ProductName))
                        {
                            consolidatedMetrics[productMetric.ProductName] = new Dictionary<int, YearlyMetrics>();
                        }

                        var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(productMetric.Month);
                        if (!consolidatedMetrics[productMetric.ProductName].ContainsKey(productMetric.Year))
                        {
                            consolidatedMetrics[productMetric.ProductName][productMetric.Year] = new YearlyMetrics { Metrics = new Dictionary<string, double>() };
                        }
                        consolidatedMetrics[productMetric.ProductName][productMetric.Year].Metrics[monthName] = productMetric.TotalOrders;
                    }

                    // Erstellen der ChartsInfo-Objekte
                    foreach (var item in consolidatedMetrics)
                    {
                        var metricsByYear = new Dictionary<int, YearlyMetrics>();
                        for (int year = filter.StartTime.Year; year <= filter.EndTime.Year; year++)
                        {
                            var metrics = new Dictionary<string, double>();
                            for (int month = 1; month <= 12; month++)
                            {
                                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
                                if (item.Value.ContainsKey(year) && item.Value[year].Metrics.ContainsKey(monthName))
                                {
                                    metrics[monthName] = item.Value[year].Metrics[monthName];
                                }
                                else
                                {
                                    metrics[monthName] = 0;
                                }
                            }

                            var sortedMetrics = metrics.OrderBy(m => DateTime.ParseExact(m.Key, "MMM", CultureInfo.CurrentCulture)).ToDictionary(m => m.Key, m => m.Value);
                            metricsByYear[year] = new YearlyMetrics { Metrics = sortedMetrics, Total = (int)sortedMetrics.Values.Sum() };
                        }

                        var chartsInfo = new ChartsInfo
                        {
                            StoreId = item.Key,
                            MetricsByYear = metricsByYear,
                            Total = (int)metricsByYear.Values.Select(y => y.Total).Sum()
                        };

                        chartsInfos.Add(chartsInfo);
                    }
                    break;

                case ComparisonType.Category:
                    // Erhöhung des Command Timeout
                    _context.Database.SetCommandTimeout(300);  // Setzt den Timeout auf 300 Sekunden

                    var categoryMetrics = await _context.OrderItems
        .AsNoTracking()
        .Where(orderItem => orderItem.Order.OrderDate >= filter.StartTime && orderItem.Order.OrderDate <= filter.EndTime)
        .GroupBy(orderItem => new { orderItem.Product.Category, Year = orderItem.Order.OrderDate.Year, Month = orderItem.Order.OrderDate.Month })
        .Select(group => new
        {
            Category = group.Key.Category,
            Year = group.Key.Year,
            Month = group.Key.Month,
            TotalOrders = group.Count()
        })
        .ToListAsync();

                    // Berechnung der Metriken direkt in der Datenbank
                    var categories = new List<string> { "Classic", "Vegetarian", "Specialty" };
                    foreach (var category in categories)
                    {
                        var chartsInfo = new ChartsInfo
                        {
                            StoreId = category,
                            MetricsByYear = new Dictionary<int, YearlyMetrics>()
                        };

                        for (var date = filter.StartTime; date <= filter.EndTime; date = date.AddMonths(1))
                        {
                            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month);
                            if (!chartsInfo.MetricsByYear.ContainsKey(date.Year))
                            {
                                chartsInfo.MetricsByYear[date.Year] = new YearlyMetrics { Metrics = new Dictionary<string, double>() };
                            }

                            var orderItemsMetrics = await _context.OrderItems
                                .AsNoTracking()
                                .Include(orderItem => orderItem.Product)
                                .Include(orderItem => orderItem.Order)
                                .Where(orderItem => orderItem.Product.Category == category && orderItem.Order.OrderDate.Month == date.Month && orderItem.Order.OrderDate.Year == date.Year)
                                .GroupBy(orderItem => new { orderItem.Order.OrderDate.Year, orderItem.Order.OrderDate.Month })
                                .Select(group => new
                                {
                                    Year = group.Key.Year,
                                    Month = group.Key.Month,
                                    TotalOrders = group.Count()
                                })
                                .ToListAsync();

                            foreach (var orderItemsMetric in orderItemsMetrics)
                            {
                                var monthNameMetric = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(orderItemsMetric.Month);
                                chartsInfo.MetricsByYear[orderItemsMetric.Year].Metrics[monthNameMetric] = orderItemsMetric.TotalOrders;
                            }
                        }

                        // Berechnen Sie das Gesamttotal für die Kategorie
                        foreach (var year in chartsInfo.MetricsByYear.Keys)
                        {
                            chartsInfo.MetricsByYear[year].Total = (int)chartsInfo.MetricsByYear[year].Metrics.Values.Sum();
                        }
                        chartsInfo.Total = (int)chartsInfo.MetricsByYear.Values.Select(y => y.Total).Sum();

                        chartsInfos.Add(chartsInfo);
                    }
                    break;
            }
            // rest of the code
            await SetCachedDataAsync(cacheKey, chartsInfos, DateTime.UtcNow.AddDays(365)); //set the data in the cache
            return chartsInfos;
        }
        private async Task<List<ChartsInfo>> GetCachedDataAsync(string cacheKey) //get the data from the cache method
        {
          
            var cacheEntry = await _context.CacheEntries 
                .AsNoTracking()
                .FirstOrDefaultAsync(ce => ce.CacheKey == cacheKey && ce.ExpirationDate > DateTime.UtcNow);

            if (cacheEntry != null)
            {
                return JsonConvert.DeserializeObject<List<ChartsInfo>>(cacheEntry.JsonValue); 
            }

            return null;
        }

        private async Task SetCachedDataAsync(string cacheKey, List<ChartsInfo> data, DateTime expirationDate) //set the data in the cache method
        {
           
            var jsonData = JsonConvert.SerializeObject(data); //convert the data to json
            var cacheEntry = await _context.CacheEntries.FindAsync(cacheKey); //check if the data is already in the cache

            if (cacheEntry != null) //if the data is already in the cache
            {
                cacheEntry.JsonValue = jsonData; //update the data
                cacheEntry.ExpirationDate = expirationDate;//update the expiration date
            }
            else
            {
                _context.CacheEntries.Add(new CacheEntry
                {
                    CacheKey = cacheKey, //add the data to the cache
                    JsonValue = jsonData,//add the data to the cache
                    ExpirationDate = expirationDate //add the data to the cache
                });
            }

            await _context.SaveChangesAsync(); //save the changes
        }
    }
}
