using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using api.Data;

namespace api.Repository
{
    public class OrderRepository : IOrderRepository
    {
         private readonly ApplicationDBContext _context;


    public Order GetOrderById(int OrderId)
    {
        return _context.Orders.Find(OrderId);
    }

    public Order GetOrderByStoreId(string StoreId)
    {
        return _context.Orders.Find(StoreId);
    }
    }
}