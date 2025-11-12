using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Tarefa;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly ITarefaRepository _tarefaRepository;
        private readonly IUserContextService _userContextService;

        public TarefaService(ITarefaRepository tarefaRepository, IUserContextService userContextService)
        {
            _tarefaRepository = tarefaRepository;
            _userContextService = userContextService;
        }

        public async Task<TarefaResponseDto> CreateAsync(CriarTarefaRequestDto request)
        {
            var usuarioId = _userContextService.GetUserId();

            var novaTarefa = new Tarefa
            {
                IdTarefa = Guid.NewGuid(),
                IdUsuario = usuarioId,
                Texto = request.Texto,
                Concluido = false,
                Ordem = 0,
                Deletado = false,
                DataCriacao = DateTime.Now,
                DataAtualizacao = DateTime.Now
            };

            await _tarefaRepository.AddAsync(novaTarefa);

            return new TarefaResponseDto
            {
                IdTarefa = novaTarefa.IdTarefa,
                IdUsuario = novaTarefa.IdUsuario,
                Texto = novaTarefa.Texto,
                Concluido = novaTarefa.Concluido,
                Ordem = novaTarefa.Ordem,
                DataCriacao = novaTarefa.DataCriacao,
                DataAtualizacao = novaTarefa.DataAtualizacao
            };
        }

        public async Task<IEnumerable<TarefaResponseDto>> GetAllAsync()
        {
            var usuarioId = _userContextService.GetUserId();

            var tarefas = await _tarefaRepository.GetAllByUserIdAsync(usuarioId);

            var responseDtos = tarefas.Select(tarefa => new TarefaResponseDto
            {
                IdTarefa = tarefa.IdTarefa,
                IdUsuario = tarefa.IdUsuario,
                Texto = tarefa.Texto,
                Concluido = tarefa.Concluido,
                Ordem = tarefa.Ordem,
                DataCriacao = tarefa.DataCriacao,
                DataAtualizacao = tarefa.DataAtualizacao
            });

            return responseDtos;
        }

        public async Task<TarefaResponseDto?> GetByIdAsync(Guid id)
        {
            var usuarioId = _userContextService.GetUserId();

            var tarefa = await _tarefaRepository.GetByIdAndUserIdAsync(id, usuarioId);

            if (tarefa == null)
            {
                return null;
            }

            return new TarefaResponseDto
            {
                IdTarefa = tarefa.IdTarefa,
                IdUsuario = tarefa.IdUsuario,
                Texto = tarefa.Texto,
                Concluido = tarefa.Concluido,
                Ordem = tarefa.Ordem,
                DataCriacao = tarefa.DataCriacao,
                DataAtualizacao = tarefa.DataAtualizacao
            };
        }

        public async Task<TarefaResponseDto?> UpdateAsync(Guid id, EditarTarefaRequestDto request)
        {
            var usuarioId = _userContextService.GetUserId();
            var tarefa = await _tarefaRepository.GetByIdAndUserIdAsync(id, usuarioId);

            if (tarefa == null)
            {
                return null;
            }

            tarefa.Texto = request.Texto;
            tarefa.DataAtualizacao = DateTime.Now;

            await _tarefaRepository.UpdateAsync(tarefa);

            return new TarefaResponseDto
            {
                IdTarefa = tarefa.IdTarefa,
                IdUsuario = tarefa.IdUsuario,
                Texto = tarefa.Texto,
                Concluido = tarefa.Concluido,
                Ordem = tarefa.Ordem,
                DataCriacao = tarefa.DataCriacao,
                DataAtualizacao = tarefa.DataAtualizacao
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var usuarioId = _userContextService.GetUserId();

            var tarefa = await _tarefaRepository.GetByIdAndUserIdAsync(id, usuarioId);

            if (tarefa == null)
            {
                return false;
            }

            tarefa.Deletado = true;
            tarefa.DataDeletado = DateTime.Now;
            tarefa.DataAtualizacao = DateTime.Now;

            await _tarefaRepository.UpdateAsync(tarefa);

            // 2. ADICIONA O SALVAMENTO QUE FALTAVA
            await _tarefaRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RestaurarAsync(Guid id)
        {
            var usuarioId = _userContextService.GetUserId();

            var tarefa = await _tarefaRepository.GetByIdEvenIfDeletedAsync(id, usuarioId);

            if (tarefa == null || !tarefa.Deletado || !tarefa.DataDeletado.HasValue)
            {
                return false; 
            }

            var prazoRestauracao = tarefa.DataDeletado.Value.AddDays(3);
            if (DateTime.Now > prazoRestauracao)
            {
                Console.WriteLine($"Tentativa de restaurar tarefa {id} fora do prazo."); 
                return false; 
            }

            tarefa.Deletado = false;
            tarefa.DataDeletado = null; 
            tarefa.DataAtualizacao = DateTime.Now;

            await _tarefaRepository.UpdateAsync(tarefa);
            await _tarefaRepository.SaveChangesAsync();

            return true;
        }
        public async Task<IEnumerable<TarefaResponseDto>> GetLixeiraAsync()
        {
            var usuarioId = _userContextService.GetUserId();
            var tarefasNaLixeira = await _tarefaRepository.GetLixeiraByUserIdAsync(usuarioId);


            return tarefasNaLixeira.Select(tarefa => new TarefaResponseDto
            {
                IdTarefa = tarefa.IdTarefa,
                IdUsuario = tarefa.IdUsuario,
                Texto = tarefa.Texto,
                Concluido = tarefa.Concluido,
                Ordem = tarefa.Ordem,
                DataCriacao = tarefa.DataCriacao,
                DataAtualizacao = tarefa.DataAtualizacao,
                DataDeletado = tarefa.DataDeletado
            });
        }

        public async Task<TarefaResponseDto?> AlternarConcluidoAsync(Guid id)
        {
            var usuarioId = _userContextService.GetUserId();

            var tarefa = await _tarefaRepository.GetByIdAndUserIdAsync(id, usuarioId);

            if (tarefa == null)
            {
                return null; 
            }

            tarefa.Concluido = !tarefa.Concluido; 
            tarefa.DataAtualizacao = DateTime.Now;

            await _tarefaRepository.UpdateAsync(tarefa);

            return new TarefaResponseDto
            {
                IdTarefa = tarefa.IdTarefa,
                IdUsuario = tarefa.IdUsuario,
                Texto = tarefa.Texto,
                Concluido = tarefa.Concluido, 
                Ordem = tarefa.Ordem,
                DataCriacao = tarefa.DataCriacao,
                DataAtualizacao = tarefa.DataAtualizacao
            };
        }

        public async Task OrdenarAsync(OrdenarTarefasRequestDto request)
        {
            var usuarioId = _userContextService.GetUserId();

            var tarefasAtuais = (await _tarefaRepository.GetAllByUserIdAsync(usuarioId)).ToList();

            var idsRecebidos = request.IdsTarefasOrdenadas;
            var idsAtuais = tarefasAtuais.Select(t => t.IdTarefa).ToHashSet();

            if (idsRecebidos.Any(id => !idsAtuais.Contains(id)))
            {
                throw new Exception("Um ou mais IDs de tarefa fornecidos são inválidos ou não pertencem a este usuário.");
            }
            if (idsRecebidos.Count != idsAtuais.Count)
            {
                throw new Exception("A lista de IDs fornecida não corresponde ao número de tarefas do usuário.");
            }

            var tarefasDict = tarefasAtuais.ToDictionary(t => t.IdTarefa);
            bool algumaOrdemMudou = false;

            for (int i = 0; i < idsRecebidos.Count; i++)
            {
                var tarefaId = idsRecebidos[i];
                var tarefa = tarefasDict[tarefaId];

                if (tarefa.Ordem != i)
                {
                    tarefa.Ordem = i;
                    tarefa.DataAtualizacao = DateTime.Now;
                    await _tarefaRepository.UpdateAsync(tarefa);
                    algumaOrdemMudou = true;
                }
            }

            if (algumaOrdemMudou)
            {
                await _tarefaRepository.SaveChangesAsync();
            }
        }
    }

}
