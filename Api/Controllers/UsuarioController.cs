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

                return StatusCode(201, new { message = "Usuário registrado com sucesso! Verifique seu email para o código de ativação." });
            }
            catch (Exception ex)
            {
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
                if (ex.Message.StartsWith("Conta bloqueada"))
                {
                    return StatusCode(429, new { error = ex.Message }); 
                }

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

        [HttpGet("verificar-email")] 
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> VerificarEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { error = "O email é obrigatório." });
            }

            try
            {
                var emailExiste = await _usuarioService.EmailJaExisteAsync(email);

                // Retornamos 200 OK com o booleano (true ou false) no corpo da resposta
                return Ok(emailExiste);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocorreu um erro interno ao verificar o email." });
            }
        }

        [HttpPost("verificar-codigo")]
        public async Task<IActionResult> VerificarCodigo([FromBody] VerificarCodigoRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _usuarioService.VerificarCodigoAsync(request);
                return Ok(new { message = "Email verificado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("esqueci-senha")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var codigoGerado = await _usuarioService.SolicitarRedefinicaoSenhaAsync(request);

                var responseMessage = "Se uma conta com este email ou telefone existir, um código de redefinição foi enviado.";

                if (codigoGerado != null) 
                {
                    return Ok(new
                    {
                        message = responseMessage + " (Código retornado apenas para DEV via SMS)",
                        codigoRedefinicao = codigoGerado
                    });
                }
                else 
                {
                    return Ok(new { message = responseMessage });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("verificar-codigo-redefinicao")]
        public async Task<IActionResult> VerificarCodigoRedefinicao([FromBody] VerificarCodigoRedefinicaoRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var response = await _usuarioService.VerificarCodigoRedefinicaoAsync(request);
                return Ok(response); // Retorna 200 OK com o token temporário
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message }); 
            }
        }


        [HttpPost("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenhaFinal([FromBody] RedefinirSenhaFinalRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Extrair o token temporário do cabeçalho Authorization
            string authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { error = "Token temporário não fornecido ou inválido." });
            }
            string tokenTemporario = authorizationHeader.Substring("Bearer ".Length).Trim();

            try
            {
                await _usuarioService.RedefinirSenhaFinalAsync(tokenTemporario, request);
                return Ok(new { message = "Senha redefinida com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("reenviar-codigo-verificacao")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ReenviarCodigoVerificacao([FromBody] ReenviarCodigoRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _usuarioService.ReenviarCodigoVerificacaoAsync(request);

                var responseMessage = "Se uma conta não verificada com este email existir, um novo código foi enviado.";

                return Ok(new { message = responseMessage });
            }
            catch (Exception ex)
            {
                // Captura o erro "Este email já foi verificado."
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}