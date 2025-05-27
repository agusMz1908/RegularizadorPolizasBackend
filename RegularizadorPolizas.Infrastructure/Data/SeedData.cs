using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Infrastructure.Data
{
    public static class SeedData
    {
        public static void SeedCurrencies(ModelBuilder modelBuilder)
        {
            var creationDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Currency>().HasData(
                new Currency
                {
                    Id = 1,
                    Moneda = "Peso Uruguayo",
                    Activo = true,
                    FechaCreacion = creationDate,
                    FechaModificacion = creationDate
                },
                new Currency
                {
                    Id = 2,
                    Moneda = "Dólar Americano",
                    Activo = true,
                    FechaCreacion = creationDate,
                    FechaModificacion = creationDate
                },
                new Currency
                {
                    Id = 3,
                    Moneda = "Unidad Indexada",
                    Activo = true,
                    FechaCreacion = creationDate,
                    FechaModificacion = creationDate
                }
            );
        }

        public static void SeedUsers(ModelBuilder modelBuilder)
        {
            var creationDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Nombre = "Administrador",
                    Email = "admin@sistema.com",
                    Activo = true,
                    FechaCreacion = creationDate,
                    FechaModificacion = creationDate
                }
            );
        }

        public static void ApplyAllSeedData(ModelBuilder modelBuilder)
        {
            SeedCurrencies(modelBuilder);
            SeedUsers(modelBuilder);
        }
    }
}