using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;


namespace api.Controllers
{
    [Route("api/OrderItem")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public OrderItemController(ApplicationDBContext context)
        {
            _context = context;
            
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var orderItems = _context.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.Order)
            .ToList();

            return Ok(orderItems);
        }

        [HttpGet("{orderId}/{sku}")]
        public IActionResult GetById([FromRoute] string orderId, string sku)
        {
            var orderItem = _context.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.Order)
            .FirstOrDefault(oi => oi.OrderID == orderId && oi.SKU == sku);

            if(orderItem == null)
            {
                return NotFound();
            }

            return Ok(orderItem);
        }
    }
}