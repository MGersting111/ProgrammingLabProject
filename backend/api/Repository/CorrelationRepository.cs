using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Data;
using api.Models;
using api.Dto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace api.Repository
{
    public class CorrelationRepository : ICorrelationRepository
    {
        private readonly ApplicationDBContext _context;

        public CorrelationRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        private async Task SetCachedDataAsync(string cacheKey, string jsonData, DateTime expirationDate)
        {
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

        private async Task<(double[] XValues, double[] YValues)> GetCachedDataAsync(string cacheKey)
        {
            var cacheEntry = await _context.CacheEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(ce => ce.CacheKey == cacheKey && ce.ExpirationDate > DateTime.UtcNow);

            return cacheEntry != null
                ? JsonConvert.DeserializeObject<(double[], double[])>(cacheEntry.JsonValue)
                : (null, null); // Oder einen anderen geeigneten Standardwert zurückgeben
        }

        public async Task<bool> IsModelSupported(string model)
        {
            return model.ToLower() == "store" || model.ToLower() == "order" || model.ToLower() == "product" || model.ToLower() == "customer";
        }



        public async Task<bool> AreAttributesValid(string model, string xAttribute, string yAttribute)
        {
            // Using a dictionary to map models to their valid attributes for a more efficient lookup
            var validAttributesByModel = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["order"] = new HashSet<string>(typeof(Order).GetProperties().Select(p => p.Name), StringComparer.OrdinalIgnoreCase),
                ["store"] = new HashSet<string>(new[] { "totalrevenue", "ordercount", "averageordervalueperstore", "averageordervalue", "averageordervaluepercustomer", "ordercountperproductperstore", "totalrevenuepercustomerperstore" }, StringComparer.OrdinalIgnoreCase),
                ["product"] = new HashSet<string>(new[] { "totalunitssold", "price", "averageordervalueperproduct", "totalrevenue" }, StringComparer.OrdinalIgnoreCase),
                ["customer"] = new HashSet<string>(new[] { "averageordervalue", "ordercountpercustomer", "totalrevenuepercustomer" }, StringComparer.OrdinalIgnoreCase)
            };

            if (validAttributesByModel.TryGetValue(model.ToLower(), out var validAttributes))
            {
                return validAttributes.Contains(xAttribute) && validAttributes.Contains(yAttribute);
            }

            return false; // If the model is not found in the dictionary, return false
        }


        public async Task<(double[] XValues, double[] YValues)> FetchData(string model, DateTime startTime, DateTime endTime, string xAttribute, string yAttribute, string size = null, string category = null)
        {
            var cacheKey = $"Correlation{model}-{startTime}-{endTime}-{xAttribute}-{yAttribute}-{size}-{category}";
            var cachedData = await GetCachedDataAsync(cacheKey);

            if (cachedData.XValues != null && cachedData.YValues != null)
            {
                return cachedData;
            }

            double[] xValues, yValues; // Deklaration außerhalb des switch-Blocks

            switch (model.ToLower())
            {
                case "store":
                    var storeIds = await _context.Orders
                        .Where(o => o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .Select(o => o.StoreId)
                        .Distinct()
                        .ToListAsync();

                    xValues = await GetAttributeValues(storeIds, startTime, endTime, xAttribute);
                    yValues = await GetAttributeValues(storeIds, startTime, endTime, yAttribute);
                    break;

                case "product":
                    IQueryable<OrderItem> orderItemsQuery = _context.OrderItems
                        .Where(oi => oi.Order.OrderDate >= startTime && oi.Order.OrderDate <= endTime);

                    if (!string.IsNullOrEmpty(size))
                    {
                        orderItemsQuery = orderItemsQuery.Where(oi => oi.Product.Size.ToLower() == size.ToLower());
                    }

                    if (!string.IsNullOrEmpty(category))
                    {
                        orderItemsQuery = orderItemsQuery.Where(oi => oi.Product.Category.ToLower() == category.ToLower());
                    }

                    var SKUs = await orderItemsQuery
                        .Select(oi => oi.SKU)
                        .Distinct()
                        .ToListAsync();

                    xValues = await GetProductAttributeValues(SKUs, startTime, endTime, xAttribute);
                    yValues = await GetProductAttributeValues(SKUs, startTime, endTime, yAttribute);
                    break;

                case "customer":
                    var customerIds = await _context.Orders
                        .Where(o => o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .Select(o => o.CustomerId)
                        .Distinct()
                        .ToListAsync();

                    xValues = await GetCustomerAttributeValues(customerIds, startTime, endTime, xAttribute);
                    yValues = await GetCustomerAttributeValues(customerIds, startTime, endTime, yAttribute);
                    break;

                default:
                    throw new ArgumentException("Invalid model specified.");
            }

            // Speichern im Cache (nur wenn noch nicht vorhanden)
            var jsonData = JsonConvert.SerializeObject((xValues, yValues));
            await SetCachedDataAsync(cacheKey, jsonData, DateTime.UtcNow.AddDays(1)); // Cache für 1 Tag

            return (xValues, yValues);
        }

        private async Task<double[]> GetCustomerAttributeValues(List<string> customerIds, DateTime startTime, DateTime endTime, string attribute)
        {
            switch (attribute.ToLower())
            {
                case "averageordervalue":
                    var averageOrderValues = await _context.Orders
                        .Where(o => customerIds.Contains(o.CustomerId) && o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .GroupBy(o => o.CustomerId)
                        .Select(g => new
                        {
                            CustomerId = g.Key,
                            TotalRevenue = g.Sum(o => o.total),
                            OrderCount = g.Count()
                        })
                        .OrderBy(g => g.CustomerId) // Sortierung nach CustomerId hinzugefügt
                        .ToListAsync()
                        .ConfigureAwait(false);
                    return averageOrderValues.Select(aov => aov.TotalRevenue / aov.OrderCount).ToArray();

                case "totalrevenuepercustomer":
                    var totalRevenuePerCustomer = await _context.Orders
                        .Where(o => customerIds.Contains(o.CustomerId) && o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .GroupBy(o => o.CustomerId)
                        .Select(g => new
                        {
                            CustomerId = g.Key,
                            TotalRevenue = g.Sum(o => o.total)
                        })
                        .OrderBy(g => g.CustomerId) // Sortierung nach CustomerId hinzugefügt
                        .ToListAsync()
                        .ConfigureAwait(false);

                    return totalRevenuePerCustomer.Select(tr => (double)tr.TotalRevenue).ToArray();

                case "ordercountpercustomer":
                    var orderCountPerCustomer = await _context.Orders
                        .Where(o => customerIds.Contains(o.CustomerId) && o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .GroupBy(o => o.CustomerId)
                        .Select(g => new
                        {
                            CustomerId = g.Key,
                            OrderCount = g.Count()
                        })
                        .OrderBy(g => g.CustomerId) // Sortierung nach CustomerId hinzugefügt
                        .ToListAsync()
                        .ConfigureAwait(false);

                    return orderCountPerCustomer.Select(oc => (double)oc.OrderCount).ToArray();

                default:
                    throw new ArgumentException($"Invalid attribute specified: {attribute}");
            }
        }


        private async Task<double[]> GetProductAttributeValues(List<string> SKUs, DateTime startTime, DateTime endTime, string attribute)
        {
            switch (attribute.ToLower())
            {
                case "averageordervalueperproduct":
                    var averageOrderValues = await _context.OrderItems
                        .Where(oi => SKUs.Contains(oi.SKU) && oi.Order.OrderDate >= startTime && oi.Order.OrderDate <= endTime)
                        .GroupBy(oi => oi.Product.SKU)
                        .Select(g => new
                        {
                            SKU = g.Key,
                            TotalRevenue = g.Sum(oi => oi.Order.total),
                            OrderCount = g.Count()
                        })
                        .ToListAsync()
                        .ConfigureAwait(false);
                    return averageOrderValues.Select(aov => aov.TotalRevenue / aov.OrderCount).ToArray();

                case "totalrevenue":
                    var totalRevenuePerProduct = await _context.OrderItems
                        .Where(oi => SKUs.Contains(oi.SKU) && oi.Order.OrderDate >= startTime && oi.Order.OrderDate <= endTime)
                        .GroupBy(oi => oi.SKU)
                        .Select(g => g.Sum(oi => oi.Order.total))
                        .ToListAsync()
                        .ConfigureAwait(false);
                    return totalRevenuePerProduct.Select(tr => (double)tr).ToArray();

                case "price":
                    var prices = await _context.Products
                        .Where(p => SKUs.Contains(p.SKU))
                        .Select(p => p.Price)
                        .ToListAsync()
                        .ConfigureAwait(false);
                    return prices.Select(p => (double)p).ToArray();

                case "totalunitssold":
                    var totalUnitsSold = await _context.OrderItems
                        .Where(oi => SKUs.Contains(oi.SKU) && oi.Order.OrderDate >= startTime && oi.Order.OrderDate <= endTime)
                        .GroupBy(oi => oi.SKU)
                        .Select(g => g.Count())
                        .ToListAsync()
                        .ConfigureAwait(false);
                    return totalUnitsSold.Select(tus => (double)tus).ToArray();


                default:
                    throw new ArgumentException($"Invalid attribute specified: {attribute}");
            }

        }

        private async Task<double[]> GetAttributeValues(List<string> storeIds, DateTime startTime, DateTime endTime, string attribute)
        {
            switch (attribute.ToLower())
            {
                case "totalrevenue":
                    var totalRevenues = await _context.Orders
                        .Where(o => storeIds.Contains(o.StoreId) && o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .GroupBy(o => o.StoreId)
                        .Select(g => g.Sum(o => o.total))
                        .ToListAsync()
                        .ConfigureAwait(false);
                    return totalRevenues.Select(tr => Math.Floor((double)tr)).ToArray();

                case "ordercount":
                    var orderCounts = await _context.Orders
                        .Where(o => storeIds.Contains(o.StoreId) && o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .GroupBy(o => o.StoreId)
                        .Select(g => g.Count())
                        .ToListAsync()
                        .ConfigureAwait(false);
                    return orderCounts.Select(oc => (double)oc).ToArray();

                case "averageordervalue":
                    var averageOrderValues = await _context.Orders
            .Where(o => storeIds.Contains(o.StoreId) && o.OrderDate >= startTime && o.OrderDate <= endTime)
            .GroupBy(o => o.StoreId)
            .Select(g => new
            {
                StoreId = g.Key,
                TotalRevenue = g.Sum(o => o.total),
                OrderCount = g.Count()
            })
            .ToListAsync()
            .ConfigureAwait(false);

                    return averageOrderValues.Select(aov => aov.TotalRevenue / aov.OrderCount).ToArray();


                //durchschnittliche bestellwert pro store
                case "averageordervalueperstore":
                    var averageOrderValuesPerStore = await _context.Orders
                        .Where(o => storeIds.Contains(o.StoreId) && o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .GroupBy(o => o.StoreId)
                        .Select(g => new
                        {
                            StoreId = g.Key,
                            TotalRevenue = g.Sum(o => o.total),
                            OrderCount = g.Count()
                        })
                        .ToListAsync()
                        .ConfigureAwait(false);
                    return averageOrderValuesPerStore.Select(aov => aov.TotalRevenue / aov.OrderCount).ToArray();


                //durchschnittliche bestellwert pro customer per store
                case "averageordervaluepercustomer":
                    var averageOrderValuesPerCustomer = await _context.Orders
                        .Where(o => o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .GroupBy(o => new { o.StoreId, o.CustomerId })
                        .Select(g => new
                        {
                            StoreId = g.Key.StoreId,
                            CustomerId = g.Key.CustomerId,
                            TotalRevenue = g.Sum(o => o.total),
                            OrderCount = g.Count()
                        })
                        .OrderBy(g => g.StoreId) // Sortierung nach StoreId hinzugefügt
                        .ToListAsync()
                        .ConfigureAwait(false);

                    return averageOrderValuesPerCustomer.Select(aov => aov.TotalRevenue / aov.OrderCount).ToArray();

                //total revenue von BESTELLWERT pro customer pro store
                case "totalrevenuepercustomerperstore":
                    var totalRevenuePerCustomerPerStore = await _context.Orders
                        .Where(o => storeIds.Contains(o.StoreId) && o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .GroupBy(o => new { o.StoreId, o.CustomerId })
                        .Select(g => new
                        {
                            StoreId = g.Key.StoreId,
                            CustomerId = g.Key.CustomerId,
                            TotalRevenue = g.Sum(o => o.total)
                        })
                        .ToListAsync()
                        .ConfigureAwait(false);
                    return totalRevenuePerCustomerPerStore.Select(tr => (double)tr.TotalRevenue).ToArray();


                //ordercount pro produkt pro store
                case "ordercountperproductperstore":
                    var orderCountPerProductPerStore = await _context.OrderItems
                        .Where(oi => storeIds.Contains(oi.Order.StoreId) && oi.Order.OrderDate >= startTime && oi.Order.OrderDate <= endTime)
                        .GroupBy(oi => new { oi.Order.StoreId, oi.SKU })
                        .Select(g => new
                        {
                            StoreId = g.Key.StoreId,
                            ProductSKU = g.Key.SKU,
                            OrderCount = g.Count()
                        })
                        .ToListAsync()
                        .ConfigureAwait(false);
                    return orderCountPerProductPerStore.Select(oc => (double)oc.OrderCount).ToArray();




                default:
                    throw new ArgumentException($"Invalid attribute specified: {attribute}");
            }
        }
        public async Task<double> CalculateCorrelation(string model, string xAttribute, string yAttribute, DateTime startTime, DateTime endTime)
        {
            var data = await FetchData(model, startTime, endTime, xAttribute, yAttribute).ConfigureAwait(false);

            if (data.XValues.Length == 0 || data.YValues.Length == 0 || data.XValues.Length != data.YValues.Length)
            {
                throw new ArgumentException("Insufficient or mismatched data for correlation calculation.");
            }

            double correlation = CalculatePearsonCorrelation(data.XValues, data.YValues);

            return correlation;
        }

        public double CalculatePearsonCorrelation(double[] xValues, double[] yValues)
        {
            double sumX = xValues.Sum();
            double sumY = yValues.Sum();
            double sumXY = xValues.Zip(yValues, (x, y) => x * y).Sum();
            double sumXSquare = xValues.Sum(x => x * x);
            double sumYSquare = yValues.Sum(y => y * y);
            int n = xValues.Length;

            double numerator = n * sumXY - sumX * sumY;
            double denominator = Math.Sqrt((n * sumXSquare - sumX * sumX) * (n * sumYSquare - sumY * sumY));

            if (denominator == 0)
            {
                return 0;
            }

            double correlation = numerator / denominator;

            return correlation;
        }

    }
}


