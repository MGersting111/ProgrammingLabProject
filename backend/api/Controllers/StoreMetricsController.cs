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

    [Route("api/[controller]")]
    [ApiController]
    public class StoreMetricsController : ControllerBase
    {

        private readonly IStoreMetricsRepository _storeMetricsRepository;
        public StoreMetricsController(IStoreMetricsRepository storeMetricsRepository) 
        {
            _storeMetricsRepository = storeMetricsRepository;}

        [HttpGet]
        public async Task<ActionResult<List<StoreMetrics>>> GetStoreMetrics(DateTime fromDate, DateTime toDate)
        {
            var storeMetrics = await _storeMetricsRepository.GetAllStoreMetricsAsync(fromDate, toDate);
            return Ok(storeMetrics);
        }
    }
}