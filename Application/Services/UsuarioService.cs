using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using BCrypt.Net; // Este 'using' ainda é útil

namespace Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly IUserContextService _userContextService;

        public UsuarioService(IUsuarioRepository usuarioRepository, ITokenService tokenService, IUserContextService userContextService)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _userContextService = userContextService;
        }

        public async Task RegistrarAsync(RegistrarUsuarioRequestDto request)
        {
            var usuarioExistente = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuarioExistente != null)
            {
                throw new Exception("Este email já está em uso.");
            }

            // --- CORREÇÃO AQUI ---
            // Precisamos de usar o nome completo da classe estática: BCrypt.Net.BCrypt
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

            var novoUsuario = new Usuario
            {
                IdUsuario = Guid.NewGuid(),
                Nome = request.Nome,
                Email = request.Email,
                Senha = senhaHash,
                Telefone = request.Telefone,
                EmailVerificado = false,
                UltimoLogin = null,
                DataCriacao = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };

            await _usuarioRepository.AddAsync(novoUsuario);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

            if (usuario == null)
            {
                throw new Exception("Email inválido.");
            }

            // --- CORREÇÃO AQUI ---
            // O mesmo aqui, usamos o nome completo: BCrypt.Net.BCrypt
            bool senhaCorreta = BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha);

            if (!senhaCorreta)
            {
                throw new Exception("Senha inválida.");
            }

            string token = _tokenService.GerarToken(usuario);

            return new LoginResponseDto
            {
                Token = token
            };
        }
        public async Task<UsuarioResponseDto> GetMeAsync()
        {
            // 1. Descobrir quem é o usuário logado lendo o "crachá" (token)
            var usuarioId = _userContextService.GetUserId();

            // 2. Buscar o usuário no banco de dados
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);

            if (usuario == null)
            {
                // Isto pode acontecer se o usuário foi deletado mas o token ainda é válido
                throw new Exception("Usuário não encontrado.");
            }

            // 3. Mapear a Entidade para o DTO de Resposta (para não enviar a senha)
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
    }
}