using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Vehiculo
    {
        [Key]
        public int Id { get; set; }

        [StringLength(20)]
        public string Conmaraut { get; set; } = string.Empty; // Marca

        public int? Conanioaut { get; set; } // Año

        [StringLength(30)]
        public string Conmataut { get; set; } = string.Empty; // Matrícula

        [StringLength(30)]
        public string Conmotor { get; set; } = string.Empty; // Número de motor

        [StringLength(30)]
        public string Conpadaut { get; set; } = string.Empty; // Padrón

        [StringLength(30)]
        public string Conchasis { get; set; } = string.Empty; // Número de chasis

        public int? Concodrev { get; set; } // Código revista

        public int? Conficto { get; set; } // Código ficto

        public int? Conclaaut { get; set; } // Clase auto

        public int? Condedaut { get; set; } // Deducible auto

        public int? Concaraut { get; set; } // Carga auto

        public int? Concapaut { get; set; } // Capacidad auto

        public int? Conresciv { get; set; } // Responsabilidad civil

        public int? Conbonnsin { get; set; } // Bonificación sin siniestros

        public int? Conbonant { get; set; } // Bonificación antigüedad

        public int? CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; } 
        public int? CalidadId { get; set; }
        [ForeignKey("CalidadId")]
        public virtual Calidad Calidad { get; set; }
        public int? DestinoId { get; set; }
        [ForeignKey("DestinoId")]
        public virtual Destino Destino { get; set; }
        public int? CombustibleId { get; set; }
        [ForeignKey("CombustibleId")]
        public virtual Combustible Combustible { get; set; }

        [StringLength(500)]
        public string Condetail { get; set; } = string.Empty; 

        [StringLength(80)]
        public string Contpocob { get; set; } = string.Empty; 

        [StringLength(80)]
        public string Contipoemp { get; set; } = string.Empty; 

        [StringLength(80)]
        public string Conmatpar { get; set; } = string.Empty;

        [StringLength(80)]
        public string Conmatte { get; set; } = string.Empty; 

        public int? Catdsc { get; set; } 
        public int? Caldsc { get; set; }  
        public int? Desdsc { get; set; } 

        [StringLength(50)]
        public string Combustibles { get; set; } = string.Empty; 

        public bool TieneAlarma { get; set; } = false; 

        public int? TiposDeAlarma { get; set; }

        public bool Granizo { get; set; } = false; 

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        public int? CreadoPor { get; set; }
        public int? ModificadoPor { get; set; }

        public virtual ICollection<Poliza> Contratos { get; set; } = new List<Poliza>();

    }
}