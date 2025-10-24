using Application.DTOs.Anotacao;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Authorize] 
    [Route("api/anotacoes")]
    public class AnotacoesController : ControllerBase
    {
        private readonly IAnotacaoService _anotacaoService;

        public AnotacoesController(IAnotacaoService anotacaoService)
        {
            _anotacaoService = anotacaoService;
        }

        [HttpPost]
        [Route("~/api/projetos/{projetoId}/anotacoes")]
        [ProducesResponseType(typeof(AnotacaoResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CriarAnotacao(Guid projetoId, [FromBody] CriarAnotacaoRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var anotacaoCriada = await _anotacaoService.CreateAsync(projetoId, request);

                return CreatedAtAction(nameof(GetAnotacaoPorId),
                                       new { id = anotacaoCriada.IdAnotacao },
                                       anotacaoCriada);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Projeto não encontrado"))
                {
                    return NotFound(new { error = ex.Message });
                }
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AnotacaoResponseDto), 200)] 
        [ProducesResponseType(404)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> GetAnotacaoPorId(Guid id)
        {
            try
            {
                var anotacao = await _anotacaoService.GetByIdAsync(id);

                if (anotacao == null)
                {
                    return NotFound(new { message = "Anotação não encontrada." });
                }

                return Ok(anotacao); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao buscar a anotação: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("~/api/projetos/{projetoId}/anotacoes")] 
        [ProducesResponseType(typeof(IEnumerable<AnotacaoResponseDto>), 200)]
        [ProducesResponseType(404)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> ListarAnotacoesPorProjeto(Guid projetoId)
        {
            try
            {
                var anotacoes = await _anotacaoService.GetAllByProjetoIdAsync(projetoId);
                return Ok(anotacoes);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Projeto não encontrado"))
                {
                    return NotFound(new { error = ex.Message });
                }
                return BadRequest(new { error = $"Ocorreu um erro ao listar as anotações: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AnotacaoResponseDto), 200)] 
        [ProducesResponseType(404)] 
        [ProducesResponseType(400)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> EditarAnotacao(Guid id, [FromBody] EditarAnotacaoRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var anotacaoAtualizada = await _anotacaoService.UpdateAsync(id, request);

                if (anotacaoAtualizada == null)
                {
                    return NotFound(new { message = "Anotação não encontrada." });
                }

                return Ok(anotacaoAtualizada); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao editar a anotação: {ex.Message}" });
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] 
        [ProducesResponseType(404)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> ExcluirAnotacao(Guid id)
        {
            try
            {
                var sucesso = await _anotacaoService.DeleteAsync(id);

                if (!sucesso)
                {
                    return NotFound(new { message = "Anotação não encontrada." });
                }

                return NoContent(); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao excluir a anotação: {ex.Message}" });
            }
        }
        [HttpPatch("{id}/restaurar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RestaurarAnotacao(Guid id)
        {
            try
            {
                var sucesso = await _anotacaoService.RestaurarAsync(id);

                if (!sucesso)
                {
                    return NotFound(new { message = "Anotação não encontrada na lixeira ou prazo para restauração expirado." });
                }

                return Ok(new { message = "Anotação restaurada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao restaurar a anotação: {ex.Message}" });
            }
        }

        [HttpGet("lixeira")]
        [ProducesResponseType(typeof(IEnumerable<AnotacaoResponseDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ListarLixeira()
        {
            try
            {
                var anotacoes = await _anotacaoService.GetLixeiraAsync();
                return Ok(anotacoes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao listar a lixeira de anotações: {ex.Message}" });
            }
        }

        [HttpPatch("{id}/favoritar")]
        [ProducesResponseType(typeof(AnotacaoResponseDto), 200)] 
        [ProducesResponseType(404)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> AlternarFavorito(Guid id)
        {
            try
            {
                var anotacaoAtualizada = await _anotacaoService.AlternarFavoritoAsync(id);

                if (anotacaoAtualizada == null)
                {
                    return NotFound(new { message = "Anotação não encontrada." });
                }

                return Ok(anotacaoAtualizada);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao favoritar/desfavoritar a anotação: {ex.Message}" });
            }
        }

        [HttpPatch("~/api/projetos/{projetoId}/anotacoes/ordenar")]
        [ProducesResponseType(204)] 
        [ProducesResponseType(400)] 
        [ProducesResponseType(404)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> OrdenarAnotacoes(Guid projetoId, [FromBody] OrdenarAnotacoesRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _anotacaoService.OrdenarAsync(projetoId, request);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Projeto não encontrado"))
                {
                    return NotFound(new { error = ex.Message });
                }
                return BadRequest(new { error = $"Ocorreu um erro ao ordenar as anotações: {ex.Message}" });
            }
        }
    }
}
