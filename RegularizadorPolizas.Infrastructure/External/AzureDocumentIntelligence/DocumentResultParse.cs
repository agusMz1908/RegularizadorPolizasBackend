using Newtonsoft.Json;
using RegularizadorPolizas.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence
{
    public class DocumentResultParser
    {
        public PolizaDto ParseToPolizaDto(DocumentResultDto documentResult)
        {
            if (documentResult == null || documentResult.CamposExtraidos == null || !documentResult.CamposExtraidos.Any())
            {
                return null;
            }

            var poliza = new PolizaDto();
            var camposOrganizados = OrganizarCampos(documentResult.CamposExtraidos);

            MapearDatosPoliza(poliza, camposOrganizados);
            MapearDatosAsegurado(poliza, camposOrganizados);
            MapearDatosVehiculo(poliza, camposOrganizados);
            MapearDatosFinancieros(poliza, camposOrganizados);
            MapearDatosCorredor(poliza, camposOrganizados);

            return poliza;
        }

        private Dictionary<string, Dictionary<string, string>> OrganizarCampos(Dictionary<string, string> camposPlanos)
        {
            var resultado = new Dictionary<string, Dictionary<string, string>>
            {
                ["poliza"] = new Dictionary<string, string>(),
                ["asegurado"] = new Dictionary<string, string>(),
                ["vehiculo"] = new Dictionary<string, string>(),
                ["financiero"] = new Dictionary<string, string>(),
                ["corredor"] = new Dictionary<string, string>(),
                ["otros"] = new Dictionary<string, string>()
            };

            foreach (var campo in camposPlanos)
            {
                var clave = campo.Key.ToLower();
                var valor = campo.Value;

                if (clave.Contains("poli") || clave.Contains("endoso") || clave.Contains("vigen") ||
                    clave.Contains("emision") || clave.Contains("movimiento"))
                {
                    resultado["poliza"][clave] = valor;
                }
                else if (clave.Contains("asegurado") || clave.Contains("cliente") || clave.Contains("domicilio") ||
                         clave.Contains("direccion") || clave.Contains("rut") || clave.Contains("documento") ||
                         clave.Contains("localidad") || clave.Contains("departamento"))
                {
                    resultado["asegurado"][clave] = valor;
                }
                else if (clave.Contains("vehi") || clave.Contains("marca") || clave.Contains("modelo") ||
                         clave.Contains("matricula") || clave.Contains("motor") || clave.Contains("chasis") ||
                         clave.Contains("padron") || clave.Contains("año") || clave.Contains("tipo_vehiculo") ||
                         clave.Contains("descripcion_vehiculo"))
                {
                    resultado["vehiculo"][clave] = valor;
                }
                else if (clave.Contains("prima") || clave.Contains("premio") || clave.Contains("valor") ||
                         clave.Contains("impuesto") || clave.Contains("iva") || clave.Contains("pago") ||
                         clave.Contains("cuota") || clave.Contains("monto"))
                {
                    resultado["financiero"][clave] = valor;
                }
                else if (clave.Contains("corredor") || clave.Contains("broker") || clave.Contains("productor"))
                {
                    resultado["corredor"][clave] = valor;
                }
                else
                {
                    resultado["otros"][clave] = valor;
                }
            }

            return resultado;
        }

        private void MapearDatosPoliza(PolizaDto poliza, Dictionary<string, Dictionary<string, string>> campos)
        {
            var datosPoliza = campos["poliza"];

            ExtraerCampo(datosPoliza, new[] { "numero_poliza", "poliza", "nro_poliza", "nro. póliza", "nº de póliza" },
                         valor => poliza.Conpol = valor);

            ExtraerCampo(datosPoliza, new[] { "endoso", "nro_endoso", "numero_endoso" },
                         valor => poliza.Conend = valor);

            ExtraerFecha(datosPoliza, new[] { "fecha_vigencia_desde", "vigencia_desde", "desde", "confchdes" },
                         fecha => poliza.Confchdes = fecha);

            ExtraerFecha(datosPoliza, new[] { "fecha_vigencia_hasta", "vigencia_hasta", "hasta", "confchhas" },
                         fecha => poliza.Confchhas = fecha);

            ExtraerFecha(datosPoliza, new[] { "fecha_emision", "emision" },
                         fecha => poliza.FechaCreacion = fecha);

            ExtraerCampo(datosPoliza, new[] { "tipo_movimiento", "movimiento" },
                         valor => poliza.Congesti = valor);

            ExtraerCampo(datosPoliza, new[] { "moneda", "moncod" },
                         valor => {
                             if (valor.ToLower().Contains("peso") || valor.ToLower().Contains("uru"))
                                 poliza.Moncod = 1; 
                             else if (valor.ToLower().Contains("dol"))
                                 poliza.Moncod = 2;
                         });

            ExtraerCampo(datosPoliza, new[] { "ramo" },
                         valor => poliza.Ramo = valor);
        }

        private void MapearDatosAsegurado(PolizaDto poliza, Dictionary<string, Dictionary<string, string>> campos)
        {
            var datosAsegurado = campos["asegurado"];

            ExtraerCampo(datosAsegurado, new[] { "nombre_asegurado", "asegurado", "nombre", "clinom" },
                         valor => poliza.Clinom = valor);

            ExtraerCampo(datosAsegurado, new[] { "documento", "rut", "cliruc", "rut_ci" },
                         valor => poliza.Cliruc = valor);

            ExtraerCampo(datosAsegurado, new[] { "direccion", "domicilio", "condom", "clidir" },
                         valor => poliza.Condom = valor);

            ExtraerCampo(datosAsegurado, new[] { "localidad", "clilocnom" },
                         valor => poliza.Clilocnom = valor);

            ExtraerCampo(datosAsegurado, new[] { "departamento", "clidptnom" },
                         valor => poliza.Clidptnom = valor);

            ExtraerCampo(datosAsegurado, new[] { "telefono", "tel", "clitelcel" },
                         valor => poliza.Clitelcel = valor);

            ExtraerCampo(datosAsegurado, new[] { "email", "correo", "cliemail" },
                         valor => poliza.Cliemail = valor);
        }

        private void MapearDatosVehiculo(PolizaDto poliza, Dictionary<string, Dictionary<string, string>> campos)
        {
            var datosVehiculo = campos["vehiculo"];

            ExtraerCampo(datosVehiculo, new[] { "marca", "conmaraut" },
                         valor => poliza.Conmaraut = valor);

            ExtraerCampo(datosVehiculo, new[] { "modelo" },
                         valor => poliza.Condetail = valor);

            ExtraerCampo(datosVehiculo, new[] { "tipo_vehiculo" },
                         valor => poliza.Contpocob = valor);

            ExtraerCampo(datosVehiculo, new[] { "descripcion_vehiculo" },
                         valor => poliza.Condecram = valor);

            ExtraerCampo(datosVehiculo, new[] { "matricula", "patente", "conmataut" },
                         valor => poliza.Conmataut = valor);

            ExtraerCampo(datosVehiculo, new[] { "motor", "nro_motor", "conmotor" },
                         valor => poliza.Conmotor = valor);

            ExtraerCampo(datosVehiculo, new[] { "chasis", "nro_chasis", "conchasis" },
                         valor => poliza.Conchasis = valor);

            ExtraerCampo(datosVehiculo, new[] { "padron", "nro_padron", "conpadaut" },
                         valor => poliza.Conpadaut = valor);

            ExtraerCampo(datosVehiculo, new[] { "año", "anio", "conanioaut" },
                         valor => {
                             if (int.TryParse(valor, out int anio))
                                 poliza.Conanioaut = anio;
                         });

            ExtraerCampo(datosVehiculo, new[] { "combustible" },
                         valor => {
                             if (valor.ToLower().Contains("nafta") || valor.ToLower().Contains("gasolina"))
                                 poliza.Combustibles = "NAF";
                             else if (valor.ToLower().Contains("diesel") || valor.ToLower().Contains("gasoil"))
                                 poliza.Combustibles = "DIE";
                         });
        }

        private void MapearDatosFinancieros(PolizaDto poliza, Dictionary<string, Dictionary<string, string>> campos)
        {
            var datosFinancieros = campos["financiero"];

            ExtraerDecimal(datosFinancieros, new[] { "prima_comercial", "prima", "conpremio" },
                          valor => poliza.Conpremio = valor);

            ExtraerDecimal(datosFinancieros, new[] { "premio_total", "premio", "contot" },
                          valor => poliza.Contot = valor);

            ExtraerDecimal(datosFinancieros, new[] { "impuesto", "iva", "impuesto_total" },
                          valor => poliza.Conimp = valor);

            ExtraerCampo(datosFinancieros, new[] { "forma_pago", "medio_pago", "consta" },
                         valor => poliza.Consta = valor);

            ExtraerCampo(datosFinancieros, new[] { "cuotas", "cantidad_cuotas", "concuo" },
                         valor => {
                             if (int.TryParse(valor, out int cuotas))
                                 poliza.Concuo = cuotas;
                         });
        }

        private void MapearDatosCorredor(PolizaDto poliza, Dictionary<string, Dictionary<string, string>> campos)
        {
            var datosCorredor = campos["corredor"];

            ExtraerCampo(datosCorredor, new[] { "nombre_corredor", "corredor", "broker", "corrnom" },
                         valor => {
                             // Guarda el nombre del corredor como texto
                             // Si tienes una tabla de corredores, podrías hacer una búsqueda por nombre
                             // y asignar el ID correspondiente
                             poliza.Com_alias = valor;
                         });

            ExtraerCampo(datosCorredor, new[] { "codigo_corredor", "nro_corredor" },
                         valor => {
                             if (int.TryParse(valor, out int corrId))
                                 poliza.Corrnom = corrId;
                         });
        }

        #region Métodos Helpers

        private void ExtraerCampo(Dictionary<string, string> datos, string[] posiblesClaves, Action<string> asignar)
        {
            foreach (var clave in posiblesClaves)
            {
                if (datos.TryGetValue(clave, out string valor) ||
                    datos.TryGetValue(clave.ToLower(), out valor))
                {
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        asignar(valor.Trim());
                        return;
                    }
                }
            }
        }

        private void ExtraerFecha(Dictionary<string, string> datos, string[] posiblesClaves, Action<DateTime> asignar)
        {
            ExtraerCampo(datos, posiblesClaves, valor => {
                if (DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                    asignar(fecha);
                else if (DateTime.TryParse(valor, new CultureInfo("es-ES"), DateTimeStyles.None, out fecha))
                    asignar(fecha);
            });
        }

        private void ExtraerDecimal(Dictionary<string, string> datos, string[] posiblesClaves, Action<decimal> asignar)
        {
            ExtraerCampo(datos, posiblesClaves, valor => {
                string normalizado = valor.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                                         .Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                                         .Replace("$", "").Replace("€", "").Trim();

                if (decimal.TryParse(normalizado, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal resultado))
                    asignar(resultado);
            });
        }

        #endregion
    }
}