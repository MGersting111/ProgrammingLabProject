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
    using api.Dto;

    namespace api.Controllers
    {
        [Route("api/Order")]
        [ApiController]
        public class OrderController : ControllerBase
        {
            private readonly ApplicationDBContext _context;
            private readonly IOrderRepository _orderRepository;
            public OrderController(ApplicationDBContext context, IOrderRepository orderRepository)
            {
                _context = context;
                _orderRepository = orderRepository;
                
            }

    
            [HttpGet ("Order")]
            public IActionResult GetOrders()
            {
                var orders = _orderRepository.GetOrders();

                return Ok(orders);
            }

            [HttpGet("by-id/{OrderId:int}")]
            public IActionResult GetOrderById(int OrderId)
            {
                var order = _orderRepository.GetOrderById(OrderId);

                if(order == null)
                {
                    return NotFound();
                }

                return Ok(order);
            }

            [HttpGet("by-customer/{CustomerId}")]
            public IActionResult GetOrderByCustomerId(string CustomerId)
            {
                var order = _orderRepository.GetOrderByCustomerId(CustomerId);

                if(order == null)
                {
                    return NotFound();
                }

                return Ok(order);
            }

            [HttpGet("by-store/{StoreId}")]
            public IActionResult GetOrderByStoreId(string StoreId)
            {
                var order = _orderRepository.GetOrderByStoreId(StoreId);

                if(order == null)
                {
                    return NotFound();
                }

                return Ok(order);
            }

            [HttpGet("by-date/{OrderDate}")]
            public IActionResult GetOrderByOrderDate(string OrderDate)
            {
                var order = _orderRepository.GetOrderByOrderDate(OrderDate);

                if(order == null)
                {
                    return NotFound();
                }

                return Ok(order);
            }

            [HttpGet("by-items/{NItems:int}")]
            public IActionResult GetOrderByNItems(int NItems)
            {
                var order = _orderRepository.GetOrderByNItems(NItems);

                if(order == null)
                {
                    return NotFound();
                }

                return Ok(order);
            }
            [HttpGet("by-total/{total:double}")]
            public IActionResult GetOrderByTotal(double total)
            {
                var order = _orderRepository.GetOrderByTotal(total);

                if(order == null)
                {
                    return NotFound();
                }

                return Ok(order);
            }

            [HttpPost]
            public async Task<IActionResult> GetByFilter([FromBody] FilterOrderEntryDto filterDto, [FromQuery] int page = 1, int pagesize=5, string sortColumn = "OrderId" , string sortOrder = "asc")
            {
                List<OrderEntryDto> orderEntryDtos = await _orderRepository.GetAllOrderEntryDtosByFilter(filterDto, page, pagesize, sortColumn, sortOrder);
                if(orderEntryDtos != null)
                {
                    int totalFilterCount = _orderRepository.GetTotalFilterRecords(filterDto);
                    Response<List<orderEntryDto>> pagedResponse = _pagedResponseRepository.createPagedResponse(orderEntryDtos, page, pagesize, sortColumn, sortOrder, this._baseUri, totalFilterCount );
                    return Ok(pagedResponse);
                }
                return NotFound();
            }
            
        }
    }