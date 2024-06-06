using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Dto;



namespace api.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order> GetOrderByIdAsync(int OrderId);
        Order GetOrderByStoreId(string storeId);
        Order GetOrderByCustomerId(string customerId);
        Order GetOrderByOrderDate(DateTime orderDate);
        Order GetOrderByNItems(int nItems);
        Order GetOrderByTotal(double total);

        //Task<List<OrderEntryDto>> GetAllOrderEntryDtoByFilter(FilterOrderEntryDto filterDto, int page, int pageSize, string sortColumn, string sortOrder);

        //Task<List<OrderEntryDto>> GetAllOrderEntryDtosByFilter(FilterOrderEntryDto filterDto, int page, int pagesize, string sortColumn, string sortOrder);




        //int GetTotalFilterRecords(FilterOrderEntryDto filterDto);
        //IEnumerable<Order> GetOrdersByStoreId(string storeId);
    }
}
