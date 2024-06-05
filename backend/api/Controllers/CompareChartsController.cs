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


    [Route("api/CompareCharts")]
    [ApiController]
    public class CompareChartsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ICompareChartsRepository _compareChartsRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITotalNumberRepository _totalNumberRepository;

        public CompareChartsController(ApplicationDBContext context, ITotalNumberRepository totalNumberRepository, IProductRepository productRepository,
        IStoreRepository storeRepository, ICustomerRepository customerRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository,
        ICompareChartsRepository compareChartsRepository)
        {
            _context = context;
            _storeRepository = storeRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _totalNumberRepository = totalNumberRepository;
            _compareChartsRepository = compareChartsRepository;
        }

       [HttpGet("ChartsInfos")]
        public async Task<ActionResult<List<ChartsInfo>>> GetDiagramDataAsync([FromQuery] FilterCharts filter, ComparisonType comparisonType)
        {
            var storeInfos = await _compareChartsRepository.GetDiagramDataAsync(filter, comparisonType);
            return Ok(storeInfos);
        }

    }
}
