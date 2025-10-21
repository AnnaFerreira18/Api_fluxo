using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RedefinirSenhaRequestDto
    {

        [EmailAddress]
        public string? Email { get; set; }

        public string? Telefone { get; set; }

        [Required(ErrorMessage = "O código é obrigatório.")]
        [StringLength(6, MinimumLength = 6)]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string NovaSenha { get; set; }
    }
}
