using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Data;
using api.Models;
using api.Dto;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class GoalRepository : IGoalRepository
    {
        private readonly ApplicationDBContext _context;

        public GoalRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // Methode zum Lesen eines Goals
        public async Task<Goal> GetGoalAsync(int id)
        {
            return await _context.Goals.FindAsync(id);
        }

        // Methode zum Ändern eines Goals
        public async Task UpdateGoalAsync(Goal goal)
        {
            _context.Entry(goal).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Methode zum Löschen eines Goals
        public async Task DeleteGoalAsync(Goal goal)
        {
            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
        }
    }
}