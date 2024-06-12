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

namespace api.Repository
{
   public class ProductSaleRepository : IProductSaleRepository
{
    private readonly ApplicationDBContext _context;

    public ProductSaleRepository(ApplicationDBContext context)
    {
        _context = context;
    }

   
   public async Task<ProductSaleInfo> GetProductSaleInfoAsync(DateTime fromDate, DateTime toDate)
{
    var productSaleInfo = new ProductSaleInfo
    {
        FromDate = fromDate,
        ToDate = toDate
    };

    productSaleInfo.ProductSales = await _context.OrderItems
        .Where(orderItem => orderItem.Order.OrderDate >= fromDate && orderItem.Order.OrderDate <= toDate)
        .GroupBy(orderItem => new { orderItem.Product.Name, orderItem.Product.Category, orderItem.Product.Size })
        .Select(group => new ProductSale
        {
            ProductName = group.Key.Name,
            Category = group.Key.Category,
            Size = group.Key.Size,
            Quantity = group.Count()
        })
        .ToListAsync();

    return productSaleInfo;
}

}
}
