using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Usuario
    {
        public Guid IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Senha { get; set; }
        public bool EmailVerificado { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public int ContadorFalhasLogin { get; set; }
        public DateTime? DataBloqueioTemporario { get; set; }

        public ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
        public ICollection<Tarefa> Tarefas { get; set; } = new List<Tarefa>();
        public ICollection<CodigoTemporario> CodigosTemporarios { get; set; } = new List<CodigoTemporario>();
    }
}
