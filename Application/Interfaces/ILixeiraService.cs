using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Lixeira;

namespace Application.Interfaces
{
    public interface ILixeiraService
    {
        Task<IEnumerable<LixeiraItemDto>> GetItensAsync();
    }
}
