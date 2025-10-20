using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class EditarUsuarioRequestDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MaxLength(255)]
        public string Nome { get; set; }

        [MaxLength(20, ErrorMessage = "O telefone não pode ter mais de 20 caracteres.")]
        public string? Telefone { get; set; }
    }
}
