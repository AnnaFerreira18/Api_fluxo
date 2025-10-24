using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Tarefa;

namespace Application.Interfaces
{
    public interface ITarefaService
    {
        Task<TarefaResponseDto> CreateAsync(CriarTarefaRequestDto request);
        Task<IEnumerable<TarefaResponseDto>> GetAllAsync();
        Task<TarefaResponseDto?> GetByIdAsync(Guid id);
        Task<TarefaResponseDto?> UpdateAsync(Guid id, EditarTarefaRequestDto request);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> RestaurarAsync(Guid id);
        Task<IEnumerable<TarefaResponseDto>> GetLixeiraAsync();
        Task<TarefaResponseDto?> AlternarConcluidoAsync(Guid id);
        Task OrdenarAsync(OrdenarTarefasRequestDto request);
    }
}
