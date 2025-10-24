using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Anotacao;

namespace Application.Interfaces
{
    public interface IAnotacaoService
    {
        Task<AnotacaoResponseDto> CreateAsync(Guid projetoId, CriarAnotacaoRequestDto request);
        Task<IEnumerable<AnotacaoResponseDto>> GetAllByProjetoIdAsync(Guid projetoId);
        Task<AnotacaoResponseDto?> GetByIdAsync(Guid id);
        Task<AnotacaoResponseDto?> UpdateAsync(Guid id, EditarAnotacaoRequestDto request);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> RestaurarAsync(Guid id);
        Task<IEnumerable<AnotacaoResponseDto>> GetLixeiraAsync();
        Task<AnotacaoResponseDto?> AlternarFavoritoAsync(Guid id);
        Task OrdenarAsync(Guid projetoId, OrdenarAnotacoesRequestDto request);
    }
}
