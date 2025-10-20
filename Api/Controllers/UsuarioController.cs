using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // Endpoint: POST /api/usuarios/registrar
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _usuarioService.RegistrarAsync(request);

                // Idealmente, retornaria o usuário criado ou um link,
                return StatusCode(201, new { message = "Usuário registrado com sucesso!" });
            }
            catch (Exception ex)
            {
                // Se o serviço lançar uma exceção (ex: email duplicado),
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _usuarioService.LoginAsync(request);
                return Ok(response); // Retorna 200 OK com o token
            }
            catch (Exception ex)
            {

                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var usuario = await _usuarioService.GetMeAsync();
                return Ok(usuario); // Retorna 200 OK com os dados do usuário
            }
            catch (Exception ex)
            {
                // Se o token for inválido, o [Authorize] já retorna 401
                // Este 'catch' é para erros como "Usuário não encontrado"
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMe([FromBody] EditarUsuarioRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var usuarioAtualizado = await _usuarioService.UpdateMeAsync(request);
                return Ok(usuarioAtualizado); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("me")]
        [Authorize] 
        public async Task<IActionResult> DeleteMe()
        {
            try
            {
                await _usuarioService.DeleteMeAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Se o banco de dados impedir o delete (ex: foreign key),
                // o erro será capturado aqui.
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}