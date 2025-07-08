using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers
{
    public static class ClientMappers
    {
        public static ClientDto ToClienteDto(this VelneoCliente velneoCliente)
        {
            return new ClientDto
            {
                Id = velneoCliente.Id,
                Clinro = velneoCliente.Clinro,
                Clinom = velneoCliente.Clinom,
                Cliape = velneoCliente.Cliape,
                Cliruc = velneoCliente.Cliruc,
                Cliced = velneoCliente.Cliced,
                Clitel = velneoCliente.Clitel,
                Cliemail = velneoCliente.Cliemail,
                Clidir = velneoCliente.Clidir,
                Cliciu = velneoCliente.Cliciu,
                Clidep = velneoCliente.Clidep,
                Clipai = velneoCliente.Clipai,
                Cliobse = velneoCliente.Cliobse,
                Activo = velneoCliente.Activo,
                FechaCreacion = velneoCliente.FechaCreacion,
                FechaModificacion = velneoCliente.FechaModificacion,
                TipoDocumento = velneoCliente.TipoDocumento,
                NombreCompleto = !string.IsNullOrEmpty(velneoCliente.Cliape)
                    ? $"{velneoCliente.Clinom} {velneoCliente.Cliape}".Trim()
                    : velneoCliente.Clinom
            };
        }

        public static IEnumerable<ClientDto> ToClienteDtos(this IEnumerable<VelneoCliente> velneoClientes)
        {
            return velneoClientes.Select(c => c.ToClienteDto());
        }

        public static VelneoCliente ToVelneoClienteDto(this ClientDto clienteDto)
        {
            return new VelneoCliente
            {
                Id = clienteDto.Id,
                Clinro = clienteDto.Clinro,
                Clinom = clienteDto.Clinom,
                Cliape = clienteDto.Cliape,
                Cliruc = clienteDto.Cliruc,
                Cliced = clienteDto.Cliced,
                Clitel = clienteDto.Clitel,
                Cliemail = clienteDto.Cliemail,
                Clidir = clienteDto.Clidir,
                Cliciu = clienteDto.Cliciu,
                Clidep = clienteDto.Clidep,
                Clipai = clienteDto.Clipai,
                Cliobse = clienteDto.Cliobse,
                Activo = clienteDto.Activo,
                FechaCreacion = clienteDto.FechaCreacion,
                FechaModificacion = clienteDto.FechaModificacion,
                TipoDocumento = clienteDto.TipoDocumento
            };
        }

        public static IEnumerable<VelneoCliente> ToVelneoClienteDtos(this IEnumerable<ClientDto> clienteDtos)
        {
            return clienteDtos.Select(c => c.ToVelneoClienteDto());
        }

        // Mapper específico para lookups/búsquedas
        public static ClientLookupDto ToClienteLookupDto(this VelneoCliente velneoCliente)
        {
            return new ClientLookupDto
            {
                Id = velneoCliente.Id,
                NombreCompleto = !string.IsNullOrEmpty(velneoCliente.Cliape)
                    ? $"{velneoCliente.Clinom} {velneoCliente.Cliape}".Trim()
                    : velneoCliente.Clinom,
                Documento = velneoCliente.Cliced,
                TipoDocumento = velneoCliente.TipoDocumento,
                Email = velneoCliente.Cliemail,
                Activo = velneoCliente.Activo
            };
        }

        public static IEnumerable<ClientLookupDto> ToClienteLookupDtos(this IEnumerable<VelneoCliente> velneoClientes)
        {
            return velneoClientes.Select(c => c.ToClienteLookupDto());
        }
    }
}