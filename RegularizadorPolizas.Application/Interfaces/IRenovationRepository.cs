﻿using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IRenovationRepository : IGenericRepository<Renovation>
    {
        Task<IEnumerable<Renovation>> GetRenovationsByStatusAsync(string status);
        Task<IEnumerable<Renovation>> GetRenovationsByPolicyAsync(int polizaId);
        Task<Renovation> GetRenovationWithDetailsAsync(int id);
    }
}