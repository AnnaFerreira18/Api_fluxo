using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Anotacao
{
    public class CriarAnotacaoRequestDto
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        [MaxLength(255)]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O conteúdo é obrigatório.")]
        public string Conteudo { get; set; }

    }
}
