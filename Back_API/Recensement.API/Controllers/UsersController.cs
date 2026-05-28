using Microsoft.AspNetCore.Mvc;
using Recensement.API.Data;
using Recensement.API.Models;
using Recensement.API.Services;
using Recensement.API.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Recensement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public UsersController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // --- LOGIN ---
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Cin == loginDto.Cin && u.PasswordHash == loginDto.Password);

            if (user == null) return Unauthorized("Misy diso ny CIN na ny Password.");
            
            var token = _tokenService.CreateToken(user);
            return Ok(new { user.Id, user.Role, Nom = user.Profile?.Nom, Token = token });
        }

        // --- GESTION RÉGIONAUX (Admin ihany) ---
        [HttpGet("regionaux")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetRegionaux()
        {
            // Ampiasao ny .Select() mba hisorohana ny Circular Reference
            var regionaux = await _context.Users
                .Where(u => u.Role == "Regional")
                .Select(u => new {
                    u.Id,
                    u.Cin,
                    u.Role,
                    Nom = u.Profile != null ? u.Profile.Nom : "N/A",
                    Prenom = u.Profile != null ? u.Profile.Prenom : "N/A"
                })
                .ToListAsync();
                
            return Ok(regionaux);
        }

        [HttpPut("update-regional/{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> UpdateRegional(Guid id, UserUpdateDto dto)
{
    var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == id);
    if (user == null) return NotFound();
    
    user.Profile.Nom = dto.Nom;
    // Ampio ny field hafa rehetra...
    await _context.SaveChangesAsync();
    return Ok();
}

        [HttpPost("create-regional")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateRegional(RegionalRegistrationDto dto)
        {
            var user = new User {
                Id = Guid.NewGuid(),
                Cin = dto.Cin,
                PasswordHash = dto.Password,
                Role = "Regional",
                QrCodeSecret = Guid.NewGuid().ToString(),
                Profile = new AgentProfile {
                    Id = Guid.NewGuid(),
                    Nom = dto.Nom,
                    Prenom = dto.Prenom
                }
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Responsable Régional voaforona.", qrCode = user.QrCodeSecret });
        }

        [HttpDelete("delete-regional/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRegional(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role == "Regional");
            if (user == null) return NotFound("Tsy hita ilay Régional.");
            
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Voafafa soa aman-tsara ny Régional." });
        }

        // --- GESTION AGENTS ---
        [HttpGet("agents")]
        [Authorize(Roles = "Regional, Admin")]
        public async Task<ActionResult> GetAgents()
        {
            var agents = await _context.Users
                .Where(u => u.Role == "Agent")
                .Select(u => new {
                    u.Id,
                    u.Cin,
                    Nom = u.Profile != null ? u.Profile.Nom : "N/A",
                    Prenom = u.Profile != null ? u.Profile.Prenom : "N/A",
                    Region = u.Profile != null ? u.Profile.RegionAssigned : "N/A"
                })
                .ToListAsync();
            return Ok(agents);
        }

        [HttpPost("create-agent")]
        [Authorize(Roles = "Regional, Admin")]
        public async Task<ActionResult> CreateAgent(AgentRegistrationDto dto)
        {
            var user = new User {
                Id = Guid.NewGuid(),
                Cin = dto.Cin,
                PasswordHash = "default123",
                Role = "Agent",
                QrCodeSecret = Guid.NewGuid().ToString(),
                Profile = new AgentProfile {
                    Id = Guid.NewGuid(),
                    Nom = dto.Nom,
                    Prenom = dto.Prenom,
                    Telephone = dto.Telephone,
                    Adresse = dto.Adresse,
                    RegionAssigned = dto.RegionAssigned
                }
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Agent voaforona", qrCode = user.QrCodeSecret });
        }
    }
}