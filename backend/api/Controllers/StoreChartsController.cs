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
using api.Repository;
using api.Dto;

namespace api.Controllers
{
    [Route("api/StoreChartsController")]
    [ApiController]
    public class StoreChartsController : ControllerBase
    {
        
         private readonly IStoreChartsRepository _storeChartsRepository;

        public StoreChartsController(IStoreChartsRepository storeChartsRepository)
        {
            _storeChartsRepository = storeChartsRepository;
        
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreChartsInfo([FromQuery] DateTime StartDate, [FromQuery] DateTime EndDate)
        {
            var storeChartsInfo = await _storeChartsRepository.GetStoreChartsInfoAsync(StartDate, EndDate);
            return Ok(storeChartsInfo);
        }
    }
}
