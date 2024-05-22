using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Threading.Tasks;
 using api.Models;

 namespace api.Interfaces
 {
     public interface IOrderItemRepository
     {
        IEnumerable<OrderItem> GetAllOrderItems();
    
         OrderItem GetOrderItemBySKU(string SKU);
         IEnumerable<OrderItem> GetOrderItemByOrderId(int orderId);

     }
 }