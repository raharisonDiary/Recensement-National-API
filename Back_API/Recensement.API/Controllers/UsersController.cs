using Microsoft.AspNetCore.Mvc;
using Recensement.API.Data;
using Recensement.API.Models;
using Recensement.API.Services;
using Microsoft.EntityFrameworkCore;
using Recensement.API.DTOs;
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

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Cin == loginDto.Cin && u.PasswordHash == loginDto.Password);

            if (user == null) return Unauthorized("Misy diso ny CIN na ny Password.");

            var token = _tokenService.CreateToken(user);
            return Ok(new { user.Id, user.Nom, user.Cin, user.Role, Token = token });
        }

        // --- GESTION DES AGENTS (CdC 3.2) ---
        [HttpPost("create-agent")]
        [Authorize(Roles = "Regional")] 
        public async Task<ActionResult> CreateAgent(User agent)
        {
            agent.QrCodeSecret = Guid.NewGuid().ToString(); 
            agent.Role = "Agent";
            _context.Users.Add(agent);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Agent crée avec succès", agent.QrCodeSecret });
        }

        [HttpPost("agent-login")]
        public async Task<ActionResult> AgentLogin(string qrCodeSecret)
        {
            var agent = await _context.Users
                .FirstOrDefaultAsync(u => u.QrCodeSecret == qrCodeSecret && u.Role == "Agent");

            if (agent == null) return Unauthorized("QR Code tsy manan-kery.");
            var token = _tokenService.CreateToken(agent);
            return Ok(new { agent, token });
        }

        // --- GESTION DES REGIONAUX (CdC 3.1 - Admin National) ---
        [HttpGet("regionaux")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetRegionaux()
        {
            return await _context.Users.Where(u => u.Role == "Regional").ToListAsync();
        }

        [HttpPost("create-regional")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateRegional(User regional)
        {
            regional.Role = "Regional";
            _context.Users.Add(regional);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Responsable Régional voaforona." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Voafafa ny kaonty." });
        }

        [HttpGet("generate-qr/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetQrCode(int id) {
            var agent = _context.Users.Find(id);

            // Eto isika no manao "null check"
            if (agent == null)
            {
                return NotFound("Tsy hita ilay Agent.");
            }

            // Eto izao dia azo antoka fa tsy null ny 'agent'
            // Fa mety mbola null ny QrCodeSecret, koa ampiasao ny ? na check-enao
            if (string.IsNullOrEmpty(agent.QrCodeSecret))
            {
                return BadRequest("Mbola tsy misy QR Code ity Agent ity.");
            }

            var qr = new QrService().GenerateQrCode(agent.QrCodeSecret);
            return Ok(new { QrImageBase64 = qr });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
    }
}