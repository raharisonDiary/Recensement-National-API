using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recensement.API.Models
{
    public class Menage 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AgentId { get; set; }

        // Fampifandraisana amin'ny tabilao User (Agent)
        [ForeignKey("AgentId")]
        public User? Agent { get; set; }

        [Required]
        [StringLength(100)]
        public string Region { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string District { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Fokontany { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Adresse { get; set; }

        [Required]
        public double GpsLat { get; set; }

        [Required]
        public double GpsLong { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public bool IsSynced { get; set; } = false;

        // Relation: Ménage iray dia manana Citoyens maro
        public ICollection<Citoyen> Citoyens { get; set; } = new List<Citoyen>();
    }
}