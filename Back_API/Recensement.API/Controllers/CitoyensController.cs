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
    public class CitoyensController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CitoyensController(AppDbContext context) => _context = context;

        [HttpPost]
        public async Task<ActionResult<Citoyen>> AddCitoyen(Citoyen citoyen)
        {
            _context.Citoyens.Add(citoyen);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCitoyen), new { id = citoyen.Id }, citoyen);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Citoyen>> GetCitoyen(Guid id) // Novana ho Guid
        {
            // Fampiasana Include mba tsy ho null ny navigation properties raha ilaina
            var citoyen = await _context.Citoyens
                .Include(c => c.Menage) 
                .FirstOrDefaultAsync(c => c.Id == id); // Efa tsy hisy error intsony eto

            if (citoyen == null) return NotFound();
            return Ok(citoyen);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Citoyen>>> Search(string? query)
        {
            var queryable = _context.Citoyens.AsNoTracking();

            if (string.IsNullOrWhiteSpace(query))
            {
                return await queryable.Take(50).ToListAsync();
            }

            return await queryable
                .Where(c => (c.NoCin != null && c.NoCin.Contains(query)) || 
                            (c.Nom != null && c.Nom.Contains(query)))
                .ToListAsync();
        }
    }
}