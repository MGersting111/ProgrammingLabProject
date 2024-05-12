using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using api.Data;

namespace api.Repository
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly ApplicationDBContext _context;

    
    public OrderItemRepository(ApplicationDBContext context )
    {
        _context = context;
    }
    public IEnumerable<OrderItem> GetAllOrderItems()
        {
            return _context.OrderItems.ToList();
        }
        public OrderItem GetOrderItemByOrderId(int OrderId)
    {
        return _context.OrderItems.Find(OrderId);
    }
    public OrderItem GetOrderItemBySKU(string SKU)
    {
        return _context.OrderItems.FirstOrDefault(oi => oi.SKU == SKU);
    }
}
}