using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Data;


namespace api.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public ProductController(ApplicationDBContext context)
        {
            _context = context;
            
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _context.Products.ToList();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] string id)
        {
            var product = _context.Products.Find(id);

            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}