using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Data;
using Microsoft.AspNetCore.Mvc;
using api.Dto;

namespace api.Interfaces
{
    public interface IStoreChartsRepository
    {
        Task<List<StoreChartsInfo>> GetStoreChartsInfoAsync(DateTime StartDate, DateTime EndtDate);
    

    }
}