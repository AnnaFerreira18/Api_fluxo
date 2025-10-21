using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RedefinirSenhaFinalRequestDto
    {
        [Required]
        [MinLength(6)]
        public string NovaSenha { get; set; }
    }
}
