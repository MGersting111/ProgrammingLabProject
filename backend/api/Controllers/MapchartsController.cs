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
    [ApiController]
    [Route("api/[controller]")]
    public class MapchartsController : ControllerBase
    {
        private readonly IMapChartsRepository _mapChartsRepository;

        public MapchartsController(IMapChartsRepository mapChartsRepository)
        {
            _mapChartsRepository = mapChartsRepository;
        }

    [HttpGet("MapChart")]
        public async Task<IActionResult> GetMapChartData([FromQuery] StoreDataDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Attribute))
            {
                return BadRequest("Invalid request parameters.");
            }

            var mapchartData = await _mapChartsRepository.GetMapChartDataAsync(request.Attribute, request.StartTime, request.EndTime).ConfigureAwait(false);

            return Ok(mapchartData);
        }
    }
}
