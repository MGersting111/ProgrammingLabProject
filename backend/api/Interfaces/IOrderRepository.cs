using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;


namespace api.Interfaces
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrders();
        Order GetOrderById(int orderId);
        Order GetOrderByStoreId(string storeId);
        Order GetOrderByCustomerId(string customerId);
        Order GetOrderByOrderDate(string orderDate);
        Order GetOrderByNItems(int nItems);
        Order GetOrderByTotal(double total);

        //Task<List<OrderEntryDto>> GetAllOrderEntryDtoByFilter(FilterOrderEntryDto filterDto, int page, int pageSize, string sortColumn, string sortOrder);
        

        //IEnumerable<Order> GetOrdersByStoreId(string storeId);
    }
}
