using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Infrastructure.Data
{
    public static class SeedData
    {
        public static void SeedCompanies(ModelBuilder modelBuilder)
        {
            var now = DateTime.Now;

            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = 1,
                    Comnom = "Banco de Seguros del Estado",
                    Nombre = "Banco de Seguros del Estado", // Campo de compatibilidad
                    Comalias = "BSE",
                    Alias = "BSE", // Campo de compatibilidad
                    Cod_srvcompanias = "BSE",
                    Codigo = "BSE", // Campo de compatibilidad
                    Activo = true,
                    FechaCreacion = now,
                    FechaModificacion = now
                },
                new Company
                {
                    Id = 2,
                    Comnom = "SURA Uruguay",
                    Nombre = "SURA Uruguay",
                    Comalias = "SURA",
                    Alias = "SURA",
                    Cod_srvcompanias = "SURA",
                    Codigo = "SURA",
                    Activo = true,
                    FechaCreacion = now,
                    FechaModificacion = now
                },
                new Company
                {
                    Id = 3,
                    Comnom = "Mapfre Uruguay",
                    Nombre = "Mapfre Uruguay",
                    Comalias = "MAPFRE",
                    Alias = "MAPFRE",
                    Cod_srvcompanias = "MAPFRE",
                    Codigo = "MAPFRE",
                    Activo = true,
                    FechaCreacion = now,
                    FechaModificacion = now
                },
                new Company
                {
                    Id = 4,
                    Comnom = "San Cristóbal",
                    Nombre = "San Cristóbal",
                    Comalias = "SAN CRISTOBAL",
                    Alias = "SAN CRISTOBAL",
                    Cod_srvcompanias = "SC",
                    Codigo = "SC",
                    Activo = true,
                    FechaCreacion = now,
                    FechaModificacion = now
                }
            );
        }

        public static void SeedCurrencies(ModelBuilder modelBuilder)
        {
            var now = DateTime.Now;

            modelBuilder.Entity<Currency>().HasData(
                new Currency
                {
                    Id = 1,
                    Nombre = "Peso Uruguayo",
                    Moneda = "UYU", 
                    Codigo = "UYU", 
                    Simbolo = "$",
                    Activo = true,
                    FechaCreacion = now, 
                    FechaModificacion = now 
                },
                new Currency
                {
                    Id = 2,
                    Nombre = "Dólar Americano",
                    Moneda = "USD",
                    Codigo = "USD",
                    Simbolo = "US$",
                    Activo = true,
                    FechaCreacion = now, 
                    FechaModificacion = now 
                },
                new Currency
                {
                    Id = 3,
                    Nombre = "Unidad Indexada",
                    Moneda = "UI",
                    Codigo = "UI",
                    Simbolo = "UI",
                    Activo = true,
                    FechaCreacion = now,
                    FechaModificacion = now 
                }
            );
        }

        public static void SeedUsers(ModelBuilder modelBuilder)
        {
            var creationDate = new DateTime(2025, 5, 26, 16, 8, 52, 542, DateTimeKind.Local).AddTicks(5783);
            var modificationDate = new DateTime(2025, 5, 26, 16, 8, 52, 542, DateTimeKind.Local).AddTicks(6023);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Nombre = "Administrador",
                    Email = "admin@sistema.com",
                    Activo = true,
                    FechaCreacion = creationDate,
                    FechaModificacion = modificationDate
                }
            );
        }

        public static void SeedBrokers(ModelBuilder modelBuilder)
        {
            var now = DateTime.Now;

            modelBuilder.Entity<Broker>().HasData(
                new Broker
                {
                    Id = 1,
                    Name = "Corredor Principal",
                    Nombre = "Corredor Principal", // Campo de compatibilidad
                    Codigo = "CORR001",
                    Direccion = "18 de Julio 1234",
                    Domicilio = "18 de Julio 1234", // Campo de compatibilidad
                    Telefono = "099123456",
                    Email = "corredor@principal.com",
                    Activo = true,
                    FechaCreacion = now,
                    FechaModificacion = now
                }
            );
        }

        public static void ApplyAllSeedData(ModelBuilder modelBuilder)
        {
            SeedCompanies(modelBuilder);
            SeedCurrencies(modelBuilder);
            SeedUsers(modelBuilder);
            SeedBrokers(modelBuilder);
        }
    }
}