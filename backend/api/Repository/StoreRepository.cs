using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using api.Data;

namespace api.Repository
{
    
    public class StoreRepository : IStoreRepository
{
    private readonly ApplicationDBContext _context;

    public StoreRepository(ApplicationDBContext context )
    {
        _context = context;
    }

    public List<Store> GetAllStores()
    {
        return _context.Stores.ToList();
    }

    public Store GetStoreById(string storeId)
    {
        return _context.Stores.Find(storeId);
    }
}
}