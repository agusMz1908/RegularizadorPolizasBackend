using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Data;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureExistingEntities(modelBuilder);
            ConfigureAuthenticationEntities(modelBuilder);
            //SeedData.ApplyAllSeedData(modelBuilder);
            SeedAuthenticationData(modelBuilder);
            ConfigureUserEntities(modelBuilder);
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