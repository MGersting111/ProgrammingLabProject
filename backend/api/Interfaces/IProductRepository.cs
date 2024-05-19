using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetAllProducts();
        Product GetProductBySku(string Sku);
        Product GetProductByName(string Name);
        Product GetProductByPrice(double Price);
        Product GetProductByCategory(string Category);
        Product GetProductBySize(string Size);
        Product GetProductByIngredients(string Ingredients);
        Product GetProductByLaunch(string Launch);
    }
}