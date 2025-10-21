using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class EsqueciSenhaRequestDto
    {
        [EmailAddress(ErrorMessage = "O email fornecido não é válido.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "O telefone fornecido não é válido.")]
        public string? Telefone { get; set; }
    }
}
