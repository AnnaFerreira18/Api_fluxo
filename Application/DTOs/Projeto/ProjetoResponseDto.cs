using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Projeto
{
    public class ProjetoResponseDto
    {
        public Guid IdProjeto { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public Guid IdUsuario { get; set; } 
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataDeletado { get; set; }
    }
}
