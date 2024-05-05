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
    [Route("api/Models/Stores")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public DataController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]

        public IActionResult GetAll() 
        {
            var Stores = _context.StoreData.ToList();

            return Ok(Stores);
        }

        [HttpGet("{Store}")]

        public IActionResult GetById([FromRoute] string id)
        {
            var Stores = _context.StoreData.Find(id);

            if(Stores == null)
            {
                return NotFound();
            }

            return Ok(Stores);
        }
    }
}