using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Lixeira
{
    public class LixeiraItemDto
    {
        public Guid Id { get; set; } // ID do item (Projeto, Anotação ou Tarefa)
        public string Tipo { get; set; } 
        public string Nome { get; set; } 
        public DateTime DataDeletado { get; set; } 

        // Opcional: Para Anotações, podemos incluir o ID e nome do projeto pai
        public Guid? IdPai { get; set; }
        public string? NomePai { get; set; }
    }
}
