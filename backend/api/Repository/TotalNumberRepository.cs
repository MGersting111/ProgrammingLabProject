using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace api.Repository
{
    public class TotalNumberRepository : ITotalNumberRepository
    {
        
        public Task<List<Store>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}