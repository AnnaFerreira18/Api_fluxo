using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class VerificarCodigoRedefinicaoRequestDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? Telefone { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Codigo { get; set; }
    }
}
