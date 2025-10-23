using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Projeto;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ProjetoService : IProjetoService
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly IUserContextService _userContextService; 

        public ProjetoService(IProjetoRepository projetoRepository, IUserContextService userContextService)
        {
            _projetoRepository = projetoRepository;
            _userContextService = userContextService;
        }

        public async Task<ProjetoResponseDto> CreateAsync(CriarProjetoRequestDto request)
        {
            var usuarioId = _userContextService.GetUserId();


            var novoProjeto = new Projeto
            {
                IdProjeto = Guid.NewGuid(),
                Nome = request.Nome,
                Descricao = request.Descricao,
                IdUsuario = usuarioId, 
                Deletado = false,
                DataCriacao = DateTime.Now,
                DataAtualizacao = DateTime.Now
            };

            await _projetoRepository.AddAsync(novoProjeto);

            var response = new ProjetoResponseDto
            {
                IdProjeto = novoProjeto.IdProjeto,
                Nome = novoProjeto.Nome,
                Descricao = novoProjeto.Descricao,
                IdUsuario = novoProjeto.IdUsuario,
                DataCriacao = novoProjeto.DataCriacao,
                DataAtualizacao = novoProjeto.DataAtualizacao
            };

            return response;
        }

        public async Task<IEnumerable<ProjetoResponseDto>> GetAllAsync()
        {
            var usuarioId = _userContextService.GetUserId();

            var projetos = await _projetoRepository.GetAllByUserIdAsync(usuarioId);

            var responseDtos = projetos.Select(projeto => new ProjetoResponseDto
            {
                IdProjeto = projeto.IdProjeto,
                Nome = projeto.Nome,
                Descricao = projeto.Descricao,
                IdUsuario = projeto.IdUsuario,
                DataCriacao = projeto.DataCriacao,
                DataAtualizacao = projeto.DataAtualizacao
            });

            return responseDtos;
        }

        public async Task<ProjetoResponseDto?> GetByIdAsync(Guid id)
        {
            var usuarioId = _userContextService.GetUserId();

            var projeto = await _projetoRepository.GetByIdAndUserIdAsync(id, usuarioId);

            if (projeto == null)
            {
                return null;
            }

            var responseDto = new ProjetoResponseDto
            {
                IdProjeto = projeto.IdProjeto,
                Nome = projeto.Nome,
                Descricao = projeto.Descricao,
                IdUsuario = projeto.IdUsuario,
                DataCriacao = projeto.DataCriacao,
                DataAtualizacao = projeto.DataAtualizacao
            };

            return responseDto;
        }

        public async Task<ProjetoResponseDto?> UpdateAsync(Guid id, EditarProjetoRequestDto request)
        {
            var usuarioId = _userContextService.GetUserId();

            var projeto = await _projetoRepository.GetByIdAndUserIdAsync(id, usuarioId);

            if (projeto == null)
            {
                return null;
            }

            projeto.Nome = request.Nome;
            projeto.Descricao = request.Descricao;
            projeto.DataAtualizacao = DateTime.Now; 

            await _projetoRepository.UpdateAsync(projeto);

            return new ProjetoResponseDto
            {
                IdProjeto = projeto.IdProjeto,
                Nome = projeto.Nome,
                Descricao = projeto.Descricao,
                IdUsuario = projeto.IdUsuario,
                DataCriacao = projeto.DataCriacao,
                DataAtualizacao = projeto.DataAtualizacao 
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var usuarioId = _userContextService.GetUserId();

            var projeto = await _projetoRepository.GetByIdAndUserIdAsync(id, usuarioId);

            if (projeto == null)
            {
                return false;
            }

            projeto.Deletado = true;
            projeto.DataDeletado = DateTime.Now; 
            projeto.DataAtualizacao = DateTime.Now; 

            await _projetoRepository.UpdateAsync(projeto);

            return true;
        }

        public async Task<bool> RestaurarAsync(Guid id)
        {
            var usuarioId = _userContextService.GetUserId();

            // Usa o novo método para buscar na lixeira
            var projeto = await _projetoRepository.GetByIdEvenIfDeletedAsync(id, usuarioId);

            // Verifica se existe E se está realmente deletado
            if (projeto == null || !projeto.Deletado || !projeto.DataDeletado.HasValue)
            {
                return false; // Não encontrado ou não está na lixeira
            }

            // Verifica o prazo de 3 dias
            var prazoRestauracao = projeto.DataDeletado.Value.AddDays(1);
            if (DateTime.Now > prazoRestauracao)
            {
                // Já passou o prazo, não pode restaurar
                Console.WriteLine($"Tentativa de restaurar projeto {id} fora do prazo."); // Log
                return false;
            }

            // Se chegou aqui, PODE RESTAURAR
            projeto.Deletado = false;
            projeto.DataDeletado = null; // Limpa a data de exclusão
            projeto.DataAtualizacao = DateTime.Now;

            await _projetoRepository.UpdateAsync(projeto);
            return true;
        }

        public async Task<IEnumerable<ProjetoResponseDto>> GetLixeiraAsync()
        {
            var usuarioId = _userContextService.GetUserId();
            var projetosNaLixeira = await _projetoRepository.GetLixeiraByUserIdAsync(usuarioId);

            return projetosNaLixeira.Select(projeto => new ProjetoResponseDto
            {
                IdProjeto = projeto.IdProjeto,
                Nome = projeto.Nome,
                Descricao = projeto.Descricao,
                IdUsuario = projeto.IdUsuario,
                DataCriacao = projeto.DataCriacao,
                DataAtualizacao = projeto.DataAtualizacao,
                DataDeletado = projeto.DataDeletado
            });
        }
    }
}
