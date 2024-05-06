using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.services
{
    public class StoreService
    {
        List<Store> getAll(){
            List<Store> allStores = repo.getAll();
            return allStores;
        }

        StoreService getStore(string id){
            Store? store = repo.getStrore(id);
            return store;
        }
    }
}