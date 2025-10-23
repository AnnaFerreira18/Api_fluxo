using Application.DTOs.Projeto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    [Authorize]
    public class ProjetosController : ControllerBase
    {
        private readonly IProjetoService _projetoService;

        public ProjetosController(IProjetoService projetoService)
        {
            _projetoService = projetoService;
        }


        [HttpPost]
        [ProducesResponseType(typeof(ProjetoResponseDto), 201)] 
        [ProducesResponseType(400)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> CriarProjeto([FromBody] CriarProjetoRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var projetoCriado = await _projetoService.CreateAsync(request);


                return CreatedAtAction(nameof(GetProjetoPorId),
                                       new { id = projetoCriado.IdProjeto },
                                       projetoCriado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjetoResponseDto), 200)] 
        [ProducesResponseType(404)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> GetProjetoPorId(Guid id)
        {
            try
            {
                var projeto = await _projetoService.GetByIdAsync(id);

                if (projeto == null)
                {

                    return NotFound(new { message = "Projeto não encontrado." });
                }

                return Ok(projeto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao buscar o projeto: {ex.Message}" });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProjetoResponseDto>), 200)] 
        [ProducesResponseType(401)]
        public async Task<IActionResult> ListarProjetos()
        {
            try
            {
                var projetos = await _projetoService.GetAllAsync();
                return Ok(projetos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao listar os projetos: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjetoResponseDto), 200)] 
        [ProducesResponseType(404)] 
        [ProducesResponseType(400)] 
        [ProducesResponseType(401)]
        public async Task<IActionResult> EditarProjeto(Guid id, [FromBody] EditarProjetoRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var projetoAtualizado = await _projetoService.UpdateAsync(id, request);

                if (projetoAtualizado == null)
                {
                    return NotFound(new { message = "Projeto não encontrado." });
                }

                return Ok(projetoAtualizado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao editar o projeto: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)] 
        [ProducesResponseType(404)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> ExcluirProjeto(Guid id)
        {
            try
            {
                var sucesso = await _projetoService.DeleteAsync(id);

                if (!sucesso)
                {
                    return NotFound(new { message = "Projeto não encontrado." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao excluir o projeto: {ex.Message}" });
            }
        }

        [HttpPatch("{id}/restaurar")]
        [ProducesResponseType(200)] 
        [ProducesResponseType(404)] 
        [ProducesResponseType(401)] 
        public async Task<IActionResult> RestaurarProjeto(Guid id)
        {
            try
            {
                var sucesso = await _projetoService.RestaurarAsync(id);

                if (!sucesso)
                {
                    // Se o serviço retornou false, projeto não encontrado, não deletado ou prazo expirado
                    return NotFound(new { message = "Projeto não encontrado na lixeira ou prazo para restauração expirado." });
                }

                return Ok(new { message = "Projeto restaurado com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao restaurar o projeto: {ex.Message}" });
            }
        }

        [HttpGet("lixeira")]
        [ProducesResponseType(typeof(IEnumerable<ProjetoResponseDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ListarLixeira()
        {
            try
            {
                var projetos = await _projetoService.GetLixeiraAsync();
                return Ok(projetos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao listar a lixeira: {ex.Message}" });
            }
        }
    }

}

