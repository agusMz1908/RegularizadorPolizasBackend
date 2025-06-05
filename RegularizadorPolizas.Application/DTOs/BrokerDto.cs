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

    public class VelneoBrokerResponse
    {
        public int Count { get; set; }
        public List<VelneoBrokerDto> Brokers { get; set; } = new();
    }

    public class VelneoBrokerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
    }
    public class VelneoCompanyResponse
    {
        public int Count { get; set; }
        public List<VelneoCompanyDto> Companies { get; set; } = new();
    }

    public class VelneoCompanyDto
    {
        public int Id { get; set; }
        public string Comnom { get; set; } = string.Empty;
        public string Comrazsoc { get; set; } = string.Empty;
        public string Comruc { get; set; } = string.Empty;
        public string Comdom { get; set; } = string.Empty;
        public string Comtel { get; set; } = string.Empty;
        public string Comfax { get; set; } = string.Empty;
        public string Comsumodia { get; set; } = string.Empty;
        public int Comcntcli { get; set; }
        public int Comcntcon { get; set; }
        public decimal Comprepes { get; set; }
        public decimal Compredol { get; set; }
        public decimal Comcomipe { get; set; }
        public decimal Comcomido { get; set; }
        public decimal Comtotcomi { get; set; }
        public decimal Comtotpre { get; set; }
        public string Comalias { get; set; } = string.Empty;
        public string Comlog { get; set; } = string.Empty;
        public bool Broker { get; set; }
        public string Cod_srvcompanias { get; set; } = string.Empty;
        public string No_utiles { get; set; } = string.Empty;
        public int Paq_dias { get; set; }
    }
}