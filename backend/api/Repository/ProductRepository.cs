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
    
        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProductBySKU(string SKU)
        {
            return _context.Products.Find(SKU);
        }

        public Product GetProductByName(string Name)
        {
            return _context.Products.FirstOrDefault(p => p.Name == Name);
        }
        public Product GetProductByPrice(double Price)
        {
            return _context.Products.FirstOrDefault(p => p.Price == Price);
        }
        public Product GetProductByCategory(string Category)
        {
            return _context.Products.First(p => p.Category == Category);
        }
        public Product GetProductBySize(string Size)
        {
            return _context.Products.FirstOrDefault(p => p.Size == Size);
        }
        public Product GetProductByIngredients(string Ingredients)
        {
            return _context.Products.FirstOrDefault(p => p.Ingredients == Ingredients);
        }
        public Product GetProductByLaunch(string Launch)
        {
            return _context.Products.FirstOrDefault(p => p.Launch == Launch);
        }
        
    }
}