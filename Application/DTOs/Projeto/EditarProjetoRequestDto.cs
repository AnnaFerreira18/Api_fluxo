using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Projeto
{
    public class EditarProjetoRequestDto
    {
        [Required(ErrorMessage = "O nome do projeto é obrigatório.")]
        [MaxLength(255)]
        public string Nome { get; set; }

        public string? Descricao { get; set; }
    }
}
