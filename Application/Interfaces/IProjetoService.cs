using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Projeto;

namespace Application.Interfaces
{
    public interface IProjetoService
    {
        Task<ProjetoResponseDto> CreateAsync(CriarProjetoRequestDto request);
        Task<IEnumerable<ProjetoResponseDto>> GetAllAsync();
        Task<ProjetoResponseDto?> GetByIdAsync(Guid id);
        Task<ProjetoResponseDto?> UpdateAsync(Guid id, EditarProjetoRequestDto request);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> RestaurarAsync(Guid id);
        Task<IEnumerable<ProjetoResponseDto>> GetLixeiraAsync();
    }
}
