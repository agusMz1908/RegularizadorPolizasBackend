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
        public DbSet<Company> Companies { get; set; }
        public DbSet<Broker> Brokers { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === CONFIGURACIÓN DE POLIZA ===
            modelBuilder.Entity<Poliza>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Campos que deben ser TEXT (sin límite de longitud)
                entity.Property(e => e.Condom).HasColumnType("TEXT");
                entity.Property(e => e.Observaciones).HasColumnType("TEXT");
                entity.Property(e => e.Sublistas).HasColumnType("TEXT");
                entity.Property(e => e.Cotizacion).HasColumnType("TEXT");

                // Relaciones
                entity.HasOne(p => p.Client)
                      .WithMany(c => c.Polizas)
                      .HasForeignKey(p => p.Clinro)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Company)
                      .WithMany(c => c.Polizas)
                      .HasForeignKey(p => p.Comcod)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Broker)
                      .WithMany(b => b.Polizas)
                      .HasForeignKey(p => p.Corrnom)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Currency)
                      .WithMany(c => c.Polizas)
                      .HasForeignKey(p => p.Moncod)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.PolizaPadre)
                      .WithMany(p => p.PolizasHijas)
                      .HasForeignKey(p => p.Conpadre)
                      .OnDelete(DeleteBehavior.Restrict);

                // Índices para optimizar consultas
                entity.HasIndex(e => e.Conpol);
                entity.HasIndex(e => e.Conmataut);
                entity.HasIndex(e => e.Confchdes);
                entity.HasIndex(e => e.Confchhas);
                entity.HasIndex(e => e.Activo);
                entity.HasIndex(e => e.Clinro);
                entity.HasIndex(e => e.Comcod);
                entity.HasIndex(e => e.Corrnom);
                entity.HasIndex(e => e.Moncod);
                entity.HasIndex(e => e.Conpadre);
            });

            // === CONFIGURACIÓN DE CLIENT ===
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Cliobse)
                    .HasColumnType("TEXT");

                entity.Property(e => e.Clifoto)
                    .HasColumnType("LONGBLOB");

                entity.HasIndex(e => e.Cliruc).HasDatabaseName("IX_Clients_Cliruc");
                entity.HasIndex(e => e.Cliced).HasDatabaseName("IX_Clients_Cliced");
                entity.HasIndex(e => e.Cliemail).HasDatabaseName("IX_Clients_Cliemail");
            });

            // === CONFIGURACIÓN DE PROCESSDOCUMENT ===
            modelBuilder.Entity<ProcessDocument>(entity =>
            {
                entity.Property(e => e.ResultadoJson)
                    .HasColumnType("TEXT");

                entity.HasOne(pd => pd.Poliza)
                      .WithMany(p => p.ProcessDocuments)
                      .HasForeignKey(pd => pd.PolizaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pd => pd.User)
                      .WithMany(u => u.ProcessDocuments)
                      .HasForeignKey(pd => pd.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // === CONFIGURACIÓN DE RENOVATION ===
            modelBuilder.Entity<Renovation>(entity =>
            {
                entity.Property(e => e.Observaciones)
                    .HasColumnType("TEXT");

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

            // === CONFIGURACIÓN DE COMPANY ===
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("IX_Companies_Codigo");
            });

            // === CONFIGURACIÓN DE BROKER ===
            modelBuilder.Entity<Broker>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Codigo)
                    .HasDatabaseName("IX_Brokers_Codigo");
            });

            // === CONFIGURACIÓN DE CURRENCY ===
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("IX_Currencies_Codigo");
            });

            // === APLICAR SEED DATA ===
            SeedData.ApplyAllSeedData(modelBuilder);
        }
    }
}