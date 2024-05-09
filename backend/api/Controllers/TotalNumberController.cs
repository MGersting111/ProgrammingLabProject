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
namespace api.Controllers
{
    

    [Route("api/TotalNumber")]
    [ApiController]
    public class TotalNumberController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ProductRepository _productRepository;
        private readonly OrderItemRepository _orderItemRepository;
        private readonly StoreRepository _storeRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly OrderRepository _orderRepository;

        public TotalNumberController(ApplicationDBContext context, ProductRepository productRepository, OrderItemRepository orderItemRepository, StoreRepository storeRepository, 
        CustomerRepository customerRepository, OrderRepository orderRepository)
        {
            _context = context;
            _customerRepository = customerRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
        }

        [HttpGet("TotalStoreRevenue")]

        public IActionResult GetStoreRevenue(string StoreId) 
        {
            var Store = _storeRepository.GetStoreId(StoreId);
            if (StoreId == null)
                return NotFound();


            var Order = _orderRepository.GetOrderByStoreId(StoreId);
            var OrderItem = _orderItemRepository.GetOrderItemByOrderId(OrderId);
            var Product = _productRepository.GetProductBySku(Sku);
            var totalStoreRevenue = Store.sum(revenue => Product.Price * Order.nItems);

            return Ok(totalStoreRevenue);
        }

        [HttpGet("{StoreID}")]

        public IActionResult GetById([FromRoute] string StoreId)
        {
            var Store = _context.TotalNumber.Find(StoreId);

            if(Store == null)
            {
                return NotFound();
            }

            return Ok(TotalNumber);
        }
    }
} 
