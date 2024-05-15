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

namespace api.Controllers
{
    [Route("api/Order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IOrderRepository _orderRepository;
        public OrderController(ApplicationDBContext context, IOrderRepository orderRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
            
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var orders = _orderRepository.GetAllOrders();

            return Ok(orders);
        }

        [HttpGet ("Only10Order")]
        public IActionResult Get10()
        {
            var orders = _orderRepository.Get10Orders();

            return Ok(orders);
        }

        [HttpGet("{OrderID}")]
        public IActionResult GetOrderById(int OrderId)
        {
            var Order = _orderRepository.GetOrderById(OrderId);

            if(OrderId == null)
            {
                return NotFound();
            }

            return Ok(OrderId);
        }

        [HttpGet("{CustomerID}")]
        public IActionResult GetOrderByCustomerId(string CustomerId)
        {
            var Order = _orderRepository.GetOrderByCustomerId(CustomerId);

            if(CustomerId == null)
            {
                return NotFound();
            }

            return Ok(CustomerId);
        }

        [HttpGet("{storeID}")]
        public IActionResult GetOrderByStoreId(string storeId)
        {
            var Order = _orderRepository.GetOrderByStoreId(storeId);

            if(storeId == null)
            {
                return NotFound();
            }

            return Ok(storeId);
        }

        [HttpGet("{OrderDate}")]
        public IActionResult GetOrderByOrderDate(string OrderDate)
        {
            var Order = _orderRepository.GetOrderByOrderDate(OrderDate);

            if(OrderDate == null)
            {
                return NotFound();
            }

            return Ok(OrderDate);
        }

        [HttpGet("{NItems}")]
        public IActionResult GetOrderByNItems(int NItems)
        {
            var Order = _orderRepository.GetOrderByNItems(NItems);

            if(NItems == null)
            {
                return NotFound();
            }

            return Ok(NItems);
        }
        [HttpGet("{total}")]
        public IActionResult GetOrderByTotal(double total)
        {
            var Order = _orderRepository.GetOrderByTotal(total);

            if(total == null)
            {
                return NotFound();
            }

            return Ok(total);
        }
    }
}