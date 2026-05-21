using Microsoft.AspNetCore.Mvc;
using Recensement.API.Data;
using Recensement.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Recensement.API.Controllers
{
    [Authorize] // Voaaro daholo ny endpoints rehetra
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
            _context.Menages.Add(menage);
            await _context.SaveChangesAsync();
            return Ok(menage);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Menage>>> GetMenages() 
        {
            return await _context.Menages.ToListAsync();
        }

        // Fikarohana isam-paritra (CdC 3.2 - Supervision par région)
        [HttpGet("by-region/{region}")]
        public async Task<ActionResult<IEnumerable<Menage>>> GetMenagesByRegion(string region)
        {
            return await _context.Menages
                .Where(m => m.Region == region)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Menage>> GetMenage(Guid id) // Novana ho Guid
        {
            var menage = await _context.Menages.FindAsync(id);
            
            if (menage == null) return NotFound();
            
            return Ok(menage);
        }
    }
}