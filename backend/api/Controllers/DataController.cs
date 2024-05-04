using api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;


namespace api.Controllers
{
    [Route("api/Stores")]
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
            var Stores = _context.Stores.ToList();

            return ok(Stores);
        }

        [HttpGet("{storeID}")]

        public IActionResult GetById([FromRoute] string id)
        {
            var Stores = _context.Stores.Find(id);

            if(Stores == null)
            {
                return NotFound();
            }

            return ok(Stores);
        }
    }
}