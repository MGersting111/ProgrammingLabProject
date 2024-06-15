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
    public interface ICorrelationRepository
    {
    Task<bool> IsModelSupported(string model);
        Task<bool> AreAttributesValid(string model, string xAttribute, string yAttribute);
        Task<(double[] XValues, double[] YValues)> FetchData(string model, DateTime startTime, DateTime endTime, string xAttribute, string yAttribute,string size = null, string category = null );
        double CalculatePearsonCorrelation(double[] xValues, double[] yValues);
         Task<double> CalculateCorrelation(string model, string xAttribute, string yAttribute, DateTime startTime, DateTime endTime);
   

    }
}