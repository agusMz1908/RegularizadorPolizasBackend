using RegularizadorPolizas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [StringLength(100)]
        public required string Email { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Relaciones
        public virtual ICollection<ProcessDocument> ProcessDocuments { get; set; }
        public virtual ICollection<Renovation> Renovations { get; set; }
    }
}