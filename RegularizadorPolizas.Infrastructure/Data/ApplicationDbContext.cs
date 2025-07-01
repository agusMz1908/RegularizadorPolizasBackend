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
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<AutorizacionDatos> AutorizacionesDatos { get; set; }
        public DbSet<Seccion> Secciones { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Destino> Destinos { get; set; }
        public DbSet<TipoSiniestro> TiposSiniestro { get; set; }
        public DbSet<TipoContrato> TiposContrato { get; set; }
        public DbSet<TipoRiesgo> TiposRiesgo { get; set; }
        public DbSet<TipoAlarma> TiposAlarma { get; set; }
        public DbSet<CoberturaBicicleta> CoberturasBicicleta { get; set; }
        public DbSet<MotivoNoRenovacion> MotivosNoRenovacion { get; set; }
        public DbSet<EdoGestion> EdosGestion { get; set; }
        public DbSet<Taller> Talleres { get; set; }
        public DbSet<Combustible> Combustibles { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Calidad> Calidades { get; set; }
        public DbSet<FormaPago> FormasPago { get; set; }
        public DbSet<AutorizaCliente> AutorizacionesCliente { get; set; }
        public DbSet<CuentaBancaria> CuentasBancarias { get; set; }
        public DbSet<Tarjeta> Tarjetas { get; set; }
        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<CategoriaCliente> CategoriasCliente { get; set; }
        public DbSet<GrupoEconomico> GruposEconomicos { get; set; }
        public DbSet<Cobertura> Coberturas { get; set; }
        public DbSet<ContactoCompania> ContactosCompania { get; set; }
        public DbSet<ComisionPorSeccion> ComisionesPorSeccion { get; set; }
        public DbSet<ImpuestoPorSeccion> ImpuestosPorSeccion { get; set; }
        public DbSet<NumeroUtil> NumerosUtiles { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureExistingEntities(modelBuilder);
            ConfigureAuthenticationEntities(modelBuilder);
            //SeedData.ApplyAllSeedData(modelBuilder);
            SeedAuthenticationData(modelBuilder);
            ConfigureUserEntities(modelBuilder);
            ConfigureFase1Entities(modelBuilder);
            ConfigureClientEntities(modelBuilder);
            ConfigureCompanyAuxiliaryEntities(modelBuilder);
            ConfigureVehiculoEntity(modelBuilder);
            ConfigureVehiculoEntity(modelBuilder);
        }

        private void ConfigureExistingEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Poliza>(entity =>
            {
                entity.HasKey(e => e.Id);
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

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("IX_Companies_Codigo");
            });

            modelBuilder.Entity<Broker>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Codigo)
                    .HasDatabaseName("IX_Brokers_Codigo");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("IX_Currencies_Codigo");
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OldValues).HasColumnType("TEXT");
                entity.Property(e => e.NewValues).HasColumnType("TEXT");
                entity.Property(e => e.AdditionalData).HasColumnType("TEXT");
                entity.Property(e => e.StackTrace).HasColumnType("TEXT");

                entity.HasOne(a => a.User)
                      .WithMany()
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.EventType);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.EntityName);
                entity.HasIndex(e => e.EntityId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Success);
                entity.HasIndex(e => new { e.EntityName, e.EntityId });
                entity.HasIndex(e => new { e.UserId, e.Timestamp });
            });

            modelBuilder.Entity<ApiKey>(entity =>
            {
                entity.ToTable("ApiKeys");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TenantId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.BaseUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500);

                entity.Property(e => e.Environment)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Production");

                entity.Property(e => e.ContactEmail)
                    .HasMaxLength(200);

                entity.Property(e => e.ApiVersion)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValue("v1");

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.EnableLogging)
                    .HasDefaultValue(true);

                entity.Property(e => e.EnableRetries)
                    .HasDefaultValue(true);

                entity.Property(e => e.TimeoutSeconds)
                    .HasDefaultValue(30);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.TenantId)
                    .IsUnique()
                    .HasDatabaseName("IX_ApiKeys_TenantId");

                entity.HasIndex(e => e.Key)
                    .IsUnique()
                    .HasDatabaseName("IX_ApiKeys_Key");

                entity.HasIndex(e => e.Environment)
                    .HasDatabaseName("IX_ApiKeys_Environment");

                entity.HasIndex(e => e.Activo)
                    .HasDatabaseName("IX_ApiKeys_Activo");

                entity.HasIndex(e => e.FechaExpiracion)
                    .HasDatabaseName("IX_ApiKeys_FechaExpiracion");

                entity.HasIndex(e => e.LastUsed)
                    .HasDatabaseName("IX_ApiKeys_LastUsed");

                entity.HasIndex(e => new { e.Activo, e.FechaExpiracion })
                    .HasDatabaseName("IX_ApiKeys_Activo_FechaExpiracion");
            });
        }

        private void ConfigureAuthenticationEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_Roles_Name");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.Resource, e.Action }).IsUnique().HasDatabaseName("IX_Permissions_Resource_Action");
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_Permissions_Name");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Resource).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(200);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique().HasDatabaseName("IX_UserRoles_UserId_RoleId");
                entity.HasIndex(e => e.UserId).HasDatabaseName("IX_UserRoles_UserId");
                entity.HasIndex(e => e.RoleId).HasDatabaseName("IX_UserRoles_RoleId");

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.AssignedByUser)
                    .WithMany()
                    .HasForeignKey(ur => ur.AssignedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique().HasDatabaseName("IX_RolePermissions_RoleId_PermissionId");
                entity.HasIndex(e => e.RoleId).HasDatabaseName("IX_RolePermissions_RoleId");
                entity.HasIndex(e => e.PermissionId).HasDatabaseName("IX_RolePermissions_PermissionId");

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.GrantedByUser)
                    .WithMany()
                    .HasForeignKey(rp => rp.GrantedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void ConfigureUserEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(200);

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_Roles_Name");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Resource)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Description)
                    .HasMaxLength(200);

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_Permissions_Name");

                entity.HasIndex(e => new { e.Resource, e.Action })
                    .HasDatabaseName("IX_Permissions_Resource_Action");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.UserId, e.RoleId })
                    .HasDatabaseName("IX_UserRoles_User_Role");

                entity.Property(e => e.AssignedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.AssignedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.AssignedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.RoleId, e.PermissionId })
                    .HasDatabaseName("IX_RolePermissions_Role_Permission");

                entity.Property(e => e.GrantedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.GrantedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.GrantedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void ConfigureFase1Entities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AutorizacionDatos>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_AutorizacionesDatos_Name");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_AutorizacionesDatos_Activo");
            });

            modelBuilder.Entity<Seccion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Icono).HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_Secciones_Name");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_Secciones_Activo");
            });

            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Dptnom).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Dptbonint).HasMaxLength(10);
                entity.Property(e => e.ScCod).HasMaxLength(20);
                entity.HasIndex(e => e.Dptnom).IsUnique().HasDatabaseName("IX_Departamentos_Dptnom");
                entity.HasIndex(e => e.ScCod).HasDatabaseName("IX_Departamentos_ScCod");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_Departamentos_Activo");
            });

            modelBuilder.Entity<Destino>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Desnom).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descod).HasMaxLength(20);
                entity.HasIndex(e => e.Desnom).IsUnique().HasDatabaseName("IX_Destinos_Desnom");
                entity.HasIndex(e => e.Descod).HasDatabaseName("IX_Destinos_Descod");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_Destinos_Activo");
            });

            modelBuilder.Entity<TipoSiniestro>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_TiposSiniestro_Name");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_TiposSiniestro_Activo");
            });

            modelBuilder.Entity<TipoContrato>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TpoConDssc).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TpoDet).HasMaxLength(500);
                entity.HasIndex(e => e.TpoConDssc).IsUnique().HasDatabaseName("IX_TiposContrato_TpoConDssc");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_TiposContrato_Activo");
            });

            modelBuilder.Entity<TipoRiesgo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TpoRieDssc).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RieDesc).HasMaxLength(500);
                entity.HasIndex(e => e.TpoRieDssc).IsUnique().HasDatabaseName("IX_TiposRiesgo_TpoRieDssc");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_TiposRiesgo_Activo");
            });

            modelBuilder.Entity<TipoAlarma>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_TiposAlarma_Name");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_TiposAlarma_Activo");
            });

            modelBuilder.Entity<CoberturaBicicleta>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_CoberturasBicicleta_Name");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_CoberturasBicicleta_Activo");
            });

            modelBuilder.Entity<MotivoNoRenovacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_MotivosNoRenovacion_Name");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_MotivosNoRenovacion_Activo");
            });

            modelBuilder.Entity<EdoGestion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_EdosGestion_Name");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_EdosGestion_Activo");
            });

            modelBuilder.Entity<Taller>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Telefono).HasMaxLength(30);
                entity.Property(e => e.Direccion).HasMaxLength(200);
                entity.Property(e => e.Contacto).HasMaxLength(100);
                entity.Property(e => e.Cel).HasMaxLength(30);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Web).HasMaxLength(100);
                entity.Property(e => e.Observaciones).HasColumnType("TEXT");
                
                entity.HasIndex(e => e.Name).HasDatabaseName("IX_Talleres_Name");
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_Talleres_Email");
                entity.HasIndex(e => e.Telefono).HasDatabaseName("IX_Talleres_Telefono");
            });

            modelBuilder.Entity<Combustible>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_Combustibles_Name");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_Combustibles_Activo");
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_Categorias_Name");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_Categorias_Activo");
            });

            modelBuilder.Entity<Calidad>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_Calidades_Name");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_Calidades_Activo");
            });

            modelBuilder.Entity<FormaPago>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_FormasPago_Name");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_FormasPago_Activo");
                entity.HasIndex(e => e.EsContado).HasDatabaseName("IX_FormasPago_EsContado");
            });
        }

        private void ConfigureClientEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasIndex(e => new { e.Corrcod, e.Subcorr })
                    .HasDatabaseName("IX_Clients_Corredor");

                entity.HasIndex(e => e.Clinom)
                    .HasDatabaseName("IX_Clients_Nombre");

                entity.HasIndex(e => e.Activo)
                    .HasDatabaseName("IX_Clients_Activo");

                entity.HasIndex(e => new { e.Activo, e.FechaCreacion })
                    .HasDatabaseName("IX_Clients_Activo_FechaCreacion");

                entity.Property(e => e.Password)
                    .HasMaxLength(255);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.HasOne(c => c.CategoriaCliente)
                    .WithMany(cc => cc.Clientes)
                    .HasForeignKey(c => c.Categorias_de_cliente)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(c => c.GrupoEconomico)
                    .WithMany(ge => ge.Clientes)
                    .HasForeignKey(c => c.Grupos_economicos)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<AutorizaCliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("AutorizacionesCliente");

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(500);

                entity.Property(e => e.Fecha)
                    .IsRequired();

                entity.Property(e => e.Autorizado)
                    .HasDefaultValue(false);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                entity.HasOne(a => a.Cliente)
                    .WithMany(c => c.AutorizacionesCliente)
                    .HasForeignKey(a => a.Clientes)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.AutorizacionDatos)
                    .WithMany()
                    .HasForeignKey(a => a.AutorizacionesDeDatos)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Clientes)
                    .HasDatabaseName("IX_AutorizacionesCliente_Clientes");

                entity.HasIndex(e => e.AutorizacionesDeDatos)
                    .HasDatabaseName("IX_AutorizacionesCliente_AutorizacionesDeDatos");

                entity.HasIndex(e => e.Autorizado)
                    .HasDatabaseName("IX_AutorizacionesCliente_Autorizado");

                entity.HasIndex(e => e.Fecha)
                    .HasDatabaseName("IX_AutorizacionesCliente_Fecha");
            });

            modelBuilder.Entity<CuentaBancaria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("CuentasBancarias");

                entity.Property(e => e.Titular)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Name) 
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Numero)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Tipo)
                    .HasMaxLength(10);

                entity.Property(e => e.MonedaCuenta)
                    .HasMaxLength(5);

                entity.Property(e => e.Sucursal)
                    .HasMaxLength(10);

                entity.Property(e => e.Subcuenta)
                    .HasMaxLength(20);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                entity.HasOne(cb => cb.Cliente)
                    .WithMany(c => c.CuentasBancarias)
                    .HasForeignKey(cb => cb.Clientes)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Clientes)
                    .HasDatabaseName("IX_CuentasBancarias_Clientes");

                entity.HasIndex(e => e.Numero)
                    .HasDatabaseName("IX_CuentasBancarias_Numero");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_CuentasBancarias_Name");
            });

            modelBuilder.Entity<Tarjeta>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Tarjetas");

                entity.Property(e => e.Emisor)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Titular)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Numero)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Vencimiento)
                    .IsRequired();

                entity.Property(e => e.Control)
                    .HasMaxLength(10);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                entity.HasOne(t => t.Cliente)
                    .WithMany(c => c.Tarjetas)
                    .HasForeignKey(t => t.Clientes)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Clientes)
                    .HasDatabaseName("IX_Tarjetas_Clientes");

                entity.HasIndex(e => e.Emisor)
                    .HasDatabaseName("IX_Tarjetas_Emisor");

                entity.HasIndex(e => e.Vencimiento)
                    .HasDatabaseName("IX_Tarjetas_Vencimiento");
            });

            modelBuilder.Entity<Contacto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Contactos");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CargoRelacion)
                    .HasMaxLength(100);

                entity.Property(e => e.Cel) 
                    .HasMaxLength(30);

                entity.Property(e => e.Domicilio)
                    .HasMaxLength(200);

                entity.Property(e => e.Mail) 
                    .HasMaxLength(100);

                entity.Property(e => e.Obs) 
                    .HasMaxLength(500);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                entity.HasOne(c => c.Cliente)
                    .WithMany(cl => cl.Contactos)
                    .HasForeignKey(c => c.Clientes)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Clientes)
                    .HasDatabaseName("IX_Contactos_Clientes");

                entity.HasIndex(e => e.Nombre)
                    .HasDatabaseName("IX_Contactos_Nombre");

                entity.HasIndex(e => e.Mail)
                    .HasDatabaseName("IX_Contactos_Mail");

                entity.HasIndex(e => e.Cel)
                    .HasDatabaseName("IX_Contactos_Cel");
            });

            modelBuilder.Entity<CategoriaCliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("CategoriasCliente");

                entity.Property(e => e.Name) 
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ValMin) 
                    .HasColumnType("decimal(15,2)");

                entity.Property(e => e.ValMax)
                    .HasColumnType("decimal(15,2)");

                entity.Property(e => e.Color)
                    .HasMaxLength(20);

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_CategoriasCliente_Name");

                entity.HasIndex(e => e.Activo)
                    .HasDatabaseName("IX_CategoriasCliente_Activo");
            });

            modelBuilder.Entity<GrupoEconomico>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("GruposEconomicos");

                entity.Property(e => e.Name) 
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_GruposEconomicos_Name");

                entity.HasIndex(e => e.Activo)
                    .HasDatabaseName("IX_GruposEconomicos_Activo");
            });
        }

        private void ConfigureCompanyAuxiliaryEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cobertura>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Coberturas");

                entity.Property(e => e.Cobdsc)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.Cobdsc)
                    .IsUnique()
                    .HasDatabaseName("IX_Coberturas_Cobdsc");

                entity.HasIndex(e => e.Activo)
                    .HasDatabaseName("IX_Coberturas_Activo");
            });

            modelBuilder.Entity<ContactoCompania>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("ContactosCompania");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(30);

                entity.Property(e => e.Mail)
                    .HasMaxLength(100);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                entity.HasOne(cc => cc.Company)
                    .WithMany() 
                    .HasForeignKey(cc => cc.Companias)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Companias)
                    .HasDatabaseName("IX_ContactosCompania_Companias");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_ContactosCompania_Name");

                entity.HasIndex(e => e.Mail)
                    .HasDatabaseName("IX_ContactosCompania_Mail");
            });

            modelBuilder.Entity<ComisionPorSeccion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("ComisionesPorSeccion");

                entity.Property(e => e.Comipor)
                    .IsRequired()
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.Comi_bo)
                    .HasColumnType("decimal(5,2)")
                    .HasDefaultValue(0);

                entity.Property(e => e.Opera_comi)
                    .HasMaxLength(10)
                    .HasDefaultValue("N");

                entity.Property(e => e.Detalle)
                    .HasMaxLength(200);

                entity.Property(e => e.Tipos_de_contrato)
                    .HasDefaultValue(0);

                entity.Property(e => e.Productos_de_vida)
                    .HasDefaultValue(0);

                entity.Property(e => e.Anios_d)
                    .HasDefaultValue(0);

                entity.Property(e => e.Anios_h)
                    .HasDefaultValue(0);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                entity.HasOne(cps => cps.Company)
                    .WithMany() 
                    .HasForeignKey(cps => cps.Compania)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cps => cps.SeccionEntity)
                    .WithMany() 
                    .HasForeignKey(cps => cps.Seccion)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Compania)
                    .HasDatabaseName("IX_ComisionesPorSeccion_Compania");

                entity.HasIndex(e => e.Seccion)
                    .HasDatabaseName("IX_ComisionesPorSeccion_Seccion");

                entity.HasIndex(e => new { e.Compania, e.Seccion })
                    .HasDatabaseName("IX_ComisionesPorSeccion_Compania_Seccion");
            });

            modelBuilder.Entity<ImpuestoPorSeccion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("ImpuestosPorSeccion");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                entity.HasOne(ips => ips.Company)
                    .WithMany() 
                    .HasForeignKey(ips => ips.Companias)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ips => ips.Seccion)
                    .WithMany() 
                    .HasForeignKey(ips => ips.Secciones)
                    .OnDelete(DeleteBehavior.Restrict);

                // NOTA: La relación con Impuesto se agregará cuando se habilite en Velneo
                // entity.HasOne(ips => ips.Impuesto)
                //     .WithMany()
                //     .HasForeignKey(ips => ips.Impuestos)
                //     .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Companias)
                    .HasDatabaseName("IX_ImpuestosPorSeccion_Companias");

                entity.HasIndex(e => e.Secciones)
                    .HasDatabaseName("IX_ImpuestosPorSeccion_Secciones");

                entity.HasIndex(e => e.Impuestos)
                    .HasDatabaseName("IX_ImpuestosPorSeccion_Impuestos");

                entity.HasIndex(e => new { e.Companias, e.Secciones, e.Impuestos })
                    .HasDatabaseName("IX_ImpuestosPorSeccion_Companias_Secciones_Impuestos");
            });

            modelBuilder.Entity<NumeroUtil>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("NumerosUtiles");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Telefono)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                entity.HasOne(nu => nu.Company)
                    .WithMany() 
                    .HasForeignKey(nu => nu.Companias)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Companias)
                    .HasDatabaseName("IX_NumerosUtiles_Companias");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_NumerosUtiles_Name");

                entity.HasIndex(e => e.Telefono)
                    .HasDatabaseName("IX_NumerosUtiles_Telefono");
            });
        }

        private void ConfigureVehiculoEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Vehiculos");

                // Configuración de campos de texto
                entity.Property(e => e.Conmaraut)
                    .HasMaxLength(20);

                entity.Property(e => e.Conmataut)
                    .HasMaxLength(30);

                entity.Property(e => e.Conmotor)
                    .HasMaxLength(30);

                entity.Property(e => e.Conpadaut)
                    .HasMaxLength(30);

                entity.Property(e => e.Conchasis)
                    .HasMaxLength(30);

                entity.Property(e => e.Condetail)
                    .HasMaxLength(500);

                entity.Property(e => e.Contpocob)
                    .HasMaxLength(80);

                entity.Property(e => e.Contipoemp)
                    .HasMaxLength(80);

                entity.Property(e => e.Conmatpar)
                    .HasMaxLength(80);

                entity.Property(e => e.Conmatte)
                    .HasMaxLength(80);

                entity.Property(e => e.Combustibles)
                    .HasMaxLength(50);

                // Valores por defecto
                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.TieneAlarma)
                    .HasDefaultValue(false);

                entity.Property(e => e.Granizo)
                    .HasDefaultValue(false);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                // Relaciones con catálogos
                entity.HasOne(v => v.Categoria)
                    .WithMany()
                    .HasForeignKey(v => v.CategoriaId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(v => v.Calidad)
                    .WithMany()
                    .HasForeignKey(v => v.CalidadId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(v => v.Destino)
                    .WithMany()
                    .HasForeignKey(v => v.DestinoId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(v => v.Combustible)
                    .WithMany()
                    .HasForeignKey(v => v.CombustibleId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Índices para mejorar performance
                entity.HasIndex(e => e.Conmataut)
                    .HasDatabaseName("IX_Vehiculos_Conmataut")
                    .IsUnique();

                entity.HasIndex(e => e.Conmaraut)
                    .HasDatabaseName("IX_Vehiculos_Conmaraut");

                entity.HasIndex(e => e.Conanioaut)
                    .HasDatabaseName("IX_Vehiculos_Conanioaut");

                entity.HasIndex(e => e.Conmotor)
                    .HasDatabaseName("IX_Vehiculos_Conmotor");

                entity.HasIndex(e => e.Conpadaut)
                    .HasDatabaseName("IX_Vehiculos_Conpadaut");

                entity.HasIndex(e => e.Conchasis)
                    .HasDatabaseName("IX_Vehiculos_Conchasis");

                entity.HasIndex(e => e.Activo)
                    .HasDatabaseName("IX_Vehiculos_Activo");

                entity.HasIndex(e => new { e.Activo, e.FechaCreacion })
                    .HasDatabaseName("IX_Vehiculos_Activo_FechaCreacion");

                // Índices para las relaciones FK
                entity.HasIndex(e => e.CategoriaId)
                    .HasDatabaseName("IX_Vehiculos_CategoriaId");

                entity.HasIndex(e => e.CalidadId)
                    .HasDatabaseName("IX_Vehiculos_CalidadId");

                entity.HasIndex(e => e.DestinoId)
                    .HasDatabaseName("IX_Vehiculos_DestinoId");

                entity.HasIndex(e => e.CombustibleId)
                    .HasDatabaseName("IX_Vehiculos_CombustibleId");

                // Índice compuesto para búsquedas complejas
                entity.HasIndex(e => new { e.Conmaraut, e.Conanioaut, e.Conmataut })
                    .HasDatabaseName("IX_Vehiculos_Marca_Anio_Matricula");
            });
        }

        private void SeedAuthenticationData(ModelBuilder modelBuilder)
        {
            var now = DateTime.UtcNow;

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin", Description = "Full system access", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Role { Id = 2, Name = "Admin", Description = "Administrative access", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Role { Id = 3, Name = "Manager", Description = "Management access", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Role { Id = 4, Name = "Operator", Description = "Operational access", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Role { Id = 5, Name = "Viewer", Description = "Read-only access", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Role { Id = 6, Name = "Client", Description = "Client portal access", IsActive = true, CreatedAt = now, UpdatedAt = now }
            );

            // Seed Permissions
            var permissions = new List<Permission>
            {
                // Clients
                new Permission { Id = 1, Name = "clients.read", Resource = "Clients", Action = "Read", Description = "View clients", IsActive = true, CreatedAt = now },
                new Permission { Id = 2, Name = "clients.create", Resource = "Clients", Action = "Create", Description = "Create clients", IsActive = true, CreatedAt = now },
                new Permission { Id = 3, Name = "clients.update", Resource = "Clients", Action = "Update", Description = "Update clients", IsActive = true, CreatedAt = now },
                new Permission { Id = 4, Name = "clients.delete", Resource = "Clients", Action = "Delete", Description = "Delete clients", IsActive = true, CreatedAt = now },
                new Permission { Id = 5, Name = "clients.search", Resource = "Clients", Action = "Search", Description = "Search clients", IsActive = true, CreatedAt = now },

                // Polizas
                new Permission { Id = 6, Name = "polizas.read", Resource = "Polizas", Action = "Read", Description = "View policies", IsActive = true, CreatedAt = now },
                new Permission { Id = 7, Name = "polizas.create", Resource = "Polizas", Action = "Create", Description = "Create policies", IsActive = true, CreatedAt = now },
                new Permission { Id = 8, Name = "polizas.update", Resource = "Polizas", Action = "Update", Description = "Update policies", IsActive = true, CreatedAt = now },
                new Permission { Id = 9, Name = "polizas.delete", Resource = "Polizas", Action = "Delete", Description = "Delete policies", IsActive = true, CreatedAt = now },
                new Permission { Id = 10, Name = "polizas.renew", Resource = "Polizas", Action = "Renew", Description = "Renew policies", IsActive = true, CreatedAt = now },
                new Permission { Id = 11, Name = "polizas.search", Resource = "Polizas", Action = "Search", Description = "Search policies", IsActive = true, CreatedAt = now },

                // Documents
                new Permission { Id = 12, Name = "documents.read", Resource = "Documents", Action = "Read", Description = "View documents", IsActive = true, CreatedAt = now },
                new Permission { Id = 13, Name = "documents.upload", Resource = "Documents", Action = "Upload", Description = "Upload documents", IsActive = true, CreatedAt = now },
                new Permission { Id = 14, Name = "documents.process", Resource = "Documents", Action = "Process", Description = "Process documents", IsActive = true, CreatedAt = now },
                new Permission { Id = 15, Name = "documents.delete", Resource = "Documents", Action = "Delete", Description = "Delete documents", IsActive = true, CreatedAt = now },

                // Brokers
                new Permission { Id = 16, Name = "brokers.read", Resource = "Brokers", Action = "Read", Description = "View brokers", IsActive = true, CreatedAt = now },
                new Permission { Id = 17, Name = "brokers.create", Resource = "Brokers", Action = "Create", Description = "Create brokers", IsActive = true, CreatedAt = now },
                new Permission { Id = 18, Name = "brokers.update", Resource = "Brokers", Action = "Update", Description = "Update brokers", IsActive = true, CreatedAt = now },
                new Permission { Id = 19, Name = "brokers.delete", Resource = "Brokers", Action = "Delete", Description = "Delete brokers", IsActive = true, CreatedAt = now },

                // Companies
                new Permission { Id = 20, Name = "companies.read", Resource = "Companies", Action = "Read", Description = "View companies", IsActive = true, CreatedAt = now },
                new Permission { Id = 21, Name = "companies.create", Resource = "Companies", Action = "Create", Description = "Create companies", IsActive = true, CreatedAt = now },
                new Permission { Id = 22, Name = "companies.update", Resource = "Companies", Action = "Update", Description = "Update companies", IsActive = true, CreatedAt = now },
                new Permission { Id = 23, Name = "companies.delete", Resource = "Companies", Action = "Delete", Description = "Delete companies", IsActive = true, CreatedAt = now },

                // Currencies
                new Permission { Id = 24, Name = "currencies.read", Resource = "Currencies", Action = "Read", Description = "View currencies", IsActive = true, CreatedAt = now },
                new Permission { Id = 25, Name = "currencies.create", Resource = "Currencies", Action = "Create", Description = "Create currencies", IsActive = true, CreatedAt = now },
                new Permission { Id = 26, Name = "currencies.update", Resource = "Currencies", Action = "Update", Description = "Update currencies", IsActive = true, CreatedAt = now },
                new Permission { Id = 27, Name = "currencies.delete", Resource = "Currencies", Action = "Delete", Description = "Delete currencies", IsActive = true, CreatedAt = now },

                // Renovations
                new Permission { Id = 28, Name = "renovations.read", Resource = "Renovations", Action = "Read", Description = "View renovations", IsActive = true, CreatedAt = now },
                new Permission { Id = 29, Name = "renovations.create", Resource = "Renovations", Action = "Create", Description = "Create renovations", IsActive = true, CreatedAt = now },
                new Permission { Id = 30, Name = "renovations.process", Resource = "Renovations", Action = "Process", Description = "Process renovations", IsActive = true, CreatedAt = now },
                new Permission { Id = 31, Name = "renovations.cancel", Resource = "Renovations", Action = "Cancel", Description = "Cancel renovations", IsActive = true, CreatedAt = now },

                // Administration
                new Permission { Id = 32, Name = "admin.users.manage", Resource = "Admin", Action = "ManageUsers", Description = "Manage users", IsActive = true, CreatedAt = now },
                new Permission { Id = 33, Name = "admin.roles.manage", Resource = "Admin", Action = "ManageRoles", Description = "Manage roles", IsActive = true, CreatedAt = now },
                new Permission { Id = 34, Name = "admin.permissions.manage", Resource = "Admin", Action = "ManagePermissions", Description = "Manage permissions", IsActive = true, CreatedAt = now },
                new Permission { Id = 35, Name = "admin.audit.read", Resource = "Admin", Action = "ReadAudit", Description = "View audit logs", IsActive = true, CreatedAt = now },
                new Permission { Id = 36, Name = "admin.system.config", Resource = "Admin", Action = "SystemConfig", Description = "System configuration", IsActive = true, CreatedAt = now },

                // Reports
                new Permission { Id = 37, Name = "reports.read", Resource = "Reports", Action = "Read", Description = "View reports", IsActive = true, CreatedAt = now },
                new Permission { Id = 38, Name = "reports.export", Resource = "Reports", Action = "Export", Description = "Export reports", IsActive = true, CreatedAt = now }
            };

            modelBuilder.Entity<Permission>().HasData(permissions);

            // Seed UserRole - Asignar SuperAdmin al usuario admin existente (asumiendo que existe un usuario con ID = 1)
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Id = 1, UserId = 1, RoleId = 1, AssignedAt = now, IsActive = true }
            );

            // Seed RolePermissions - SuperAdmin tiene todos los permisos
            var superAdminPermissions = permissions.Select((p, index) => new RolePermission
            {
                Id = index + 1,
                RoleId = 1, // SuperAdmin
                PermissionId = p.Id,
                GrantedAt = now,
                IsActive = true
            }).ToArray();

            modelBuilder.Entity<RolePermission>().HasData(superAdminPermissions);

            // Admin tiene la mayoría de permisos (excepto system config)
            var adminPermissionIds = permissions.Where(p => p.Id != 36).Select(p => p.Id).ToList(); // Sin system config
            var adminPermissions = adminPermissionIds.Select((permId, index) => new RolePermission
            {
                Id = superAdminPermissions.Length + index + 1,
                RoleId = 2, // Admin
                PermissionId = permId,
                GrantedAt = now,
                IsActive = true
            }).ToArray();

            modelBuilder.Entity<RolePermission>().HasData(adminPermissions);

            // Manager - permisos operacionales (sin delete críticos)
            var managerPermissionIds = new[] { 1, 2, 3, 5, 6, 7, 8, 10, 11, 12, 13, 14, 16, 17, 18, 20, 21, 22, 24, 25, 26, 28, 29, 30, 37 };
            var managerPermissions = managerPermissionIds.Select((permId, index) => new RolePermission
            {
                Id = superAdminPermissions.Length + adminPermissions.Length + index + 1,
                RoleId = 3, // Manager
                PermissionId = permId,
                GrantedAt = now,
                IsActive = true
            }).ToArray();

            modelBuilder.Entity<RolePermission>().HasData(managerPermissions);

            // Operator - permisos básicos operacionales
            var operatorPermissionIds = new[] { 1, 5, 6, 7, 8, 11, 12, 13, 16, 17, 20, 24, 28, 29 };
            var operatorPermissions = operatorPermissionIds.Select((permId, index) => new RolePermission
            {
                Id = superAdminPermissions.Length + adminPermissions.Length + managerPermissions.Length + index + 1,
                RoleId = 4, // Operator
                PermissionId = permId,
                GrantedAt = now,
                IsActive = true
            }).ToArray();

            modelBuilder.Entity<RolePermission>().HasData(operatorPermissions);

            // Viewer - solo lectura
            var viewerPermissionIds = new[] { 1, 5, 6, 11, 12, 16, 20, 24, 28, 37 };
            var viewerPermissions = viewerPermissionIds.Select((permId, index) => new RolePermission
            {
                Id = superAdminPermissions.Length + adminPermissions.Length + managerPermissions.Length + operatorPermissions.Length + index + 1,
                RoleId = 5, // Viewer
                PermissionId = permId,
                GrantedAt = now,
                IsActive = true
            }).ToArray();

            modelBuilder.Entity<RolePermission>().HasData(viewerPermissions);

            // Client - acceso muy limitado (solo sus propias pólizas y documentos)
            var clientPermissionIds = new[] { 6, 12, 28, 29 };
            var clientPermissions = clientPermissionIds.Select((permId, index) => new RolePermission
            {
                Id = superAdminPermissions.Length + adminPermissions.Length + managerPermissions.Length + operatorPermissions.Length + viewerPermissions.Length + index + 1,
                RoleId = 6, // Client
                PermissionId = permId,
                GrantedAt = now,
                IsActive = true
            }).ToArray();

            modelBuilder.Entity<RolePermission>().HasData(clientPermissions);
        }
    }
}