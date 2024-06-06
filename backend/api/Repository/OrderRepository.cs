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


        public OrderRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders.AsNoTracking().ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int OrderId)
        {
            return await _context.Orders.AsNoTracking().FirstOrDefaultAsync(order => order.OrderId == OrderId);
        }


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