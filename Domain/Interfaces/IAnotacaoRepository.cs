using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAnotacaoRepository
    {
        Task AddAsync(Anotacao anotacao);
        Task<IEnumerable<Anotacao>> GetAllByProjetoIdAsync(Guid projetoId);
        Task<Anotacao?> GetByIdAsync(Guid id);
        Task UpdateAsync(Anotacao anotacao);
        Task<Anotacao?> GetByIdEvenIfDeletedAsync(Guid id);
        Task<IEnumerable<Anotacao>> GetLixeiraByUserIdAsync(Guid userId);
        Task SaveChangesAsync();
    }
}
