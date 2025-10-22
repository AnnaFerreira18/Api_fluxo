using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using BCrypt.Net;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly IUserContextService _userContextService;
        private readonly ICodigoTemporarioRepository _codigoRepo;
        private readonly IConfiguration _configuration;

        public UsuarioService(IUsuarioRepository usuarioRepository, ITokenService tokenService, IUserContextService userContextService, ICodigoTemporarioRepository codigoRepo, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _userContextService = userContextService;
            _codigoRepo = codigoRepo;
            _configuration = configuration;
        }

        //public async Task RegistrarAsync(RegistrarUsuarioRequestDto request)
        //{
        //    var usuarioExistente = await _usuarioRepository.GetByEmailAsync(request.Email);
        //    if (usuarioExistente != null)
        //    {
        //        throw new Exception("Este email já está em uso.");
        //    }

        //    string senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

        //    var novoUsuario = new Usuario
        //    {
        //        IdUsuario = Guid.NewGuid(),
        //        Nome = request.Nome,
        //        Email = request.Email,
        //        Senha = senhaHash,
        //        Telefone = request.Telefone,
        //        EmailVerificado = false,
        //        DataCriacao = DateTime.Now,
        //        DataAtualizacao = DateTime.Now
        //    };

        //    await _usuarioRepository.AddAsync(novoUsuario);

        //    var codigo = new Random().Next(100000, 999999).ToString();

        //    Console.WriteLine($"*** CÓDIGO DE VERIFICAÇÃO PARA {novoUsuario.Email}: {codigo} ***");

        //    var codigoHash = BCrypt.Net.BCrypt.HashPassword(codigo);

        //    // 4. Salvar o código no banco
        //    var codigoTemporario = new CodigoTemporario
        //    {
        //        IdCodigoTemporario = Guid.NewGuid(),
        //        IdUsuario = novoUsuario.IdUsuario,
        //        TipoDeCodigo = "VERIFICACAO_EMAIL",
        //        CodigoHash = codigoHash,
        //        DataExpiracao = DateTime.Now.AddMinutes(15) // Código expira em 15 minutos
        //    };

        //    await _codigoRepo.AddAsync(codigoTemporario);
        //}

        public async Task<string> RegistrarAsync(RegistrarUsuarioRequestDto request)
        {
            var usuarioExistente = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuarioExistente != null)
            {
                throw new Exception("Este email já está em uso.");
            }

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

            var novoUsuario = new Usuario
            {
                IdUsuario = Guid.NewGuid(),
                Nome = request.Nome,
                Email = request.Email,
                Senha = senhaHash,
                Telefone = request.Telefone,
                EmailVerificado = false,
                DataCriacao = DateTime.Now,
                DataAtualizacao = DateTime.Now
            };

            await _usuarioRepository.AddAsync(novoUsuario);

            var codigo = new Random().Next(100000, 999999).ToString();

            Console.WriteLine($"*** CÓDIGO DE VERIFICAÇÃO PARA {novoUsuario.Email}: {codigo} ***");

            var codigoHash = BCrypt.Net.BCrypt.HashPassword(codigo);

            // 4. Salvar o código no banco
            var codigoTemporario = new CodigoTemporario
            {
                IdCodigoTemporario = Guid.NewGuid(),
                IdUsuario = novoUsuario.IdUsuario,
                TipoDeCodigo = "VERIFICACAO_EMAIL",
                CodigoHash = codigoHash,
                DataExpiracao = DateTime.Now.AddMinutes(15) // Código expira em 15 minutos
            };

            await _codigoRepo.AddAsync(codigoTemporario);
            return codigo;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

            // 1. Verifica se usuário existe
            if (usuario == null)
            {
                throw new Exception("Email inválido.");
            }

            bool senhaCorreta = BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha);

            if (!senhaCorreta)
            {
                throw new Exception("senha inválida.");
            }

            if (!usuario.EmailVerificado)
            {
                throw new Exception("Email não verificado. Por favor, verifique seu email antes de fazer login.");
            }


            string token = _tokenService.GerarToken(usuario);

            return new LoginResponseDto
            {
                Token = token
            };
        }
        public async Task<UsuarioResponseDto> GetMeAsync()
        {
            var usuarioId = _userContextService.GetUserId();

            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);

            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            var response = new UsuarioResponseDto
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                EmailVerificado = usuario.EmailVerificado,
                DataCriacao = usuario.DataCriacao
            };

            return response;
        }

        public async Task<UsuarioResponseDto> UpdateMeAsync(EditarUsuarioRequestDto request)
        {
            // 1. Descobrir quem é o usuário logado
            var usuarioId = _userContextService.GetUserId();

            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            usuario.Nome = request.Nome;
            usuario.Telefone = request.Telefone;
            usuario.DataAtualizacao = DateTime.Now;


            await _usuarioRepository.UpdateAsync(usuario);

            return new UsuarioResponseDto
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                EmailVerificado = usuario.EmailVerificado,
                DataCriacao = usuario.DataCriacao
            };
        }

        public async Task DeleteMeAsync()
        {
            var usuarioId = _userContextService.GetUserId();

            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            await _usuarioRepository.DeleteAsync(usuario);
        }

        public async Task<bool> EmailJaExisteAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }


            var usuario = await _usuarioRepository.GetByEmailAsync(email);

            return usuario != null;
        }

        public async Task VerificarCodigoAsync(VerificarCodigoRequestDto request)
        {
            // 1. Encontrar o usuário
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuario == null) throw new Exception("Usuário não encontrado.");
            if (usuario.EmailVerificado) throw new Exception("Este email já foi verificado.");

            // 2. Buscar todos os códigos de verificação válidos deste usuário
            var codigosValidos = await _codigoRepo.GetCodigosValidosAsync(usuario.IdUsuario, "VERIFICACAO_EMAIL", DateTime.Now);

            if (!codigosValidos.Any())
            {
                throw new Exception("Código inválido ou expirado.");
            }

            CodigoTemporario codigoCorreto = null;
            foreach (var codigoDb in codigosValidos)
            {
                if (BCrypt.Net.BCrypt.Verify(request.Codigo, codigoDb.CodigoHash))
                {
                    codigoCorreto = codigoDb;
                    break;
                }
            }

            if (codigoCorreto == null)
            {
                throw new Exception("Código inválido ou expirado.");
            }

            // 5.  Marcar o usuário como verificado
            usuario.EmailVerificado = true;
            await _usuarioRepository.UpdateAsync(usuario);

            // 6. Marcar o código como utilizado
            codigoCorreto.DataUtilizacao = DateTime.Now;
            await _codigoRepo.UpdateAsync(codigoCorreto);
        }

        //public async Task SolicitarRedefinicaoSenhaAsync(EsqueciSenhaRequestDto request)
        //{
        //    // 1. Validar se pelo menos um campo foi enviado
        //    if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Telefone))
        //    {
        //        throw new Exception("É necessário fornecer um email ou um telefone para redefinir a senha.");
        //    }

        //    // 2. Encontrar o usuário por email OU telefone
        //    Usuario usuario = null;
        //    if (!string.IsNullOrEmpty(request.Email))
        //    {
        //        usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
        //    }
        //    else if (!string.IsNullOrEmpty(request.Telefone))
        //    {
        //        usuario = await _usuarioRepository.GetByTelefoneAsync(request.Telefone);
        //    }

        //    if (usuario == null)
        //    {
        //        Console.WriteLine($"*** (Simulação) Solicitação de redefinição para {request.Email ?? request.Telefone}, mas o usuário não foi encontrado. Retornando 'OK' por segurança. ***");
        //        return; // Sai do método sem fazer nada.
        //    }

        //    // 4. Se o usuário FOI encontrado, geramos o código
        //    var codigo = new Random().Next(100000, 999999).ToString();
        //    var codigoHash = BCrypt.Net.BCrypt.HashPassword(codigo);

        //    // 5. Salvar o código no banco
        //    var codigoTemporario = new CodigoTemporario
        //    {
        //        IdCodigoTemporario = Guid.NewGuid(),
        //        IdUsuario = usuario.IdUsuario,
        //        TipoDeCodigo = "REDEFINICAO_SENHA", 
        //        CodigoHash = codigoHash,
        //        DataExpiracao = DateTime.Now.AddMinutes(15) // Validade de 15 minutos
        //    };

        //    await _codigoRepo.AddAsync(codigoTemporario);

        //    // 6. (Simulação) "Enviar" o código
        //    Console.WriteLine($"*** CÓDIGO DE REDEFINIÇÃO DE SENHA PARA {usuario.Email}: {codigo} ***");
        //}
        public async Task<string?> SolicitarRedefinicaoSenhaAsync(EsqueciSenhaRequestDto request) 
        {
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Telefone))
            {
                throw new Exception("É necessário fornecer um email ou um telefone para redefinir a senha.");
            }

            // 2. Encontrar o usuário por email OU telefone
            Usuario usuario = null;
            if (!string.IsNullOrEmpty(request.Email))
            {
                usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            }
            else if (!string.IsNullOrEmpty(request.Telefone))
            {
                usuario = await _usuarioRepository.GetByTelefoneAsync(request.Telefone);
            }

            if (usuario == null)
            {
                Console.WriteLine($"*** (Simulação) Solicitação de redefinição para {request.Email ?? request.Telefone}, mas o usuário não foi encontrado. Retornando 'OK' por segurança. ***");
                return null; // Sai do método sem fazer nada.
            }

            // 4. Se o usuário FOI encontrado, geramos o código
            var codigo = new Random().Next(100000, 999999).ToString();
            var codigoHash = BCrypt.Net.BCrypt.HashPassword(codigo);

            var codigoTemporario = new CodigoTemporario
            {
                IdCodigoTemporario = Guid.NewGuid(),
                IdUsuario = usuario.IdUsuario,
                TipoDeCodigo = "REDEFINICAO_SENHA",
                CodigoHash = codigoHash,
                DataExpiracao = DateTime.Now.AddMinutes(15) 
            };
            await _codigoRepo.AddAsync(codigoTemporario);

            Console.WriteLine($"*** CÓDIGO DE REDEFINIÇÃO DE SENHA PARA {usuario.Email}: {codigo} ***");

            return codigo; 
        }
        public async Task<VerificarCodigoRedefinicaoResponseDto> VerificarCodigoRedefinicaoAsync(VerificarCodigoRedefinicaoRequestDto request)
        {
            // 1. Validar e Encontrar Usuário (igual ao método antigo)
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Telefone)) throw new Exception("Email ou telefone necessário.");
            Usuario usuario = null;
            if (!string.IsNullOrEmpty(request.Email)) usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            else usuario = await _usuarioRepository.GetByTelefoneAsync(request.Telefone);
            if (usuario == null) throw new Exception("Código inválido ou expirado.");

            // 2. Validar Código (igual ao método antigo)
            var codigosValidos = await _codigoRepo.GetCodigosValidosAsync(usuario.IdUsuario, "REDEFINICAO_SENHA", DateTime.UtcNow);
            if (!codigosValidos.Any()) throw new Exception("Código inválido ou expirado.");
            CodigoTemporario codigoCorreto = null;
            foreach (var codigoDb in codigosValidos)
            {
                if (BCrypt.Net.BCrypt.Verify(request.Codigo, codigoDb.CodigoHash))
                {
                    codigoCorreto = codigoDb;
                    break;
                }
            }
            if (codigoCorreto == null) throw new Exception("Código inválido ou expirado.");

            // 3.Gerar o Token Temporário
            string tokenTemporario = _tokenService.GerarTokenRedefinicaoSenha(usuario);

            // 4. Marcar o código de 6 dígitos como utilizado
            codigoCorreto.DataUtilizacao = DateTime.Now;
            await _codigoRepo.UpdateAsync(codigoCorreto);

            // 5. Retornar o DTO com o token temporário
            return new VerificarCodigoRedefinicaoResponseDto { TokenTemporario = tokenTemporario };
        }

        // --- NOVO MÉTODO: Redefinir Senha Final ---
        public async Task RedefinirSenhaFinalAsync(string tokenTemporario, RedefinirSenhaFinalRequestDto request)
        {
            try
            {
                // 1. Validar manualmente o Token Temporário
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtKey = _configuration["Jwt:Key"];
                var key = Encoding.ASCII.GetBytes(jwtKey);

                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(tokenTemporario, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // 2. Verificar se o propósito do token está correto
                var purposeClaim = claimsPrincipal.FindFirst("purpose")?.Value;
                if (purposeClaim != "password_reset")
                {
                    throw new SecurityTokenException("Token inválido para esta operação.");
                }

                // 3. Extrair o ID do usuário do token validado
                var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                  claimsPrincipal.FindFirst("sub")?.Value;

                if (!Guid.TryParse(userIdClaim, out Guid usuarioId))
                {

                    throw new Exception("ID do usuário inválido no token.");
                }

                // 4. Buscar o usuário
                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuario == null)
                {
                    throw new Exception("Falha ao redefinir a senha."); 
                }

                // 5. Verificar se a nova senha é igual à antiga
                if (BCrypt.Net.BCrypt.Verify(request.NovaSenha, usuario.Senha))
                {
                    throw new Exception("A nova senha não pode ser igual à senha anterior.");
                }

                // 6. Fazer o hash da nova senha e atualizar
                string novaSenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);

                usuario.Senha = novaSenhaHash;
                usuario.DataAtualizacao = DateTime.UtcNow;
                await _usuarioRepository.UpdateAsync(usuario);

            }
            catch (SecurityTokenException ex)
            {
                throw new Exception($"Token inválido ou expirado: {ex.Message}");
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message); 
            }
        }
    }
}
