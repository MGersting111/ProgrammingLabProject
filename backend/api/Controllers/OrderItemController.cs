// using api.Data;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using System.Data.SqlClient;
// using MySql.Data.MySqlClient;
// using api.Models;
// using MySql.Data.EntityFrameworkCore.Extensions;
// using api.Interfaces;


// namespace api.Controllers
// {
//     [Route("api/OrderItem")]
//     // [ApiController]
//     public class OrderItemController : ControllerBase
//     {
//         //private readonly ApplicationDBContext _context;
//         private readonly IOrderItemRepository _orderItemRepository;
//         public OrderItemController(IOrderItemRepository orderItemRepository)
//         // {
//             //_context = context;
//             _orderItemRepository = orderItemRepository;
            
//         }

//        [HttpGet]
//        public IActionResult GetAll()
//         {
//            var orderItems = _orderItemRepository.GetAllOrderItems();
      

//             return Ok(orderItems);
//         }

//         [HttpGet("{OrderId}")]
//         public IActionResult GetOrderItemByOrderId(int OrderId)
//         {
//             var orderItem = _orderItemRepository.GetOrderItemByOrderId(OrderId);

//             if(OrderId == null)
//           {
//                 return NotFound();
//           }
//           return Ok(OrderId);
//         }
//         [HttpGet("{SKU}")]
//         public IActionResult GetOrderItemBySKU(string SKU)
//         {
//             var orderItem = _orderItemRepository.GetOrderItemBySKU(SKU);

//             if(SKU == null)
//           {
//                 return NotFound();
//           }
//           return Ok(SKU);
//         }

//     }
// }