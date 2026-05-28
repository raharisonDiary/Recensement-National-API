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
        [Authorize(Roles = "Regional")]
        public async Task<ActionResult<Rapport>> SendRapport(Rapport rapport)
        {
            rapport.Id = Guid.NewGuid();
            rapport.DateEnvoi = DateTime.Now;
            _context.Rapports.Add(rapport);
            await _context.SaveChangesAsync();
            return Ok(rapport);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rapport>>> GetRapports() 
        {
            // Mampiseho ny rapport miaraka amin'ny anaran'ny Regional nanao azy
            return await _context.Rapports
                .Include(r => r.Regional)
                .ThenInclude(u => u.Profile)
                .OrderByDescending(r => r.DateEnvoi)
                .ToListAsync();
        }

        [HttpPut("{id}/validate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ValidateRapport(Guid id)
        {
            var rapport = await _context.Rapports.FindAsync(id);
            if (rapport == null) return NotFound();

            rapport.IsValidated = true; 
            await _context.SaveChangesAsync();
            return Ok(new { message = "Rapport validé." });
        }

        [HttpPut("{id}/reply")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReplyRapport(Guid id, [FromBody] string reponse)
        {
            var rapport = await _context.Rapports.FindAsync(id);
            if (rapport == null) return NotFound();

            rapport.ReponseAdmin = reponse;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Valin-teny nalefa soa aman-tsara." });
        }
    }
}