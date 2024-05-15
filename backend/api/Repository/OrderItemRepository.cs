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
    
    public OrderItem GetOrderItemBySKU(string SKU)
    {
        return _context.OrderItems.FirstOrDefault(oi => oi.SKU == SKU);
    }

    public IEnumerable<OrderItem> GetOrderItemByOrderId(int orderId)
{
    return _context.OrderItems.Where(orderItem => orderItem.OrderId == orderId).ToList();
}
}
}