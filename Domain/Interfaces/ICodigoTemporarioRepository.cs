using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICodigoTemporarioRepository
    {
        Task AddAsync(CodigoTemporario codigo);
        Task<List<CodigoTemporario>> GetCodigosValidosAsync(Guid usuarioId, string tipo, DateTime horaAtual);
        Task UpdateAsync(CodigoTemporario codigo);
    }
}
