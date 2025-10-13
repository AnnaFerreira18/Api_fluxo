using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CodigoTemporario
    {
        public Guid IdCodigoTemporario { get; set; }
        public Guid IdUsuario { get; set; }
        public string TipoDeCodigo { get; set; }
        public string? CodigoHash { get; set; }
        public DateTime DataExpiracao { get; set; }
        public DateTime? DataUtilizacao { get; set; }

        // Propriedade de Navegação
        public Usuario Usuario { get; set; }
    }
}
