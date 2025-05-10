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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Relacion Cliente -> Polizas
            modelBuilder.Entity<Poliza>()
                .HasOne(p => p.Client)
                .WithMany(c => c.Polizas)
                .HasForeignKey(p => p.Clinro)
                .OnDelete(DeleteBehavior.Restrict);

            //Relacion Poliza -> Polizas hijas (Auto-ref)
            modelBuilder.Entity<Poliza>()
                .HasOne(p => p.PolizaPadre)
                .WithMany(p => p.PolizasHijas)
                .HasForeignKey(p => p.Conpadre)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Poliza -> DocumentosProcesados
            modelBuilder.Entity<ProcessDocument>()
                .HasOne(d => d.Poliza)
                .WithMany(p => p.ProcessDocuments)
                .HasForeignKey(d => d.PolizaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Usuario -> DocumentosProcesados
            modelBuilder.Entity<ProcessDocument>()
                .HasOne(d => d.User)
                .WithMany(u => u.ProcessDocuments)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Poliza -> RenovacionesOrigen
            modelBuilder.Entity<Renovation>()
                .HasOne(r => r.PolizaOriginal)
                .WithMany(p => p.RenovacionesOrigen)
                .HasForeignKey(r => r.PolizaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Poliza -> RenovacionesDestino
            modelBuilder.Entity<Renovation>()
                .HasOne(r => r.PolizaNueva)
                .WithMany(p => p.RenovacionesDestino)
                .HasForeignKey(r => r.PolizaNuevaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Usuario -> Renovaciones
            modelBuilder.Entity<Renovation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Renovations)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
