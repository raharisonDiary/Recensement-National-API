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
    public class MenagesController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public MenagesController(AppDbContext context) 
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Menage>> CreateMenage(Menage menage)
        {
            // 1. Manamarina raha feno ny data (ohatra: AgentId tsy tokony ho null)
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 2. Raha tsy misy ID (GUID) dia amboary
            if (menage.Id == Guid.Empty) menage.Id = Guid.NewGuid();
            
            _context.Menages.Add(menage);
            await _context.SaveChangesAsync();

            // 3. Mamerina ny Menage misy ny ID vaovao
            return CreatedAtAction(nameof(GetMenage), new { id = menage.Id }, menage);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Menage>>> GetMenages() 
        {
            return await _context.Menages.Include(m => m.Citoyens).ToListAsync();
        }

        [HttpGet("by-region/{region}")]
        public async Task<ActionResult<IEnumerable<Menage>>> GetMenagesByRegion(string region)
        {
            return await _context.Menages
                .Where(m => m.Region == region)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Menage>> GetMenage(Guid id)
        {
            var menage = await _context.Menages
                .Include(m => m.Citoyens) // Ampiana Include raha te-hahita ny Citoyens ao anatiny
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (menage == null) return NotFound();
            
            return Ok(menage);
        }
    }
}