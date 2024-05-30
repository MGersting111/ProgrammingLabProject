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
            return model.ToLower() == "store" || model.ToLower() == "order";
        }

        public async Task<bool> AreAttributesValid(string model, string xAttribute, string yAttribute)
        {
            if (model.ToLower() == "order")
            {
                var orderProps = typeof(Order).GetProperties().Select(p => p.Name.ToLower()).ToList();
                return orderProps.Contains(xAttribute.ToLower()) && orderProps.Contains(yAttribute.ToLower());
            }
            else if (model.ToLower() == "store")
            {
                var validAttributes = new List<string> { "totalrevenue", "ordercount" };
                return validAttributes.Contains(xAttribute.ToLower()) && validAttributes.Contains(yAttribute.ToLower());
            }
            return false;
        }

       public async Task<(double[] XValues, double[] YValues)> FetchData(string model, DateTime startTime, DateTime endTime, string xAttribute, string yAttribute)
{
    if (model.ToLower() == "store")
    {
        var storeIds = await _context.Orders
            .Where(o => o.OrderDate >= startTime && o.OrderDate <= endTime)
            .Select(o => o.StoreId)
            .Distinct()
            .ToListAsync();

        var xValuesTask = GetAttributeValues(storeIds, startTime, endTime, xAttribute);
        var yValuesTask = GetAttributeValues(storeIds, startTime, endTime, yAttribute);

        var xValues = await xValuesTask;
        var yValues = await yValuesTask;

        return (xValues, yValues);
    }

    throw new ArgumentException("Invalid model specified.");
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
                .ToListAsync();
            return totalRevenues.Select(tr => (double)tr).ToArray();
            
        case "ordercount":
            var orderCounts = await _context.Orders
                .Where(o => storeIds.Contains(o.StoreId) && o.OrderDate >= startTime && o.OrderDate <= endTime)
                .GroupBy(o => o.StoreId)
                .Select(g => g.Count())
                .ToListAsync();
            return orderCounts.Select(oc => (double)oc).ToArray();
            
        default:
            throw new ArgumentException($"Invalid attribute specified: {attribute}");
    }
}



public async Task<double> CalculateCorrelation(string model, string xAttribute, string yAttribute, DateTime startTime, DateTime endTime)
        {
            var data = await FetchData(model, startTime, endTime, xAttribute, yAttribute);

            // Prüfe, ob Daten zum Berechnen der Korrelation vorhanden sind
            if (data.XValues.Length == 0 || data.YValues.Length == 0 || data.XValues.Length != data.YValues.Length)
            {
                throw new ArgumentException("Insufficient or mismatched data for correlation calculation.");
            }

            // Berechne die Korrelation
            double correlation = CalculatePearsonCorrelation(data.XValues, data.YValues);

            return correlation;
        }

        // Eine Beispielimplementierung der Pearson-Korrelationskoeffizientenberechnung
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
                // Wenn der Nenner 0 ist, gibt es keine Korrelation.
                return 0;
            }

            double correlation = numerator / denominator;

            return correlation;
        }
    }
}