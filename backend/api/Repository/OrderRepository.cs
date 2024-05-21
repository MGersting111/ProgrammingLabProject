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
    public Order GetOrderByOrderDate(string OrderDate)
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


    public async Task<List<OrderEntry>> GetByFilter(FilterOrderEntryDto filterDto, int page, int pageSize, string sortColumn, string sortOrder)
    {
        IQueryable<OrderEntry> query = createFilterQuery(filterDto);
        query = addOrderLogicToQuery(query, page, pageSize, sortColumn, sortOrder);
        List<OrderEntry> orderEntries = await query.ToListAsync();
        return orderEntries;
    }

    private IQueryable<OrderEntry> createFilterQuery(FilterOrderEntryDto filterDto)
    {
        IQueryable<OrderEntry> filterQuery = _context.Orders.Where(entry =>
        (filterDto.OrderId == null || entry.OrderId == filterDto.OrderId) &&
        (filterDto.NItems == null || entry.NItems == filterDto.NItems)
       
        );
        return filterQuery;
    }
    
    private IQueryable<OrderEntry> addOrderLogicToQuery(IQueryable<OrderEntry> query, int page, int pageSize, string sortColumn, string sortOrder)
    {
        IQueryable<OrderEntry> response;
        sortOrder = sortOrder.ToLower();
        if (sortOrder == "desc")
        {
            response = query.OrderBy(sortColumn + "desc");
        }
        else if ( sortOrder == "asc" || sortOrder == null || sortOrder == "")
        {
            response = query.OrderBy(sortColumn + "asc");
        }
        else
        {
            throw new ArgumentException("Order Type must be 'asc' or 'desc' ", nameof(sortOrder));

        }
        return response.Skip((page - 1) * pageSize).Take(pageSize); ;
    }

    }
}