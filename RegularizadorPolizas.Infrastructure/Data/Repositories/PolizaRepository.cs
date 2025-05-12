using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class PolizaRepository : GenericRepository<Poliza>, IPolizaRepository
    {
        public PolizaRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}