using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Anotacao
{
    public class OrdenarAnotacoesRequestDto
    {
        [Required]
        [MinLength(0)] 
        public List<Guid> IdsAnotacoesOrdenadas { get; set; }
    }
}
