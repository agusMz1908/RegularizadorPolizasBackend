﻿using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IUserRoleRepository : IGenericRepository<UserRole>
    {
        Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId);
        Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId);
        Task<UserRole?> GetByUserAndRoleAsync(int userId, int roleId);
        Task<IEnumerable<UserRole>> GetActiveUserRolesAsync(int userId);
        Task SetUserRolesAsync(int userId, IEnumerable<int> roleIds, int? assignedByUserId = null);
        Task RemoveUserRoleAsync(int userId, int roleId, int? removedByUserId = null);
    }
}