using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Data;


namespace api.Controllers
{
    [Route("api/Order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public OrderController(ApplicationDBContext context)
        {
            _context = context;
            
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var orders = _context.Orders.ToList();

            return Ok(orders);
        }

        [HttpGet("{OrderID}")]
        public IActionResult GetById([FromRoute] string OrderID)
        {
            var Order = _context.Orders.Find(OrderID);

            if(Order == null)
            {
                return NotFound();
            }

            return Ok(Order);
        }
    }
}