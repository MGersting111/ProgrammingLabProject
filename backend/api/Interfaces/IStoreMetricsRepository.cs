using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Data;
using Microsoft.AspNetCore.Mvc;
using api.Dto;
using api.Repository;


namespace api.Interfaces
{
    public interface IStoreMetricsRepository
    {
        Task<List<StoreMetrics>> GetAllStoreMetricsAsync(DateTime fromDate, DateTime toDate);
    }
}