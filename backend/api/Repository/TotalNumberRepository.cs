using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Data;
using api.Models;


namespace api.Repository
{
    public class TotalNumberRepository : ITotalNumberRepository
    {
    private readonly ApplicationDBContext _context;
    
    

    public TotalNumberRepository(ApplicationDBContext context)
    {
        _context = context;

    }

     public double GetTotalStoreRevenue(string storeId)
    {
        var orders = _context.Orders.Where(order => order.StoreId == storeId);
        return orders.Sum(order => order.total);
    }
    }
}