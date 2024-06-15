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
    public class CorrelationRepository : ICorrelationRepository
    {
        private readonly ApplicationDBContext _context;

        public CorrelationRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<bool> IsModelSupported(string model)
        {
            return model.ToLower() == "store" || model.ToLower() == "order" || model.ToLower() == "product"|| model.ToLower() == "customer";
        }

        public async Task<bool> AreAttributesValid(string model, string xAttribute, string yAttribute)
        {
            switch (model.ToLower())
    {
        case "order":
            var orderProps = typeof(Order).GetProperties().Select(p => p.Name.ToLower()).ToList();
            return orderProps.Contains(xAttribute.ToLower()) && orderProps.Contains(yAttribute.ToLower());

        case "store":
            var validStoreAttributes = new List<string> { "totalrevenue", "ordercount", "averageordervalueperstore", "averageordervalue", "averageordervaluepercustomer", "ordercountperproductperstore", "totalrevenuepercustomerperstore" };
            return validStoreAttributes.Contains(xAttribute.ToLower()) && validStoreAttributes.Contains(yAttribute.ToLower());

        case "product":
            var validProductAttributes = new List<string> { "totalunitssold", "price", "averageordervalueperproduct","totalrevenue"};
            return validProductAttributes.Contains(xAttribute.ToLower()) && validProductAttributes.Contains(yAttribute.ToLower());
        
        case "customer":
                    var validCustomerAttributes = new List<string> { "averageordervalue", "ordercountpercustomer", "totalrevenuepercustomer" };
                    return validCustomerAttributes.Contains(xAttribute.ToLower()) && validCustomerAttributes.Contains(yAttribute.ToLower());

        default:
            return false; // Falls ein ungültiges Modell angegeben wurde
    }
}

       public async Task<(double[] XValues, double[] YValues)> FetchData(string model, DateTime startTime, DateTime endTime, string xAttribute, string yAttribute,string size = null, string category = null)
{
    switch (model.ToLower())
    {
        case "store":
            var storeIds = await _context.Orders
                .Where(o => o.OrderDate >= startTime && o.OrderDate <= endTime)
                .Select(o => o.StoreId)
                .Distinct()
                .ToListAsync()
                .ConfigureAwait(false);

            var xValues = await GetAttributeValues(storeIds, startTime, endTime, xAttribute).ConfigureAwait(false);
            var yValues = await GetAttributeValues(storeIds, startTime, endTime, yAttribute).ConfigureAwait(false);

            return (xValues, yValues);

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
                .ToListAsync()
                .ConfigureAwait(false);

            var xValuesProduct = await GetProductAttributeValues(SKUs, startTime, endTime, xAttribute).ConfigureAwait(false);
            var yValuesProduct = await GetProductAttributeValues(SKUs, startTime, endTime, yAttribute).ConfigureAwait(false);

            return (xValuesProduct, yValuesProduct);
        
        case "customer":
                    var customerIds = await _context.Orders
                        .Where(o => o.OrderDate >= startTime && o.OrderDate <= endTime)
                        .Select(o => o.CustomerId)
                        .Distinct()
                        .ToListAsync()
                        .ConfigureAwait(false);

                    var xValuesCustomer = await GetCustomerAttributeValues(customerIds, startTime, endTime, xAttribute).ConfigureAwait(false);
                    var yValuesCustomer = await GetCustomerAttributeValues(customerIds, startTime, endTime, yAttribute).ConfigureAwait(false);

                    return (xValuesCustomer, yValuesCustomer);
        default:
            throw new ArgumentException("Invalid model specified.");
    }
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
            return totalRevenues.Select(tr => (double)tr).ToArray();
            
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

    
