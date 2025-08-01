﻿using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class FormaPago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        public bool RequiereAutorizacion { get; set; }
        public bool EsContado { get; set; }
        public int MaximoCuotas { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
    }
}