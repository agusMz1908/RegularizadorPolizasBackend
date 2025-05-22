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
        public DbSet<Company> Companys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Poliza
            modelBuilder.Entity<Poliza>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Cliente principal
                entity.HasOne(p => p.Client)
                      .WithMany(c => c.Polizas)
                      .HasForeignKey(p => p.Clinro)
                      .OnDelete(DeleteBehavior.Restrict);

                // Compañía
                entity.HasOne(p => p.Company)
                      .WithMany(c => c.Polizas)
                      .HasForeignKey(p => p.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Corredor
                entity.HasOne(p => p.Broker)
                      .WithMany(b => b.Polizas)
                      .HasForeignKey(p => p.BrokerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Moneda
                entity.HasOne(p => p.Currency)
                      .WithMany(c => c.Polizas)
                      .HasForeignKey(p => p.CurrencyId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Auto-relación
                entity.HasOne(p => p.PolizaPadre)
                      .WithMany(p => p.PolizasHijas)
                      .HasForeignKey(p => p.Conpadre)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Client actualizada
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasMany(c => c.Polizas)
                      .WithOne(p => p.Client)
                      .HasForeignKey(p => p.Clinro);

                entity.HasMany(c => c.Polizas)
                      .WithOne(p => p.Tomador)
                      .HasForeignKey(p => p.Clinro1);
            });
        }
    }
}
