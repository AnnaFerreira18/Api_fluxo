using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tarefa
{
    public class CriarTarefaRequestDto
    {
        [Required(ErrorMessage = "O texto da tarefa é obrigatório.")]
        [MaxLength(500)]
        public string Texto { get; set; }
    }
}
