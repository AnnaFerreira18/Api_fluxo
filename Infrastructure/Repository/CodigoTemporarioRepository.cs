using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class CodigoTemporarioRepository : ICodigoTemporarioRepository
    {
        private readonly FluxoDeNotasDbContext _context;

        public CodigoTemporarioRepository(FluxoDeNotasDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CodigoTemporario codigo)
        {
            await _context.CodigosTemporarios.AddAsync(codigo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CodigoTemporario codigo)
        {
            _context.CodigosTemporarios.Update(codigo);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CodigoTemporario>> GetCodigosValidosAsync(Guid usuarioId, string tipo, DateTime horaAtual)
        {
            return await _context.CodigosTemporarios
                .Where(c => c.IdUsuario == usuarioId &&
                            c.TipoDeCodigo == tipo &&
                            c.DataUtilizacao == null &&
                            c.DataExpiracao > DateTime.Now)
                .ToListAsync();
        }
    }
}
