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
        private readonly IStoreRepository _storeRepository;
        public StoreController(ApplicationDBContext context, IStoreRepository storeRepository)
        {
            _context = context;
            _storeRepository = storeRepository;
        }

        [HttpGet]

        public IActionResult GetAll() 
        {
            var stores = _storeRepository.GetAllStores();

            return Ok(Stores);
        }

        [HttpGet("{StoreID}")]

        public IActionResult GetById([FromRoute] string StoreId)
        {
            var store = _storeRepository.GetStoreById(storeId);

            if(StoreId == null)
            {
                return NotFound();
            }

            return Ok(Store);
        }
    }
}