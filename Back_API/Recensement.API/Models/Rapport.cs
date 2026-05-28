using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recensement.API.Models
{
    public class Rapport 
    {
        [Key]
        public Guid Id { get; set; } // Novana ho Guid mba hitovy amin'ny modely hafa

        [Required]
        public Guid RegionalId { get; set; }

        [ForeignKey("RegionalId")]
        public User? Regional { get; set; }

        [Required]
        public string Contenu { get; set; } = string.Empty;

        public string? ReponseAdmin { get; set; } // Valin-tenin'ny Admin

        public DateTime DateEnvoi { get; set; } = DateTime.Now;

        public bool IsValidated { get; set; } = false;
    }
}