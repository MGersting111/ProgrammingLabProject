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
    [Route("api/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IProductRepository _productRepository;
        public ProductController(ApplicationDBContext context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
            
        }

        [HttpGet ("Product")]
        public IActionResult GetAll()
        {
            var products = _productRepository.GetAllProducts();

            return Ok(products);
        }

        [HttpGet("{SKU}")]
        public IActionResult GetProductBySKU(string SKU)
        {
            var product = _productRepository.GetProductBySKU(SKU);

            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        [HttpGet("name/{Name}")]
        public IActionResult GetProductByName(string Name)
        {
            var product = _productRepository.GetProductByName(Name);

            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        [HttpGet("price/{Price:double}")]
        public IActionResult GetProductByPrice(double Price)
        {
            var product = _productRepository.GetProductByPrice(Price);

            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        [HttpGet("category/{Category}")]
        public IActionResult GetProductByCategory(string Category)
        {
            var product = _productRepository.GetProductByCategory(Category);

            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        [HttpGet("size/{Size}")]
        public IActionResult GetProductBySize(string Size)
        {
            var product = _productRepository.GetProductBySize(Size);

            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        [HttpGet("ingredients/{Ingredients}")]
        public IActionResult GetProductByIngredients(string Ingredients)
        {
            var product = _productRepository.GetProductByIngredients(Ingredients);

            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        [HttpGet("launch{Launch}")]
        public IActionResult GetProductByLaunch(string Launch)
        {
            var product = _productRepository.GetProductByLaunch(Launch);

            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}