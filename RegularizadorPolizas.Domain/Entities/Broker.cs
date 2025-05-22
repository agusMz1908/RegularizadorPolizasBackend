using RegularizadorPolizas.Domain.Entities;
using System.ComponentModel.DataAnnotations;

public class Broker
{
    [Key]
    public int Id { get; set; }
    [StringLength(150)]
    public string Nombre { get; set; }
    [StringLength(50)]
    public string Codigo { get; set; }
    [StringLength(255)]
    public string Domicilio { get; set; }
    [StringLength(50)]
    public string Telefono { get; set; }
    [StringLength(100)]
    public string Email { get; set; }
    public bool Activo { get; set; } = true;

    public virtual ICollection<Poliza> Polizas { get; set; }
}