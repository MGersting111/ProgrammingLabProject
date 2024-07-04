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
    public interface IMapChartsRepository
    {
        Task<List<Dictionary<string, object>>> GetMapChartDataAsync(string attribute, DateTime startTime, DateTime endTime);
        
    }
}