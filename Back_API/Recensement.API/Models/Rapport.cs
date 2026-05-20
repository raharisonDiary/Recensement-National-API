using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recensement.API.Models
{
    public class Rapport 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RegionalId { get; set; }

        // Fampifandraisana amin'ny mpitantana nanao ny rapport
        [ForeignKey("RegionalId")]
        public User? Regional { get; set; }

        [Required]
        public string Contenu { get; set; } = string.Empty;

        public DateTime DateEnvoi { get; set; } = DateTime.Now;

        public bool IsValidated { get; set; } = false;
    }
}