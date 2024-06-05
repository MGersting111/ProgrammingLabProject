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
    public interface IGoalRepository
    {
        Task<Goal> GetGoalAsync(int id);
        Task UpdateGoalAsync(Goal goal);
        Task DeleteGoalAsync(Goal goal);
    }
}