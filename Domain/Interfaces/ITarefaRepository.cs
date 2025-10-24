using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ITarefaRepository
    {
        Task AddAsync(Tarefa tarefa);
        Task<IEnumerable<Tarefa>> GetAllByUserIdAsync(Guid userId);
        Task<Tarefa?> GetByIdAndUserIdAsync(Guid id, Guid userId);
        Task UpdateAsync(Tarefa tarefa);
        Task<Tarefa?> GetByIdEvenIfDeletedAsync(Guid id, Guid userId);

        Task<IEnumerable<Tarefa>> GetLixeiraByUserIdAsync(Guid userId);
        Task SaveChangesAsync();
    }
}
