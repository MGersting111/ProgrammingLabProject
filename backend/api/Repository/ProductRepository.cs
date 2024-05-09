using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Data;
using api.Models;

namespace api.Repository
{
    public class ProductRepository : IProductRepository

    {
        private readonly ApplicationDBContext _context;

        public ProductRepository(ApplicationDBContext context )
    {
        _context = context;
    }
        public Product GetProductBySKU(string SKU)
        {
            return _context.Products.Find(SKU);
        }
    }
}