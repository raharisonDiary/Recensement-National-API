using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recensement.API.Models
{
    public class Citoyen 
    {
        [Key]
        public Guid Id { get; set; } 

        [Required]
        public Guid MenageId { get; set; }

        [ForeignKey("MenageId")]
        public Menage? Menage { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public DateTime DateNaissance { get; set; }

        [Required]
        [StringLength(10)]
        public string Sexe { get; set; } = string.Empty;

        public bool EstMarie { get; set; }

        public int? NbEnfants { get; set; }

        public string? PhotoPath { get; set; }

        [StringLength(20)]
        public string? NoCin { get; set; }
    }
}