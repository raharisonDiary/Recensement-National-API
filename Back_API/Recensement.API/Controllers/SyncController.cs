using Microsoft.AspNetCore.Mvc;
using Recensement.API.Data;
using Recensement.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Recensement.API.Controllers
{
    [Authorize(Roles = "Agent")]
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SyncController(AppDbContext context) => _context = context;

        [HttpPost("upload")]
        public async Task<IActionResult> SyncData([FromBody] List<Citoyen> citoyens)
        {
            if (citoyens == null || citoyens.Count == 0)
                return BadRequest("Tsy misy data nalefa.");

            int addedCount = 0;
            int updatedCount = 0;

            foreach (var item in citoyens)
            {
                // Jereo raha efa misy ilay olona ao amin'ny database
                var existing = await _context.Citoyens
                    .FirstOrDefaultAsync(c => c.NoCin == item.NoCin);

                if (existing != null)
                {
                    // Raha efa misy dia havaozina (Update)
                    existing.Nom = item.Nom;
                    existing.DateNaissance = item.DateNaissance;
                    existing.Sexe = item.Sexe;
                    existing.EstMarie = item.EstMarie;
                    existing.NbEnfants = item.NbEnfants;
                    existing.PhotoPath = item.PhotoPath;
                    updatedCount++;
                }
                else
                {
                    // Raha mbola vaovao dia ampiana (Insert)
                    await _context.Citoyens.AddAsync(item);
                    addedCount++;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Vita ny synchronisation.", 
                Ajoutés = addedCount, 
                Misà_à_jour = updatedCount 
            });
        }
    }
}