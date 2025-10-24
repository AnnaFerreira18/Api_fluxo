using Application.DTOs.Lixeira;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    [Authorize] 
    public class LixeiraController : ControllerBase
    {
        private readonly ILixeiraService _lixeiraService;

        public LixeiraController(ILixeiraService lixeiraService)
        {
            _lixeiraService = lixeiraService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LixeiraItemDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ListarItensLixeira()
        {
            try
            {
                var itens = await _lixeiraService.GetItensAsync();
                return Ok(itens);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ocorreu um erro ao listar a lixeira: {ex.Message}" });
            }
        }

        // Poderíamos adicionar um endpoint DELETE /api/lixeira/{id}?tipo={tipo} para exclusão permanente no futuro
    }
}
