using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Application.DTOs
{
    public class PolizaCreateRequest
    {
        // CAMPOS BÁSICOS EXISTENTES
        public int Comcod { get; set; }
        public int Clinro { get; set; }
        public string Conpol { get; set; }
        public string Confchdes { get; set; }
        public string Confchhas { get; set; }
        public decimal Conpremio { get; set; }
        public string Asegurado { get; set; }
        public string Observaciones { get; set; }
        public string Moneda { get; set; }

        // CAMPOS DEL VEHÍCULO (EXISTENTES)
        public string? Vehiculo { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Motor { get; set; }
        public string? Chasis { get; set; }
        public string? Matricula { get; set; }
        public string? Combustible { get; set; }
        public int? Anio { get; set; }

        // CAMPOS COMERCIALES (EXISTENTES)
        public decimal? PrimaComercial { get; set; }
        public decimal? PremioTotal { get; set; }
        public string? Corredor { get; set; }
        public string? Plan { get; set; }
        public string? Ramo { get; set; }

        // CAMPOS DEL CLIENTE (EXISTENTES)
        public string? Documento { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Localidad { get; set; }
        public string? Departamento { get; set; }

        public bool ProcesadoConIA { get; set; }

        // ✅ NUEVOS CAMPOS FALTANTES DEL WIZARD
        public int? SeccionId { get; set; }  // ← seccod
        public string? Estado { get; set; }
        public string? Tramite { get; set; }
        public string? EstadoPoliza { get; set; }
        public int? CalidadId { get; set; }   // ← caldsc
        public int? DestinoId { get; set; }   // ← desdsc
        public int? CategoriaId { get; set; } // ← catdsc
        public string? TipoVehiculo { get; set; }
        public string? Uso { get; set; }
        public string? FormaPago { get; set; }
        public int? CantidadCuotas { get; set; }
        public decimal? ValorCuota { get; set; }
        public string? Tipo { get; set; } // PERSONA/EMPRESA
        public string? Cobertura { get; set; }
        public string? Certificado { get; set; }
    }
}