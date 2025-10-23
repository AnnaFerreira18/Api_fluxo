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
    public class ProjetoRepository : IProjetoRepository
    {
        private readonly FluxoDeNotasDbContext _context;

        public ProjetoRepository(FluxoDeNotasDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Projeto projeto)
        {
            await _context.Projetos.AddAsync(projeto);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Projeto>> GetAllByUserIdAsync(Guid userId)
        {

            return await _context.Projetos
                .Where(p => p.IdUsuario == userId && !p.Deletado)
                .OrderByDescending(p => p.DataCriacao) 
                .ToListAsync(); 
        }

        public async Task<Projeto?> GetByIdAndUserIdAsync(Guid id, Guid userId)
        {

            return await _context.Projetos
                .FirstOrDefaultAsync(p => p.IdProjeto == id &&
                                           p.IdUsuario == userId &&
                                           !p.Deletado);
        }

        public async Task UpdateAsync(Projeto projeto)
        {
            _context.Projetos.Update(projeto);
            await _context.SaveChangesAsync();
        }

        public async Task<Projeto?> GetByIdEvenIfDeletedAsync(Guid id, Guid userId)
        {
            return await _context.Projetos
                .FirstOrDefaultAsync(p => p.IdProjeto == id &&
                                           p.IdUsuario == userId);
        }

        public async Task<IEnumerable<Projeto>> GetLixeiraByUserIdAsync(Guid userId)
        {
            var tresDiasAtras = DateTime.Now.AddDays(-3);

            // Busca projetos do usuário que estão Deletados
            // E cuja DataDeletado é MAIS RECENTE que 3 dias atrás
            return await _context.Projetos
                .Where(p => p.IdUsuario == userId &&
                            p.Deletado &&
                            p.DataDeletado.HasValue &&
                            p.DataDeletado.Value > tresDiasAtras)
                .OrderByDescending(p => p.DataDeletado) // Ordena por data de exclusão
                .ToListAsync();
        }
    }
}
