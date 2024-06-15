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
    public class CorrelationController : ControllerBase
    {
        private readonly ICorrelationRepository _correlationRepository;

        public CorrelationController(ICorrelationRepository correlationRepository)
    {
        _correlationRepository = correlationRepository;
    }

    [HttpGet("Calculate")]
public async Task<IActionResult> GetData([FromQuery] CorrelationRequestDto request)
{
    if (request == null || string.IsNullOrEmpty(request.FirstModel) || string.IsNullOrEmpty(request.XAttribute) || string.IsNullOrEmpty(request.YAttribute))
    {
        return BadRequest("Invalid request parameters.");
    }

    if (!await _correlationRepository.IsModelSupported(request.FirstModel).ConfigureAwait(false))
    {
        return BadRequest("Unsupported model.");
    }

    if (!await _correlationRepository.AreAttributesValid(request.FirstModel, request.XAttribute, request.YAttribute).ConfigureAwait(false))
    {
        return BadRequest("Invalid attributes for the specified model.");
    }

    var data = await _correlationRepository.FetchData(request.FirstModel, request.StartTime, request.EndTime, request.XAttribute, request.YAttribute, request.Size, request.Category).ConfigureAwait(false);

    var sortedData = data.XValues.Zip(data.YValues, (x, y) => new { X = x, Y = y })
                                  .OrderBy(pair => pair.X)
                                  .ToList();

    var sortedXValues = sortedData.Select(pair => pair.X).ToArray();
    var sortedYValues = sortedData.Select(pair => pair.Y).ToArray();

    var correlation = _correlationRepository.CalculatePearsonCorrelation(sortedXValues, sortedYValues);

    var response = new
    {
        XValues = sortedXValues,
        YValues = sortedYValues,
        Correlation = correlation
    };

    return Ok(response);
}

}
}