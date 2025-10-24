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
    public class TarefaRepository : ITarefaRepository
    {
        private readonly FluxoDeNotasDbContext _context;

        public TarefaRepository(FluxoDeNotasDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Tarefa tarefa)
        {
            await _context.Tarefas.AddAsync(tarefa);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Tarefa>> GetAllByUserIdAsync(Guid userId)
        {
            return await _context.Tarefas
                .Where(t => t.IdUsuario == userId && !t.Deletado)
                .OrderBy(t => t.Ordem)       
                .ThenByDescending(t => t.DataCriacao) 
                .ToListAsync();
        }

        public async Task<Tarefa?> GetByIdAndUserIdAsync(Guid id, Guid userId)
        {

            return await _context.Tarefas
                .FirstOrDefaultAsync(t => t.IdTarefa == id &&
                                           t.IdUsuario == userId &&
                                           !t.Deletado);
        }

        public async Task UpdateAsync(Tarefa tarefa)
        {

            _context.Tarefas.Update(tarefa);
            //await _context.SaveChangesAsync();
        }

        public async Task<Tarefa?> GetByIdEvenIfDeletedAsync(Guid id, Guid userId)
        {
            return await _context.Tarefas
                .FirstOrDefaultAsync(t => t.IdTarefa == id &&
                                           t.IdUsuario == userId);
        }

        public async Task<IEnumerable<Tarefa>> GetLixeiraByUserIdAsync(Guid userId)
        {
            var tresDiasAtras = DateTime.Now.AddDays(-1);

            return await _context.Tarefas
                .Where(t => t.IdUsuario == userId &&
                            t.Deletado &&
                            t.DataDeletado.HasValue &&
                            t.DataDeletado.Value > tresDiasAtras)
                .OrderByDescending(t => t.DataDeletado) 
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
