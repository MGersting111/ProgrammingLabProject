using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Data;

namespace api.Controllers
{
    [Route("api/Customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public CustomerController(ApplicationDBContext context)
        {
            _context = context;
            
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var customers = _context.Customers.ToList();

            return Ok(customers);
        }

        [HttpGet("{CustomerID}")]
        public IActionResult GetById([FromRoute] string CustomerID)
        {
            var customer = _context.Customers.Find(CustomerID);

            if(customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
    }
}