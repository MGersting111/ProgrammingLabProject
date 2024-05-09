using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface IStoreRepository
{
    Store GetStoreById(string storeId);

    public List<Store> GetAllStores();
    
}


}