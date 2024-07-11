using api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using api.Models;
using MySql.Data.EntityFrameworkCore.Extensions;
using api.Interfaces;
using api.Repository;
using api.Dto;

namespace api.Controllers
{
    [Route("api/Goal")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        private readonly IGoalRepository _goalRepository;
        private readonly ApplicationDBContext _context;

        public GoalController(IGoalRepository goalRepository, ApplicationDBContext context)
        {
            _goalRepository = goalRepository;
            _context = context;

        }

        // GET: api/Goal/5
        [HttpGet("id")]
        public async Task<ActionResult<IEnumerable<Goal>>> GetGoal(int? id)
        {
            if (id == null)
            {
                // Wenn keine ID übergeben wird, alle Goals zurückgeben
                var allGoals = await _context.Goals.ToListAsync();
                return Ok(allGoals);
            }
            else
            {
                // Wenn eine ID übergeben wird, das entsprechende Goal zurückgeben
                var goal = await _goalRepository.GetGoalAsync(id.Value);
                if (goal == null)
                {
                    return NotFound();
                }
                return Ok(new List<Goal> { goal });
            }
        }

        // PUT: api/Goal/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PutGoal(int id, Goal goal)
        {
            if (id != goal.Id)
            {
                return BadRequest();
            }

            await _goalRepository.UpdateGoalAsync(goal);

            return NoContent();
        }

        // DELETE: api/Goal/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            var goal = await _goalRepository.GetGoalAsync(id);
            if (goal == null)
            {
                return NotFound();
            }

            await _goalRepository.DeleteGoalAsync(goal);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Goal>> PostGoal(Goal goal)
        {
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGoal), new { id = goal.Id }, goal);
        }
    }

}