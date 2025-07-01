using RegularizadorPolizas.Domain.Entities;
using System.ComponentModel.DataAnnotations;

public class Currency
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Moneda { get; set; }
    public string Codigo { get; set; }    // Para APIs
    public string Simbolo { get; set; }   // Para UI  
    public bool Activo { get; set; } = true;
    public decimal? TipoCambio { get; set; }
    public bool EsMonedaBase { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime FechaModificacion { get; set; } = DateTime.Now;
    public virtual ICollection<Poliza> Polizas { get; set; } = new List<Poliza>();
}