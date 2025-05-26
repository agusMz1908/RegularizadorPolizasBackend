using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Poliza> Polizas { get; set; }
        public DbSet<ProcessDocument> ProcessDocuments { get; set; }
        public DbSet<Renovation> Renovations { get; set; }
        public DbSet<Company> Companies { get; set; } // Cambiado de Companys a Companies
        public DbSet<Broker> Brokers { get; set; } // Agregado
        public DbSet<Currency> Currencies { get; set; } // Agregado

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Poliza>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(p => p.Client)
                      .WithMany(c => c.Polizas)
                      .HasForeignKey(p => p.Clinro)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Tomador)
                      .WithMany()
                      .HasForeignKey(p => p.Clinro1)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Company)
                      .WithMany(c => c.Polizas)
                      .HasForeignKey(p => p.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Broker)
                      .WithMany(b => b.Polizas)
                      .HasForeignKey(p => p.BrokerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Currency)
                      .WithMany(c => c.Polizas)
                      .HasForeignKey(p => p.CurrencyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.PolizaPadre)
                      .WithMany(p => p.PolizasHijas)
                      .HasForeignKey(p => p.Conpadre)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Cliruc).HasDatabaseName("IX_Clients_Cliruc");
                entity.HasIndex(e => e.Cliced).HasDatabaseName("IX_Clients_Cliced");
                entity.HasIndex(e => e.Cliemail).HasDatabaseName("IX_Clients_Cliemail");
            });

            modelBuilder.Entity<ProcessDocument>(entity =>
            {
                entity.HasOne(pd => pd.Poliza)
                      .WithMany(p => p.ProcessDocuments)
                      .HasForeignKey(pd => pd.PolizaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pd => pd.User)
                      .WithMany(u => u.ProcessDocuments)
                      .HasForeignKey(pd => pd.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Renovation>(entity =>
            {
                entity.HasOne(r => r.PolizaOriginal)
                      .WithMany(p => p.RenovacionesOrigen)
                      .HasForeignKey(r => r.PolizaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.PolizaNueva)
                      .WithMany(p => p.RenovacionesDestino)
                      .HasForeignKey(r => r.PolizaNuevaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Renovations)
                      .HasForeignKey(r => r.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Codigo).IsUnique().HasDatabaseName("IX_Companies_Codigo");
            });

            modelBuilder.Entity<Broker>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Codigo).HasDatabaseName("IX_Brokers_Codigo");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Codigo).IsUnique().HasDatabaseName("IX_Currencies_Codigo");
            });

            SeedData.ApplyAllSeedData(modelBuilder);
        }
    }
}