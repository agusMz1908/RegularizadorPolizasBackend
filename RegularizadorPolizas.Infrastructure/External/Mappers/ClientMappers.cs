// ClientMappers.cs - CORREGIDO: Eliminadas propiedades inexistentes

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
                Corrcod = velneoCliente.Corrcod,
                Subcorr = velneoCliente.Subcorr,
                Clinom = velneoCliente.Clinom,
                Telefono = velneoCliente.Telefono,
                Clitelcel = velneoCliente.Clitelcel,
                Clifchnac = velneoCliente.Clifchnac,
                Clifching = velneoCliente.Clifching,
                Clifchegr = velneoCliente.Clifchegr,
                Clicargo = velneoCliente.Clicargo,
                Clicon = velneoCliente.Clicon,
                Cliruc = velneoCliente.Cliruc,
                Clirsoc = velneoCliente.Clirsoc,
                Cliced = velneoCliente.Cliced,
                Clilib = velneoCliente.Clilib,
                Clicatlib = velneoCliente.Clicatlib,
                Clitpo = velneoCliente.Clitpo,
                Clidir = velneoCliente.Clidir,
                Cliemail = velneoCliente.Cliemail,
                Clivtoced = velneoCliente.Clivtoced,
                Clivtolib = velneoCliente.Clivtolib,
                Cliposcod = velneoCliente.Cliposcod,
                Clitelcorr = velneoCliente.Clitelcorr,
                Clidptnom = velneoCliente.Clidptnom,
                Clisex = velneoCliente.Clisex,
                Clitelant = velneoCliente.Clitelant,
                Cliobse = velneoCliente.Cliobse,
                Clifax = velneoCliente.Clifax,
                Cliclasif = velneoCliente.Cliclasif,
                Clinumrel = velneoCliente.Clinumrel,
                Clicasapt = velneoCliente.Clicasapt,
                Clidircob = velneoCliente.Clidircob,
                Clibse = velneoCliente.Clibse,
                Clifoto = velneoCliente.Clifoto,
                Pruebamillares = velneoCliente.Pruebamillares,
                Ingresado = velneoCliente.Ingresado,
                Clialias = velneoCliente.Clialias,
                Clipor = velneoCliente.Clipor,
                Clisancor = velneoCliente.Clisancor,
                Clirsa = velneoCliente.Clirsa,
                Codposcob = velneoCliente.Codposcob,
                Clidptcob = velneoCliente.Clidptcob,
                Activo = velneoCliente.Activo,
                Cli_s_cris = velneoCliente.Cli_s_cris,
                Clifchnac1 = velneoCliente.Clifchnac1,
                Clilocnom = velneoCliente.Clilocnom,
                Cliloccob = velneoCliente.Cliloccob,
                Categorias_de_cliente = velneoCliente.Categorias_de_cliente,
                Sc_departamentos = velneoCliente.Sc_departamentos,
                Sc_localidades = velneoCliente.Sc_localidades,
                Fch_ingreso = velneoCliente.Fch_ingreso,
                Grupos_economicos = velneoCliente.Grupos_economicos,
                Etiquetas = velneoCliente.Etiquetas,
                Doc_digi = velneoCliente.Doc_digi,
                Password = velneoCliente.Password,
                Habilita_app = velneoCliente.Habilita_app,
                Referido = velneoCliente.Referido,
                Altura = velneoCliente.Altura,
                Peso = velneoCliente.Peso,
                Cliberkley = velneoCliente.Cliberkley,
                Clifar = velneoCliente.Clifar,
                Clisurco = velneoCliente.Clisurco,
                Clihdi = velneoCliente.Clihdi,
                Climapfre = velneoCliente.Climapfre,
                Climetlife = velneoCliente.Climetlife,
                Clisancris = velneoCliente.Clisancris,
                Clisbi = velneoCliente.Clisbi,
                Edo_civil = velneoCliente.Edo_civil,
                Not_bien_mail = velneoCliente.Not_bien_mail,
                Not_bien_wap = velneoCliente.Not_bien_wap,
                Ing_poliza_mail = velneoCliente.Ing_poliza_mail,
                Ing_poliza_wap = velneoCliente.Ing_poliza_wap,
                Ing_siniestro_mail = velneoCliente.Ing_siniestro_mail,
                Ing_siniestro_wap = velneoCliente.Ing_siniestro_wap,
                Noti_obs_sini_mail = velneoCliente.Noti_obs_sini_mail,
                Noti_obs_sini_wap = velneoCliente.Noti_obs_sini_wap,
                Last_update = velneoCliente.Last_update,
                App_id = velneoCliente.App_id,

                Clinro = velneoCliente.Id,

                Polizas = new List<PolizaResumidaDto>()
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
                Corrcod = clienteDto.Corrcod,
                Subcorr = clienteDto.Subcorr,
                Clinom = clienteDto.Clinom,
                Telefono = clienteDto.Telefono,
                Clitelcel = clienteDto.Clitelcel,
                Clifchnac = clienteDto.Clifchnac,
                Clifching = clienteDto.Clifching,
                Clifchegr = clienteDto.Clifchegr,
                Clicargo = clienteDto.Clicargo,
                Clicon = clienteDto.Clicon,
                Cliruc = clienteDto.Cliruc,
                Clirsoc = clienteDto.Clirsoc,
                Cliced = clienteDto.Cliced,
                Clilib = clienteDto.Clilib,
                Clicatlib = clienteDto.Clicatlib,
                Clitpo = clienteDto.Clitpo,
                Clidir = clienteDto.Clidir,
                Cliemail = clienteDto.Cliemail,
                Clivtoced = clienteDto.Clivtoced,
                Clivtolib = clienteDto.Clivtolib,
                Cliposcod = clienteDto.Cliposcod,
                Clitelcorr = clienteDto.Clitelcorr,
                Clidptnom = clienteDto.Clidptnom,
                Clisex = clienteDto.Clisex,
                Clitelant = clienteDto.Clitelant,
                Cliobse = clienteDto.Cliobse,
                Clifax = clienteDto.Clifax,
                Cliclasif = clienteDto.Cliclasif,
                Clinumrel = clienteDto.Clinumrel,
                Clicasapt = clienteDto.Clicasapt,
                Clidircob = clienteDto.Clidircob,
                Clibse = clienteDto.Clibse,
                Clifoto = clienteDto.Clifoto,
                Pruebamillares = clienteDto.Pruebamillares,
                Ingresado = clienteDto.Ingresado,
                Clialias = clienteDto.Clialias,
                Clipor = clienteDto.Clipor,
                Clisancor = clienteDto.Clisancor,
                Clirsa = clienteDto.Clirsa,
                Codposcob = clienteDto.Codposcob,
                Clidptcob = clienteDto.Clidptcob,
                Activo = clienteDto.Activo,
                Cli_s_cris = clienteDto.Cli_s_cris,
                Clifchnac1 = clienteDto.Clifchnac1,
                Clilocnom = clienteDto.Clilocnom,
                Cliloccob = clienteDto.Cliloccob,
                Categorias_de_cliente = clienteDto.Categorias_de_cliente,
                Sc_departamentos = clienteDto.Sc_departamentos,
                Sc_localidades = clienteDto.Sc_localidades,
                Fch_ingreso = clienteDto.Fch_ingreso,
                Grupos_economicos = clienteDto.Grupos_economicos,
                Etiquetas = clienteDto.Etiquetas,
                Doc_digi = clienteDto.Doc_digi,
                Password = clienteDto.Password,
                Habilita_app = clienteDto.Habilita_app,
                Referido = clienteDto.Referido,
                Altura = clienteDto.Altura,
                Peso = clienteDto.Peso,
                Cliberkley = clienteDto.Cliberkley,
                Clifar = clienteDto.Clifar,
                Clisurco = clienteDto.Clisurco,
                Clihdi = clienteDto.Clihdi,
                Climapfre = clienteDto.Climapfre,
                Climetlife = clienteDto.Climetlife,
                Clisancris = clienteDto.Clisancris,
                Clisbi = clienteDto.Clisbi,
                Edo_civil = clienteDto.Edo_civil,
                Not_bien_mail = clienteDto.Not_bien_mail,
                Not_bien_wap = clienteDto.Not_bien_wap,
                Ing_poliza_mail = clienteDto.Ing_poliza_mail,
                Ing_poliza_wap = clienteDto.Ing_poliza_wap,
                Ing_siniestro_mail = clienteDto.Ing_siniestro_mail,
                Ing_siniestro_wap = clienteDto.Ing_siniestro_wap,
                Noti_obs_sini_mail = clienteDto.Noti_obs_sini_mail,
                Noti_obs_sini_wap = clienteDto.Noti_obs_sini_wap,
                Last_update = clienteDto.Last_update,
                App_id = clienteDto.App_id
            };
        }

        public static ClientLookupDto ToClienteLookupDto(this VelneoCliente velneoCliente)
        {
            return new ClientLookupDto
            {
                Id = velneoCliente.Id,
                NombreCompleto = velneoCliente.Clinom,
                Documento = velneoCliente.Cliced,
                TipoDocumento = "CI", 
                Email = velneoCliente.Cliemail,
                Activo = velneoCliente.Activo
            };
        }

        public static IEnumerable<ClientLookupDto> ToClienteLookupDtos(this IEnumerable<VelneoCliente> velneoClientes)
        {
            return velneoClientes.Select(c => c.ToClienteLookupDto());
        }

        private static DateTime? ParseVelneoDate(string? dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;

            if (DateTime.TryParse(dateString, out var date))
                return date;

            return null;
        }
    }
}