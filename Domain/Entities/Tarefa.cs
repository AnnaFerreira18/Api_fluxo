using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Tarefa
    {
        public Guid IdTarefa { get; set; }
        public Guid IdUsuario { get; set; }
        public string Texto { get; set; }
        public bool Concluido { get; set; }
        public int Ordem { get; set; }
        public bool Deletado { get; set; }
        public DateTime? DataDeletado { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        // Propriedade de Navegação
        public Usuario Usuario { get; set; }
    }
}
