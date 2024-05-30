using api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using api.Models;
using MySql.Data.EntityFrameworkCore.Extensions;
using api.Interfaces;
using api.Repository;

namespace api.Controllers
{
    [Route("api/Models")]
    [ApiController]
    public class SortedPagedModelsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IProductRepository _productRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ISortedPagedModelsRepository _sortedPagedModelsRepository;

        public SortedPagedModelsController(ApplicationDBContext context, ISortedPagedModelsRepository sortedPagedModelsRepository, IProductRepository productRepository,
        IStoreRepository storeRepository, ICustomerRepository customerRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
        {
            _context = context;
            _storeRepository = storeRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _sortedPagedModelsRepository = sortedPagedModelsRepository;
        }

         [HttpGet("SortedPagedOrders")]
        public async Task<IActionResult> GetSortedPagedOrders([FromQuery] int page = 1, int pageSize = 5, string sortColumn = "OrderId", string sortOrder = "asc")
        {
            var pagedOrders = await _sortedPagedModelsRepository.GetSortedPagedOrders(page, pageSize, sortColumn, sortOrder);
            return Ok(pagedOrders);
        }

        [HttpGet("SortedPagedStores")]
        public async Task<IActionResult> GetSortedPagedStores([FromQuery] int page = 1, int pageSize = 5, string sortColumn = "StoreId", string sortOrder = "asc")
        {
        var pagedStoresWithRevenue = await _sortedPagedModelsRepository.GetSortedPagedStoresWithRevenue(page, pageSize, sortColumn, sortOrder);
        return Ok(pagedStoresWithRevenue);
        }

        [HttpGet("SortedPagedCustomers")]
        public async Task<IActionResult> GetSortedPagedCustomers([FromQuery] int page = 1, int pageSize = 5, string sortColumn = "CustomerId", string sortOrder = "asc")
        {
            var pagedCustomers = await _sortedPagedModelsRepository.GetSortedPagedCustomers(page, pageSize, sortColumn, sortOrder);
            return Ok(pagedCustomers);
        }

        [HttpGet("SortedPagedProducts")]
        public async Task<IActionResult> GetSortedPagedProducts([FromQuery] int page = 1, int pageSize = 5, string sortColumn = "SKU", string sortOrder = "asc")
        {
            var pagedProducts = await _sortedPagedModelsRepository.GetSortedPagedProducts(page, pageSize, sortColumn, sortOrder);
            return Ok(pagedProducts);
        }
    }
}
