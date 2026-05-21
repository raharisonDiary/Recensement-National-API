using System.ComponentModel.DataAnnotations;

namespace Recensement.API.Models
{
    public class User 
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Cin { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty; // Admin, Regional, Agent

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(100)]
        public string? QrCodeSecret { get; set; } // Ho an'ny Login Agent

        [StringLength(100)]
        public string? RegionAssigned { get; set; }
    }
}