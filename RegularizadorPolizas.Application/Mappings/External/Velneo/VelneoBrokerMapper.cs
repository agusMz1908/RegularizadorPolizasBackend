using RegularizadorPolizas.Application.DTOs.External.Velneo;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers
{
    public static class VelneoBrokerMapper
    {
        public static BrokerDto ToBrokerDto(this VelneoBrokerDto velneoBroker)
        {
            return new BrokerDto
            {
                Id = velneoBroker.Id,
                Name = velneoBroker.Name,
                Telefono = velneoBroker.Telefono,
                Direccion = velneoBroker.Direccion,
                Observaciones = velneoBroker.Observaciones,
                Foto = velneoBroker.Foto,
                Codigo = velneoBroker.Codigo,
                Email = velneoBroker.Email,
                Activo = velneoBroker.Activo,
                TotalPolizas = 0
            };
        }

        public static IEnumerable<BrokerDto> ToBrokerDtos(this IEnumerable<VelneoBrokerDto> velneoBrokers)
        {
            return velneoBrokers.Select(vb => vb.ToBrokerDto());
        }

        public static VelneoBrokerDto ToVelneoBrokerDto(this BrokerDto brokerDto)
        {
            return new VelneoBrokerDto
            {
                Id = brokerDto.Id,
                Name = brokerDto.Name,
                Telefono = brokerDto.Telefono,
                Direccion = brokerDto.Direccion,
                Observaciones = brokerDto.Observaciones,
                Foto = brokerDto.Foto,
                Codigo = brokerDto.Codigo,
                Email = brokerDto.Email
            };
        }
    }
}