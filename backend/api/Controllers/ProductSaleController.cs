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
    [Route("api/ProductSales")]
    [ApiController]
    public class ProductSaleController : ControllerBase
    {
        private readonly IProductSaleRepository _productSaleRepository;
        private readonly ApplicationDBContext _context;
        private readonly IProductRepository _productRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITotalNumberRepository _totalNumberRepository;

        public ProductSaleController(IProductSaleRepository productSaleRepository, ApplicationDBContext context, ITotalNumberRepository totalNumberRepository, IProductRepository productRepository,
            IStoreRepository storeRepository, ICustomerRepository customerRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
        {
            _productSaleRepository = productSaleRepository;
            _context = context;
            _storeRepository = storeRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _totalNumberRepository = totalNumberRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductSalesInfo([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var productSaleInfo = await _productSaleRepository.GetProductSaleInfoAsync(fromDate, toDate);
            return Ok(productSaleInfo);
        }
    }
}
