using System.ComponentModel.DataAnnotations;

namespace Recensement.API.Models
{
    public class User 
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Cin { get; set; } = string.Empty; // Ity no ampiasaina login

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty; // Admin, Regional, Agent

        public string? QrCodeSecret { get; set; } // Ho an'ny Agent
        
        // Relationship
        public AgentProfile? Profile { get; set; }
    }
}