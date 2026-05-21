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

            // 1. Raiso ny ID rehetra nalefan'ny agent
            var ids = citoyens.Select(c => c.Id).ToList();

            // 2. Tadiavo ao amin'ny DB izay mifanaraka amin'ireo ID ireo
            var existingCitoyens = await _context.Citoyens
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            // 3. Ovay ho Dictionary ny lisitra avy ao amin'ny DB mba ho haingana ny fitadiavana
            var existingDict = existingCitoyens.ToDictionary(c => c.Id);

            int addedCount = 0;
            int updatedCount = 0;

            foreach (var item in citoyens)
            {
                if (existingDict.TryGetValue(item.Id, out var existing))
                {
                    // Raha efa misy dia havaozina ny fields rehetra
                    existing.NoCin = item.NoCin; // Mbola azo ovaina ihany ny CIN raha ilaina
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
                    // Raha vaovao dia ampiana
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