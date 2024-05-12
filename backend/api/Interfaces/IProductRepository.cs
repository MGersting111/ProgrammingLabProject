using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAllProducts();
        Product GetProductById(string SKU);
        Product GetProductByName(string Name);
        Product GetProductByPrice(double Price);
        Product GetProductByCategory(string Category);
        Product GetProductBySize(string Size);
        Product GetProductByIngredients(string Ingredients);
        Product GetProductByLaunch(string Launch);
    }
}