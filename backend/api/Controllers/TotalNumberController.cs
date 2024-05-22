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

namespace api.Controllers
{


    [Route("api/TotalNumber")]
    [ApiController]
    public class TotalNumberController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IProductRepository _productRepository;
         private readonly IOrderItemRepository _orderItemRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITotalNumberRepository _totalNumberRepository;

        public TotalNumberController(ApplicationDBContext context, ITotalNumberRepository totalNumberRepository, IProductRepository productRepository,
        IStoreRepository storeRepository, ICustomerRepository customerRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
        {
            _context = context;
            _storeRepository = storeRepository;
         _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _totalNumberRepository = totalNumberRepository;
        }

        [HttpGet("TotalStoreRevenue/{storeId}")]
        public IActionResult GetTotalStoreRevenue(string storeId)
        {
            double totalStoreRevenue = _totalNumberRepository.GetTotalStoreRevenue(storeId);
            return Ok(new { Revenue = totalStoreRevenue });
        }

        [HttpGet("FilteredStoreInfo")]
        public IActionResult GetFilteredStoreInfo([FromQuery] FilterRevenue filter)
        {
            var storeInfos = _totalNumberRepository.GetFilteredStoreInfo(filter);
            return Ok(storeInfos);
        }



    }
}
