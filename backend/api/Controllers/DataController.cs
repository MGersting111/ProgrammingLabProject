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
    [Route("api/Stores")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly StoreService _service;
        public DataController(ApplicationDBContext context, StoreService service)
        {
            _context = context;
            _service = service;
        }

        [HttpGet]

        public IActionResult GetAll() 
        {
            //ruft Methode in StoreService auf
            var allStores = _service.getAll();
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