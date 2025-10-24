using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Anotacao;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class AnotacaoService : IAnotacaoService
    {
        private readonly IAnotacaoRepository _anotacaoRepository;
        private readonly IProjetoRepository _projetoRepository; 
        private readonly IUserContextService _userContextService;

        public AnotacaoService(
            IAnotacaoRepository anotacaoRepository,
            IProjetoRepository projetoRepository,
            IUserContextService userContextService)
        {
            _anotacaoRepository = anotacaoRepository;
            _projetoRepository = projetoRepository; 
            _userContextService = userContextService;
        }

        public async Task<AnotacaoResponseDto> CreateAsync(Guid projetoId, CriarAnotacaoRequestDto request)
        {
            var usuarioId = _userContextService.GetUserId();

            var projeto = await _projetoRepository.GetByIdAndUserIdAsync(projetoId, usuarioId);
            if (projeto == null)
            {
                throw new Exception("Projeto não encontrado ou você não tem permissão para acessá-lo.");
            }

            var novaAnotacao = new Anotacao
            {
                IdAnotacao = Guid.NewGuid(),
                Titulo = request.Titulo,
                Conteudo = request.Conteudo,
                IdProjeto = projetoId, 
                Favorito = false, 
                Ordem = 0,        
                Deletado = false,
                DataCriacao = DateTime.Now,
                DataAtualizacao = DateTime.Now
            };

            await _anotacaoRepository.AddAsync(novaAnotacao);

            return new AnotacaoResponseDto
            {
                IdAnotacao = novaAnotacao.IdAnotacao,
                Titulo = novaAnotacao.Titulo,
                Conteudo = novaAnotacao.Conteudo,
                IdProjeto = novaAnotacao.IdProjeto,
                Favorito = novaAnotacao.Favorito,
                Ordem = novaAnotacao.Ordem,
                DataCriacao = novaAnotacao.DataCriacao,
                DataAtualizacao = novaAnotacao.DataAtualizacao
            };
        }

        public async Task<IEnumerable<AnotacaoResponseDto>> GetAllByProjetoIdAsync(Guid projetoId)
        {
            var usuarioId = _userContextService.GetUserId();

            var projeto = await _projetoRepository.GetByIdAndUserIdAsync(projetoId, usuarioId);
            if (projeto == null)
            {
                throw new Exception("Projeto não encontrado ou você não tem permissão para acessá-lo.");
            }

            var anotacoes = await _anotacaoRepository.GetAllByProjetoIdAsync(projetoId);

            var responseDtos = anotacoes.Select(anotacao => new AnotacaoResponseDto
            {
                IdAnotacao = anotacao.IdAnotacao,
                Titulo = anotacao.Titulo,
                Conteudo = anotacao.Conteudo,
                IdProjeto = anotacao.IdProjeto,
                Favorito = anotacao.Favorito,
                Ordem = anotacao.Ordem,
                DataCriacao = anotacao.DataCriacao,
                DataAtualizacao = anotacao.DataAtualizacao
            });

            return responseDtos;
        }

        public async Task<AnotacaoResponseDto?> GetByIdAsync(Guid id)
        {
            var usuarioId = _userContextService.GetUserId();

            var anotacao = await _anotacaoRepository.GetByIdAsync(id);

            if (anotacao == null || anotacao.Projeto?.IdUsuario != usuarioId)
            {
                return null;
            }

            var responseDto = new AnotacaoResponseDto
            {
                IdAnotacao = anotacao.IdAnotacao,
                Titulo = anotacao.Titulo,
                Conteudo = anotacao.Conteudo,
                IdProjeto = anotacao.IdProjeto,
                Favorito = anotacao.Favorito,
                Ordem = anotacao.Ordem,
                DataCriacao = anotacao.DataCriacao,
                DataAtualizacao = anotacao.DataAtualizacao
            };

            return responseDto;
        }

        public async Task<AnotacaoResponseDto?> UpdateAsync(Guid id, EditarAnotacaoRequestDto request)
        {
            var usuarioId = _userContextService.GetUserId();

            var anotacao = await _anotacaoRepository.GetByIdAsync(id);

            if (anotacao == null || anotacao.Projeto?.IdUsuario != usuarioId)
            {
                return null; 
            }

            anotacao.Titulo = request.Titulo;
            anotacao.Conteudo = request.Conteudo;
            anotacao.DataAtualizacao = DateTime.Now;

            await _anotacaoRepository.UpdateAsync(anotacao);

            return new AnotacaoResponseDto
            {
                IdAnotacao = anotacao.IdAnotacao,
                Titulo = anotacao.Titulo,
                Conteudo = anotacao.Conteudo,
                IdProjeto = anotacao.IdProjeto,
                Favorito = anotacao.Favorito,
                Ordem = anotacao.Ordem,
                DataCriacao = anotacao.DataCriacao,
                DataAtualizacao = anotacao.DataAtualizacao 
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var usuarioId = _userContextService.GetUserId();

            var anotacao = await _anotacaoRepository.GetByIdAsync(id);

            if (anotacao == null || anotacao.Projeto?.IdUsuario != usuarioId)
            {
                return false; 
            }

            anotacao.Deletado = true;
            anotacao.DataDeletado = DateTime.Now;
            anotacao.DataAtualizacao = DateTime.Now;

            await _anotacaoRepository.UpdateAsync(anotacao);

            return true;
        }

        public async Task<bool> RestaurarAsync(Guid id)
        {
            var usuarioId = _userContextService.GetUserId();

            var anotacao = await _anotacaoRepository.GetByIdEvenIfDeletedAsync(id);

            if (anotacao == null || !anotacao.Deletado || !anotacao.DataDeletado.HasValue || anotacao.Projeto?.IdUsuario != usuarioId)
            {
                return false; 
            }

            var prazoRestauracao = anotacao.DataDeletado.Value.AddDays(1);
            if (DateTime.Now > prazoRestauracao)
            {
                Console.WriteLine($"Tentativa de restaurar anotação {id} fora do prazo."); 
                return false; 
            }

            // Se chegou aqui, PODE RESTAURAR
            anotacao.Deletado = false;
            anotacao.DataDeletado = null;
            anotacao.DataAtualizacao = DateTime.UtcNow;

            await _anotacaoRepository.UpdateAsync(anotacao);
            return true;
        }

        public async Task<IEnumerable<AnotacaoResponseDto>> GetLixeiraAsync()
        {
            var usuarioId = _userContextService.GetUserId();
            var anotacoesNaLixeira = await _anotacaoRepository.GetLixeiraByUserIdAsync(usuarioId);

            return anotacoesNaLixeira.Select(anotacao => new AnotacaoResponseDto
            {
                IdAnotacao = anotacao.IdAnotacao,
                Titulo = anotacao.Titulo,
                Conteudo = anotacao.Conteudo, 
                IdProjeto = anotacao.IdProjeto,
                Favorito = anotacao.Favorito,
                Ordem = anotacao.Ordem,
                DataCriacao = anotacao.DataCriacao,
                DataAtualizacao = anotacao.DataAtualizacao,
                DataDeletado = anotacao.DataDeletado
            });
        }
        public async Task<AnotacaoResponseDto?> AlternarFavoritoAsync(Guid id)
        {

            var usuarioId = _userContextService.GetUserId();

            var anotacao = await _anotacaoRepository.GetByIdAsync(id);

            if (anotacao == null || anotacao.Projeto?.IdUsuario != usuarioId)
            {
                return null; 
            }

            anotacao.Favorito = !anotacao.Favorito; 
            anotacao.DataAtualizacao = DateTime.Now;

            await _anotacaoRepository.UpdateAsync(anotacao);

            return new AnotacaoResponseDto
            {
                IdAnotacao = anotacao.IdAnotacao,
                Titulo = anotacao.Titulo,
                Conteudo = anotacao.Conteudo,
                IdProjeto = anotacao.IdProjeto,
                Favorito = anotacao.Favorito, 
                Ordem = anotacao.Ordem,
                DataCriacao = anotacao.DataCriacao,
                DataAtualizacao = anotacao.DataAtualizacao
            };
        }

        public async Task OrdenarAsync(Guid projetoId, OrdenarAnotacoesRequestDto request)
        {
            var usuarioId = _userContextService.GetUserId();
            var projeto = await _projetoRepository.GetByIdAndUserIdAsync(projetoId, usuarioId);
            if (projeto == null)
            {
                throw new Exception("Projeto não encontrado ou você não tem permissão.");
            }

            var anotacoesAtuais = (await _anotacaoRepository.GetAllByProjetoIdAsync(projetoId)).ToList();

            var idsRecebidos = request.IdsAnotacoesOrdenadas;
            var idsAtuais = anotacoesAtuais.Select(a => a.IdAnotacao).ToHashSet();

            if (idsRecebidos.Any(id => !idsAtuais.Contains(id)))
            {
                throw new Exception("Um ou mais IDs de anotação fornecidos são inválidos ou não pertencem a este projeto.");
            }
            // Verifica se a contagem de IDs bate (todos os IDs atuais devem estar na lista recebida)
            if (idsRecebidos.Count != idsAtuais.Count)
            {
                throw new Exception("A lista de IDs fornecida não corresponde ao número de anotações do projeto.");
            }

            var anotacoesDict = anotacoesAtuais.ToDictionary(a => a.IdAnotacao);

            for (int i = 0; i < idsRecebidos.Count; i++)
            {
                var anotacaoId = idsRecebidos[i];
                var anotacao = anotacoesDict[anotacaoId];

                if (anotacao.Ordem != i)
                {
                    anotacao.Ordem = i; 
                    anotacao.DataAtualizacao = DateTime.Now;
                }
            }

            await _anotacaoRepository.SaveChangesAsync();
        }
    }
}
