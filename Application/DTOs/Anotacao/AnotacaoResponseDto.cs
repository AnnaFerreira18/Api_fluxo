using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Anotacao
{
    public class AnotacaoResponseDto
    {
        public Guid IdAnotacao { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public Guid IdProjeto { get; set; }
        public bool Favorito { get; set; }
        public int Ordem { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataDeletado { get; set; }
    }
}
