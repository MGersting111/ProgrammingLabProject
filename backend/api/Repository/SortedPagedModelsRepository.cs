using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Data;
using api.Models;
using api.Dto;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class SortedPagedModelsRepository : ISortedPagedModelsRepository
    {
        private readonly ApplicationDBContext _context;

        public SortedPagedModelsRepository(ApplicationDBContext context)      
        {
            _context = context;
        }

        public async Task<List<Order>> GetSortedPagedOrders(int page, int pageSize, string sortColumn, string sortOrder)
        {
            IQueryable<Order> query = _context.Orders;

            // Sortierung anwenden
            if (sortOrder.ToLower() == "asc")
            {
                query = query.OrderBy(o => EF.Property<object>(o, sortColumn));
            }
            else
            {
                query = query.OrderByDescending(o => EF.Property<object>(o, sortColumn));
            }

            // Paginierung anwenden
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<List<Store>> GetSortedPagedStores(int page, int pageSize, string sortColumn, string sortOrder)
        {
            IQueryable<Store> query = _context.Stores;

            // Sortierung anwenden
            if (sortOrder.ToLower() == "asc")
            {
                query = query.OrderBy(o => EF.Property<object>(o, sortColumn));
            }
            else
            {
                query = query.OrderByDescending(o => EF.Property<object>(o, sortColumn));
            }

            // Paginierung anwenden
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<List<Customer>> GetSortedPagedCustomers(int page, int pageSize, string sortColumn, string sortOrder)
        {
            IQueryable<Customer> query = _context.Customers;

            // Sortierung anwenden
            if (sortOrder.ToLower() == "asc")
            {
                query = query.OrderBy(o => EF.Property<object>(o, sortColumn));
            }
            else
            {
                query = query.OrderByDescending(o => EF.Property<object>(o, sortColumn));
            }

            // Paginierung anwenden
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }
        public async Task<List<Product>> GetSortedPagedProducts(int page, int pageSize, string sortColumn, string sortOrder)
        {
            IQueryable<Product> query = _context.Products;

            // Sortierung anwenden
            if (sortOrder.ToLower() == "asc")
            {
                query = query.OrderBy(o => EF.Property<object>(o, sortColumn));
            }
            else
            {
                query = query.OrderByDescending(o => EF.Property<object>(o, sortColumn));
            }

            // Paginierung anwenden
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }
    }
}