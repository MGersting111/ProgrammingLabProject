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
using api.Dto;

namespace api.Controllers
{
    [Route("api/StoreChartsController")]
    [ApiController]
    public class StoreChartsController : ControllerBase
    {
        private readonly IProductSaleRepository _productSaleRepository;
        private readonly ApplicationDBContext _context;
        private readonly IProductRepository _productRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITotalNumberRepository _totalNumberRepository;
         private readonly IStoreChartsRepository _storeChartsRepository;

        public StoreChartsController(IStoreChartsRepository storeChartsRepository, ApplicationDBContext context, ITotalNumberRepository totalNumberRepository, IProductRepository productRepository,
            IStoreRepository storeRepository, ICustomerRepository customerRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
        {
            _storeChartsRepository = storeChartsRepository;
            _context = context;
            _storeRepository = storeRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _totalNumberRepository = totalNumberRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreChartsInfo([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var storeChartsInfo = await _storeChartsRepository.GetStoreChartsInfoAsync(fromDate, toDate);
            return Ok(storeChartsInfo);
        }
    }
}
