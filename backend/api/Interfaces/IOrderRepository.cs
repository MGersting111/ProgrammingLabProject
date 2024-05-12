using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;


namespace api.Interfaces
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAllOrders();
        Order GetOrderById(int orderId);
        Order GetOrderByStoreId(string storeId);
        Order GetOrderByCustomerId(string customerId);
        Order GetOrderByOrderDate(string orderDate);
        Order GetOrderByNItems(int nItems);
        Order GetOrderByTotal(double total);
    }
}
