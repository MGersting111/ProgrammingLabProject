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
    [Route("api/Models/Customer")]
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

        [HttpGet]
        public IActionResult GetAll()
        {
            var customers = _customerRepository.GetAllCustomers();

            return Ok(customers);
        }

        [HttpGet("{CustomerID}")]
        public IActionResult GetCustomerById(string CustomerID)
        {
            var customer = _customerRepository.GetCustomerById(CustomerID);

            if(CustomerID == null)
            {
                return NotFound();
            }

            return Ok(CustomerID);
        }

        [HttpGet("{Latitude}")]
        public IActionResult GetCustomerByLatitude(double Latitude)
        {
            var customer = _customerRepository.GetCustomerByLatitude(Latitude);

            if(Latitude == null)
            {
                return NotFound();
            }

            return Ok(Latitude);
        }

        [HttpGet("{Longitude}")]
        public IActionResult GetCustomerByLongitude(double Longitude)
        {
            var customer = _customerRepository.GetCustomerByLongitude(Longitude);

            if(Longitude == null)
            {
                return NotFound();
            }

            return Ok(Longitude);
        }


    }
}