using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Data;
using Microsoft.AspNetCore.Mvc;

namespace api.Interfaces
{
    public interface ITotalNumberRepository
    {
        double GetTotalStoreRevenue(string storeId);


        Dictionary<string, double> GetFilteredStoreRevenues(FilterRevenue filter);
    }
}