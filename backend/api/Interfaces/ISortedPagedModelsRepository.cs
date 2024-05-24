using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Data;
using Microsoft.AspNetCore.Mvc;
using api.Dto;

namespace api.Interfaces
{
    public interface ISortedPagedModelsRepository
    {
        Task<List<Order>> GetSortedPagedOrders(int page, int pageSize, string sortColumn, string sortOrder);
        Task<List<Store>> GetSortedPagedStores(int page, int pageSize, string sortColumn, string sortOrder);
        Task<List<Customer>> GetSortedPagedCustomers(int page, int pageSize, string sortColumn, string sortOrder);
        Task<List<Product>> GetSortedPagedProducts(int page, int pageSize, string sortColumn, string sortOrder);
    }
}