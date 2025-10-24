using Application.DTOs.Tarefa;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    [Authorize] 
    public class TarefasController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;

        public TarefasController(ITarefaService tarefaService)
        {
            _tarefaService = tarefaService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TarefaResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CriarTarefa([FromBody] CriarTarefaRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tarefaCriada = await _tarefaService.CreateAsync(request);

                return CreatedAtAction(nameof(GetTarefaPorId),
                                       new { id = tarefaCriada.IdTarefa },
                                       tarefaCriada);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao criar a tarefa: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TarefaResponseDto), 200)] 
        [ProducesResponseType(404)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> GetTarefaPorId(Guid id)
        {
            try
            {
                var tarefa = await _tarefaService.GetByIdAsync(id);

                if (tarefa == null)
                {
                    return NotFound(new { message = "Tarefa não encontrada." });
                }

                return Ok(tarefa); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao buscar a tarefa: {ex.Message}" });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), 200)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> ListarTarefas()
        {
            try
            {
                var tarefas = await _tarefaService.GetAllAsync();
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao listar as tarefas: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TarefaResponseDto), 200)] 
        [ProducesResponseType(404)]
        [ProducesResponseType(400)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> EditarTarefa(Guid id, [FromBody] EditarTarefaRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tarefaAtualizada = await _tarefaService.UpdateAsync(id, request);

                if (tarefaAtualizada == null)
                {
                    return NotFound(new { message = "Tarefa não encontrada." });
                }

                return Ok(tarefaAtualizada); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao editar a tarefa: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)] 
        [ProducesResponseType(404)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> ExcluirTarefa(Guid id)
        {
            try
            {
                var sucesso = await _tarefaService.DeleteAsync(id);

                if (!sucesso)
                {
                    return NotFound(new { message = "Tarefa não encontrada." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao excluir a tarefa: {ex.Message}" });
            }
        }

        [HttpPatch("{id}/restaurar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RestaurarTarefa(Guid id)
        {
            try
            {
                var sucesso = await _tarefaService.RestaurarAsync(id);

                if (!sucesso)
                {
                    return NotFound(new { message = "Tarefa não encontrada na lixeira ou prazo para restauração expirado." });
                }

                return Ok(new { message = "Tarefa restaurada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao restaurar a tarefa: {ex.Message}" });
            }
        }
        [HttpGet("lixeira")]
        [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ListarLixeira()
        {
            try
            {
                var tarefas = await _tarefaService.GetLixeiraAsync();
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao listar a lixeira de tarefas: {ex.Message}" });
            }
        }

        [HttpPatch("{id}/concluir")]
        [ProducesResponseType(typeof(TarefaResponseDto), 200)]
        [ProducesResponseType(404)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> AlternarConcluido(Guid id)
        {
            try
            {
                var tarefaAtualizada = await _tarefaService.AlternarConcluidoAsync(id);

                if (tarefaAtualizada == null)
                {
                    return NotFound(new { message = "Tarefa não encontrada." });
                }

                return Ok(tarefaAtualizada);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao concluir/reabrir a tarefa: {ex.Message}" });
            }
        }

        [HttpPatch("ordenar")]
        [ProducesResponseType(204)] 
        [ProducesResponseType(400)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> OrdenarTarefas([FromBody] OrdenarTarefasRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _tarefaService.OrdenarAsync(request);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao ordenar as tarefas: {ex.Message}" });
            }
        }
    }
}
