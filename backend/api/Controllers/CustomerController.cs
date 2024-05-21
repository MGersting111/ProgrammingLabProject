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
    [Route("api/Models/Customer/")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ICustomerRepository _customerRepository;
        public CustomerController(ApplicationDBContext context, ICustomerRepository customerRepository)
        {
            _context = context;
            _customerRepository = customerRepository;
            
        }

   
        [HttpGet ("Customer")]
        public IActionResult GetCustomers()
        {
            var customers = _customerRepository.GetCustomers();

            return Ok(customers);
        }

        [HttpGet("{CustomerId}")]
        public IActionResult GetCustomerById(string CustomerId)
        {
            var customer = _customerRepository.GetCustomerById(CustomerId);

            if(customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpGet("latitude/{Latitude:double}")]
        public IActionResult GetCustomerByLatitude(double Latitude)
        {
            var customer = _customerRepository.GetCustomerByLatitude(Latitude);

            if(customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpGet("longitude/{Longitude:double}")]
        public IActionResult GetCustomerByLongitude(double Longitude)
        {
            var customer = _customerRepository.GetCustomerByLongitude(Longitude);

            if(customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }


        


    }
}