using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using api.Data;
using api.Dto;

namespace api.Repository
{
    public class OrderRepository : IOrderRepository 
    {
         private readonly ApplicationDBContext _context;


    public OrderRepository(ApplicationDBContext context )
    {
        _context = context;
    }
    
    public IEnumerable<Order> GetOrders()
        {
            return _context.Orders.Take(100).ToList();
        }
        public Order GetOrderById(int OrderId)
    {
        return _context.Orders.Find(OrderId);
    }
    // public IEnumerable<Order> GetOrdersByStoreId(string storeId)
    // {
    // return _context.Orders.Where(order => order.StoreId == storeId).ToList();
    // }

    public Order GetOrderByStoreId(string StoreId)
    {
        return _context.Orders.FirstOrDefault(o => o.StoreId == StoreId);
    }

    public Order GetOrderByCustomerId(string CustomerId)
    {
        return _context.Orders.FirstOrDefault(o => o.CustomerId == CustomerId);
    }
    public Order GetOrderByOrderDate(DateTime OrderDate)
    {
        return _context.Orders.FirstOrDefault(o => o.OrderDate == OrderDate);
    }
    public Order GetOrderByNItems(int NItems)
    {
        return _context.Orders.FirstOrDefault(o => o.NItems == NItems);
    }
    public Order GetOrderByTotal(double total)
    {
        return _context.Orders.FirstOrDefault(o => o.total == total);
    }


    }
}