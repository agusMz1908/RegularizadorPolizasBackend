namespace RegularizadorPolizas.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

        // Información de roles
        public List<RoleDto> Roles { get; set; } = new();
        public List<string> RoleNames { get; set; } = new();
        public bool IsAdmin => RoleNames.Contains("SuperAdmin") || RoleNames.Contains("Admin");
        public bool IsSuperAdmin => RoleNames.Contains("SuperAdmin");
    }

    public class UserCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<int> RoleIds { get; set; } = new();
        public bool Activo { get; set; } = true;
    }

    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }

    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int TotalRoles { get; set; }
        public string PrimaryRole { get; set; } = string.Empty;
        public DateTime UltimaActividad { get; set; }
        public bool PuedeEliminar { get; set; }
    }

    public class UserLookupDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }

    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalUsers { get; set; }
        public int TotalPermissions { get; set; }
    }

    public class RoleLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class AssignRoleDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int? AssignedBy { get; set; }
    }

    public class UserRoleAssignmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public string AssignedByName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}