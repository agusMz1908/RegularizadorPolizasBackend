using RegularizadorPolizas.Domain.Entities;
using System.ComponentModel.DataAnnotations;

public class Company
{
    [Key]
    public int Id { get; set; }
    [StringLength(100)]
    public string Nombre { get; set; }
    [StringLength(50)]
    public string Alias { get; set; }
    [StringLength(20)]
    public string Codigo { get; set; }
    public bool Activo { get; set; } = true;

    public virtual ICollection<Poliza> Polizas { get; set; }
}