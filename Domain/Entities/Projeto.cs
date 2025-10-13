using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Projeto
    {
        public Guid IdProjeto { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public Guid IdUsuario { get; set; }
        public bool Deletado { get; set; }
        public DateTime? DataDeletado { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        // Propriedades de Navegação
        public Usuario Usuario { get; set; }
        public ICollection<Anotacao> Anotacoes { get; set; } = new List<Anotacao>();
    }
}
