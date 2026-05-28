using Microsoft.AspNetCore.Mvc;
using Recensement.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Recensement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class StatistiquesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StatistiquesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("global")]
        public async Task<ActionResult> GetGlobalStats()
        {
            var citoyens = await _context.Citoyens.ToListAsync();
            
            var stats = new
            {
                TotalPopulation = citoyens.Count,
                Lahy = citoyens.Count(c => c.Sexe == "M"),
                Vavy = citoyens.Count(c => c.Sexe == "F"),
                Mineurs = citoyens.Count(c => CalculateAge(c.DateNaissance) < 18),
                Majeurs = citoyens.Count(c => CalculateAge(c.DateNaissance) >= 18),
                MoyenneAge = citoyens.Any() ? citoyens.Average(c => CalculateAge(c.DateNaissance)) : 0
            };

            return Ok(stats);
        }

        [HttpGet("by-region")]
        public async Task<ActionResult> GetStatsByRegion()
        {
            var stats = await _context.Citoyens
                .Include(c => c.Menage)
                .GroupBy(c => c.Menage != null ? c.Menage.Region : "Tsy fantatra")
                .Select(g => new {
                    Region = g.Key,
                    Total = g.Count(),
                    Lahy = g.Count(c => c.Sexe == "M"),
                    Vavy = g.Count(c => c.Sexe == "F")
                })
                .ToListAsync();

            return Ok(stats);
        }

        private static int CalculateAge(DateTime dob)
        {
            int age = DateTime.Today.Year - dob.Year;
            if (dob.Date > DateTime.Today.AddYears(-age)) age--;
            return age;
        }
    }
}