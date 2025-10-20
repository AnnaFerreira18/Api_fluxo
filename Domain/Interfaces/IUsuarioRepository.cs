using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        // Busca um usuário pelo email
        Task<Usuario> GetByEmailAsync(string email);

        Task AddAsync(Usuario usuario);
        Task<Usuario> GetByIdAsync(Guid id);
        Task UpdateAsync(Usuario usuario);

        Task DeleteAsync(Usuario usuario);
    }
}
