using Microsoft.AspNetCore.Mvc;
using Recensement.API.Data;
using Recensement.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Recensement.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RapportsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RapportsController(AppDbContext context) => _context = context;

        [HttpPost]
        [Authorize(Roles = "Regional")] // Ny Régional ihany no afaka mandefa rapport
        public async Task<ActionResult<Rapport>> SendRapport(Rapport rapport)
        {
            rapport.DateEnvoi = DateTime.Now; // Manampy date automatique
            _context.Rapports.Add(rapport);
            await _context.SaveChangesAsync();
            return Ok(rapport);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rapport>>> GetRapports() 
        {
            return await _context.Rapports.ToListAsync();
        }

        // Fanampiny: Validation avy amin'ny Admin (CdC 3.1)
        [HttpPut("{id}/validate")]
        [Authorize(Roles = "Admin")] // Admin ihany no afaka manao validation
        public async Task<IActionResult> ValidateRapport(int id)
        {
            var rapport = await _context.Rapports.FindAsync(id);
            if (rapport == null) return NotFound();

            rapport.IsValidated = true; // Ataovy azo antoka fa misy field 'IsValidated' ao amin'ny model Rapport
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rapport validé avec succès." });
        }
    }
}