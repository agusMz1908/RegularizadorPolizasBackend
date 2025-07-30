using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Models;

namespace RegularizadorPolizas.Application.Mappers
{
    public static class BrokerMappers
    {
        public static BrokerDto ToBrokerDto(this VelneoBroker velneoBroker)
        {
            return new BrokerDto
            {
                Id = velneoBroker.Id,
                Name = velneoBroker.Nombre, // VelneoBroker.Nombre -> BrokerDto.Name
                Telefono = velneoBroker.Telefono ?? string.Empty,
                Direccion = velneoBroker.Direccion ?? string.Empty,
                Observaciones = velneoBroker.Descripcion ?? string.Empty,
                Foto = string.Empty,
                Codigo = velneoBroker.Codigo ?? string.Empty,
                Email = velneoBroker.Email ?? string.Empty,
                Activo = velneoBroker.Activo,
                TotalPolizas = 0
            };
        }

        public static IEnumerable<BrokerDto> ToBrokerDtos(this IEnumerable<VelneoBroker> velneoBrokers)
        {
            return velneoBrokers.Select(b => b.ToBrokerDto());
        }

        public static VelneoBroker ToVelneoBrokerDto(this BrokerDto brokerDto)
        {
            return new VelneoBroker
            {
                Id = brokerDto.Id,
                Nombre = brokerDto.Name,
                Codigo = brokerDto.Codigo,
                Descripcion = brokerDto.Observaciones,
                Activo = brokerDto.Activo,
                Direccion = brokerDto.Direccion,
                Telefono = brokerDto.Telefono,
                Email = brokerDto.Email,
                Website = string.Empty,
                Ruc = string.Empty,
                ContactoPrincipal = string.Empty,
                TelefonoContacto = string.Empty,
                EmailContacto = string.Empty,
                PorcentajeComision = 0,
                TipoComision = string.Empty,
                LimiteCredito = 0,
                DiasCredito = 0,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            };
        }

        public static BrokerLookupDto ToBrokerLookupDto(this VelneoBroker velneoBroker)
        {
            return new BrokerLookupDto
            {
                Id = velneoBroker.Id,
                Name = velneoBroker.Nombre ?? string.Empty,
                Codigo = velneoBroker.Codigo ?? string.Empty,
                Telefono = velneoBroker.Telefono ?? string.Empty
            };
        }

        public static IEnumerable<BrokerLookupDto> ToBrokerLookupDtos(this IEnumerable<VelneoBroker> velneoBrokers)
        {
            return velneoBrokers.Select(b => b.ToBrokerLookupDto());
        }
    }
}