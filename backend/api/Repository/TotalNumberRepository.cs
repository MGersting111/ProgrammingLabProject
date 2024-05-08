using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.interfaces;
using api.Models;

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