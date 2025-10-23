using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProjetoRepository
    {
        Task AddAsync(Projeto projeto);
        Task<IEnumerable<Projeto>> GetAllByUserIdAsync(Guid userId);
        Task<Projeto?> GetByIdAndUserIdAsync(Guid id, Guid userId);

        Task UpdateAsync(Projeto projeto);

        Task<Projeto?> GetByIdEvenIfDeletedAsync(Guid id, Guid userId);

        Task<IEnumerable<Projeto>> GetLixeiraByUserIdAsync(Guid userId);
    }
}
