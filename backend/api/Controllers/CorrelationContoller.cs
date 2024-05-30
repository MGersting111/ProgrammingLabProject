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
    [Route("api/Dto")]
    [ApiController]

    public class CorrelationController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IProductRepository _productRepository;
         private readonly IOrderItemRepository _orderItemRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICorrelationRepository _correlationRepository;

        public CorrelationController(ICorrelationRepository correlationRepository)
        {
            _correlationRepository = correlationRepository;
        }

        // POST api/correlation/calculate
        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateCorrelation([FromBody] CorrelationRequestDto request)
        {
            try
            {
                // Überprüfen, ob das Modell und die Attribute gültig sind
                if (string.IsNullOrEmpty(request.Model) || string.IsNullOrEmpty(request.XAttribute) || string.IsNullOrEmpty(request.YAttribute))
                {
                    return BadRequest("Model, XAttribute, and YAttribute must be provided.");
                }

                // Überprüfen, ob das Modell im Repository unterstützt wird
                if (!_correlationRepository.IsModelSupported(request.Model))
                {
                    return BadRequest($"Model '{request.Model}' is not supported.");
                }

                // Überprüfen, ob die Attribute im Modell vorhanden sind
                if (!_correlationRepository.AreAttributesValid(request.Model, request.XAttribute, request.YAttribute))
                {
                    return BadRequest("One or both of the provided attributes are not valid for the selected model.");
                }

                // Korrelation berechnen
                var correlation = await _correlationRepository.CalculateCorrelation(request.Model, request.XAttribute, request.YAttribute);

                // Ergebnis zurückgeben
                return Ok(correlation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
