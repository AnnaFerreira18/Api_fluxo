using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RegistrarUsuarioRequestDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MaxLength(255)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email não é válido.")]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [MaxLength(20, ErrorMessage = "O telefone não pode ter mais de 20 caracteres.")]
        public string Telefone { get; set; }
    }
}
