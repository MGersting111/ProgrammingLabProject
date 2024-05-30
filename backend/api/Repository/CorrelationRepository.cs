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
            // Überprüfen, ob das angegebene Modell in der Datenbank vorhanden ist
            return await _context.Set<Store>().AnyAsync(s => s.StoreId == model);
        }

        public async Task<bool> AreAttributesValid(string model, string xAttribute, string yAttribute)
        {
            // Überprüfen, ob die angegebenen Attribute im angegebenen Modell vorhanden sind
            if (model == "Store")
            {
                return await _context.Set<Store>().AnyAsync(s => EF.Property<double>(s, xAttribute) != null && EF.Property<double>(s, yAttribute) != null);
            }
            else if (model == "Customer")
            {
                return await _context.Set<Customer>().AnyAsync(c => EF.Property<double>(c, xAttribute) != null && EF.Property<double>(c, yAttribute) != null);
            }
            else if (model == "Order")
            {
                return await _context.Set<Order>().AnyAsync(o => EF.Property<double>(o, xAttribute) != null && EF.Property<double>(o, yAttribute) != null);
            }
            // Weitere Modelle können entsprechend hinzugefügt werden

            return false; // Modell nicht gefunden oder Attribute nicht vorhanden
        }

        public async Task<double> CalculateCorrelation(string model, string xAttribute, string yAttribute)
        {
            // Hier müsste die eigentliche Logik zur Berechnung der Korrelation implementiert werden
            // Beispiel: Pearson-Korrelation, Spearman-Korrelation oder eine andere Methode
            // Für dieses Beispiel wird einfach 0 zurückgegeben
            return 0;
        }
    }
}