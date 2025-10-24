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
    public class AnotacaoRepository : IAnotacaoRepository
    {
        private readonly FluxoDeNotasDbContext _context;

        public AnotacaoRepository(FluxoDeNotasDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Anotacao anotacao)
        {
            await _context.Anotacoes.AddAsync(anotacao);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Anotacao>> GetAllByProjetoIdAsync(Guid projetoId)
        {
            // Busca no DbContext por Anotacoes onde o IdProjeto bate com o fornecido
            // E onde 'Deletado' seja false.
            // Ordena pela 'Ordem' e depois por data (útil para anotações sem ordem definida)
            return await _context.Anotacoes
                .Where(a => a.IdProjeto == projetoId && !a.Deletado)
                .OrderBy(a => a.Ordem)         // Ordena primeiro pela ordem definida
                .ThenByDescending(a => a.DataCriacao) // Depois pela data, mais recentes primeiro
                .ToListAsync();
        }

        public async Task<Anotacao?> GetByIdAsync(Guid id)
        {
            return await _context.Anotacoes
                .Include(a => a.Projeto)
                .FirstOrDefaultAsync(a => a.IdAnotacao == id && !a.Deletado);
        }

        public async Task UpdateAsync(Anotacao anotacao)
        {
            _context.Anotacoes.Update(anotacao);
            await _context.SaveChangesAsync();
        }

        public async Task<Anotacao?> GetByIdEvenIfDeletedAsync(Guid id)
        {

            return await _context.Anotacoes
                .Include(a => a.Projeto)
                .FirstOrDefaultAsync(a => a.IdAnotacao == id);
        }

        public async Task<IEnumerable<Anotacao>> GetLixeiraByUserIdAsync(Guid userId)
        {
            var tresDiasAtras = DateTime.Now.AddDays(-1);

            return await _context.Anotacoes
                .Include(a => a.Projeto) 
                .Where(a => a.Deletado &&
                            a.DataDeletado.HasValue &&
                            a.DataDeletado.Value > tresDiasAtras &&
                            a.Projeto.IdUsuario == userId) 
                .OrderByDescending(a => a.DataDeletado)
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
