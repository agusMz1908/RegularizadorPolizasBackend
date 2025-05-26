using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Infrastructure.Data
{
    public static class SeedData
    {
        public static void SeedCompanies(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = 1,
                    Nombre = "Banco de Seguros del Estado",
                    Alias = "BSE",
                    Codigo = "BSE",
                    Activo = true
                },
                new Company
                {
                    Id = 2,
                    Nombre = "SURA Uruguay",
                    Alias = "SURA",
                    Codigo = "SURA",
                    Activo = true
                },
                new Company
                {
                    Id = 3,
                    Nombre = "Mapfre Uruguay",
                    Alias = "MAPFRE",
                    Codigo = "MAPFRE",
                    Activo = true
                },
                new Company
                {
                    Id = 4,
                    Nombre = "San Cristóbal",
                    Alias = "SAN CRISTOBAL",
                    Codigo = "SC",
                    Activo = true
                },
                new Company
                {
                    Id = 5,
                    Nombre = "La Caja de Seguros",
                    Alias = "LA CAJA",
                    Codigo = "CAJA",
                    Activo = true
                }
            );
        }

        public static void SeedCurrencies(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>().HasData(
                new Currency
                {
                    Id = 1,
                    Nombre = "Peso Uruguayo",
                    Codigo = "UYU",
                    Simbolo = "$",
                    Activo = true
                },
                new Currency
                {
                    Id = 2,
                    Nombre = "Dólar Americano",
                    Codigo = "USD",
                    Simbolo = "US$",
                    Activo = true
                },
                new Currency
                {
                    Id = 3,
                    Nombre = "Unidad Indexada",
                    Codigo = "UI",
                    Simbolo = "UI",
                    Activo = true
                }
            );
        }

        public static void SeedUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Nombre = "Administrador",
                    Email = "admin@sistema.com",
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now
                },
                new User
                {
                    Id = 2,
                    Nombre = "Usuario Demo",
                    Email = "demo@sistema.com",
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now
                }
            );
        }

        public static void ApplyAllSeedData(ModelBuilder modelBuilder)
        {
            SeedCompanies(modelBuilder);
            SeedCurrencies(modelBuilder);
            SeedUsers(modelBuilder);
        }
    }
}