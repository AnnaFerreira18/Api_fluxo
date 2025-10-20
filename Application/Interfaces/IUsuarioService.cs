using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUsuarioService
    {
        Task RegistrarAsync(RegistrarUsuarioRequestDto request);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<UsuarioResponseDto> GetMeAsync();
        Task<UsuarioResponseDto> UpdateMeAsync(EditarUsuarioRequestDto request);
        Task DeleteMeAsync();
    }
}
