using Microsoft.AspNetCore.Mvc;
using Recensement.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Recensement.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StatistiquesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public StatistiquesController(AppDbContext context) => _context = context;

        [HttpGet("resume")]
        public async Task<IActionResult> GetStats()
        {
            var totalCitoyens = await _context.Citoyens.CountAsync();
            var totalMenages = await _context.Menages.CountAsync();
            
            // 1. Statistiques par Sexe
            var statsParSexe = await _context.Citoyens
                .GroupBy(c => c.Sexe)
                .Select(g => new { Sexe = g.Key, Count = g.Count() })
                .ToListAsync();

            // 2. Statistiques par Tranche d'Âge (Kajy taona marina kokoa)
            var today = DateTime.Today;
            var citoyenDates = await _context.Citoyens.Select(c => c.DateNaissance).ToListAsync();
            
            var statsAge = new {
                Enfants = citoyenDates.Count(d => d.AddYears(18) > today),
                Adultes = citoyenDates.Count(d => d.AddYears(18) <= today && d.AddYears(60) > today),
                Seniors = citoyenDates.Count(d => d.AddYears(60) <= today)
            };

            // 3. Statistiques par Région
            var statsParRegion = await _context.Menages
                .GroupBy(m => m.Region)
                .Select(g => new { Region = g.Key, TotalMenages = g.Count() })
                .ToListAsync();
            
            return Ok(new { 
                TotalCitoyens = totalCitoyens, 
                TotalMenages = totalMenages,
                StatistiquesParSexe = statsParSexe,
                StatistiquesParAge = statsAge,
                StatistiquesParRegion = statsParRegion
            });
        }
    }
}