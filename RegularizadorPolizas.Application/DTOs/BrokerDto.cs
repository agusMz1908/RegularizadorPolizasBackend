public class BrokerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
    public string Foto { get; set; } = string.Empty;

    public string Codigo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public int TotalPolizas { get; set; }
    public bool PuedeEliminar => TotalPolizas == 0;

    public string Nombre => Name;
    public string Domicilio => Direccion;
}

public class BrokerLookupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;

    public string Nombre => Name;
}

public class BrokerCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
    public string Foto { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class BrokerSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public int TotalPolizas { get; set; }
    public string Nombre => Name;
}