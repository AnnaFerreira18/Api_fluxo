using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tarefa
{
    public class OrdenarTarefasRequestDto
    {
        [Required]
        [MinLength(0)]
        public List<Guid> IdsTarefasOrdenadas { get; set; }
    }
}
