using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers
{
    public static class BrokerMappers
    {
        public static BrokerDto ToBrokerDto(this VelneoBroker velneoBroker)
        {
            return new BrokerDto
            {
                Id = velneoBroker.Id,
                Nombre = velneoBroker.Nombre,
                Codigo = velneoBroker.Codigo,
                Descripcion = velneoBroker.Descripcion,
                Activo = velneoBroker.Activo,
                FechaCreacion = velneoBroker.FechaCreacion,
                FechaModificacion = velneoBroker.FechaModificacion,
                Direccion = velneoBroker.Direccion,
                Telefono = velneoBroker.Telefono,
                Email = velneoBroker.Email,
                Website = velneoBroker.Website,
                Ruc = velneoBroker.Ruc,
                ContactoPrincipal = velneoBroker.ContactoPrincipal,
                TelefonoContacto = velneoBroker.TelefonoContacto,
                EmailContacto = velneoBroker.EmailContacto,
                PorcentajeComision = velneoBroker.PorcentajeComision,
                TipoComision = velneoBroker.TipoComision,
                LimiteCredito = velneoBroker.LimiteCredito,
                DiasCredito = velneoBroker.DiasCredito
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
                Nombre = brokerDto.Nombre,
                Codigo = brokerDto.Codigo,
                Descripcion = brokerDto.Descripcion,
                Activo = brokerDto.Activo,
                FechaCreacion = brokerDto.FechaCreacion,
                FechaModificacion = brokerDto.FechaModificacion,
                Direccion = brokerDto.Direccion,
                Telefono = brokerDto.Telefono,
                Email = brokerDto.Email,
                Website = brokerDto.Website,
                Ruc = brokerDto.Ruc,
                ContactoPrincipal = brokerDto.ContactoPrincipal,
                TelefonoContacto = brokerDto.TelefonoContacto,
                EmailContacto = brokerDto.EmailContacto,
                PorcentajeComision = brokerDto.PorcentajeComision,
                TipoComision = brokerDto.TipoComision,
                LimiteCredito = brokerDto.LimiteCredito,
                DiasCredito = brokerDto.DiasCredito
            };
        }

        public static IEnumerable<VelneoBroker> ToVelneoBrokerDtos(this IEnumerable<BrokerDto> brokerDtos)
        {
            return brokerDtos.Select(b => b.ToVelneoBrokerDto());
        }

        // Mapper específico para lookups/selects
        public static BrokerLookupDto ToBrokerLookupDto(this VelneoBroker velneoBroker)
        {
            return new BrokerLookupDto
            {
                Id = velneoBroker.Id,
                Nombre = velneoBroker.Nombre,
                Codigo = velneoBroker.Codigo,
                Activo = velneoBroker.Activo,
                PorcentajeComision = velneoBroker.PorcentajeComision
            };
        }

        public static IEnumerable<BrokerLookupDto> ToBrokerLookupDtos(this IEnumerable<VelneoBroker> velneoBrokers)
        {
            return velneoBrokers.Select(b => b.ToBrokerLookupDto());
        }

        // Mapper desde VelneoBrokerLookup (si Velneo tiene endpoints específicos para lookups)
        public static BrokerLookupDto ToBrokerLookupDto(this VelneoBrokerLookup velneoBrokerLookup)
        {
            return new BrokerLookupDto
            {
                Id = velneoBrokerLookup.Id,
                Nombre = velneoBrokerLookup.Nombre,
                Codigo = velneoBrokerLookup.Codigo,
                Activo = velneoBrokerLookup.Activo,
                PorcentajeComision = velneoBrokerLookup.PorcentajeComision
            };
        }

        public static IEnumerable<BrokerLookupDto> ToBrokerLookupDtos(this IEnumerable<VelneoBrokerLookup> velneoBrokerLookups)
        {
            return velneoBrokerLookups.Select(b => b.ToBrokerLookupDto());
        }
    }
}