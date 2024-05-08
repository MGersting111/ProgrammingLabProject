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
    [Route("api/Models/Store")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public StoreController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]

        public IActionResult GetAll() 
        {
            var Stores = _context.Stores.ToList();

            return Ok(Stores);
        }

        [HttpGet("{StoreID}")]

        public IActionResult GetById([FromRoute] string StoreId)
        {
            var Store = _context.Stores.Find(StoreId);

            if(Store == null)
            {
                return NotFound();
            }

            return Ok(Store);
        }
    }
}