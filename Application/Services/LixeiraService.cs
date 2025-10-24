using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Lixeira;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services
{
    public class LixeiraService : ILixeiraService
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly IAnotacaoRepository _anotacaoRepository;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly IUserContextService _userContextService;

        public LixeiraService(
            IProjetoRepository projetoRepository,
            IAnotacaoRepository anotacaoRepository,
            ITarefaRepository tarefaRepository,
            IUserContextService userContextService)
        {
            _projetoRepository = projetoRepository;
            _anotacaoRepository = anotacaoRepository;
            _tarefaRepository = tarefaRepository;
            _userContextService = userContextService;
        }

        public async Task<IEnumerable<LixeiraItemDto>> GetItensAsync()
        {
            var usuarioId = _userContextService.GetUserId();
            var agora = DateTime.Now;

            var projetos = await _projetoRepository.GetLixeiraByUserIdAsync(usuarioId);
            var anotacoes = await _anotacaoRepository.GetLixeiraByUserIdAsync(usuarioId);
            var tarefas = await _tarefaRepository.GetLixeiraByUserIdAsync(usuarioId);

            var itensLixeira = new List<LixeiraItemDto>();

            itensLixeira.AddRange(projetos.Select(p => new LixeiraItemDto
            {
                Id = p.IdProjeto,
                Tipo = "Projeto",
                Nome = p.Nome,
                DataDeletado = p.DataDeletado ?? agora
            }));

            itensLixeira.AddRange(anotacoes.Select(a => new LixeiraItemDto
            {
                Id = a.IdAnotacao,
                Tipo = "Anotacao",
                Nome = a.Titulo,
                DataDeletado = a.DataDeletado ?? agora,
                IdPai = a.IdProjeto,
                NomePai = a.Projeto?.Nome
            }));

            itensLixeira.AddRange(tarefas.Select(t => new LixeiraItemDto
            {
                Id = t.IdTarefa,
                Tipo = "Tarefa",
                Nome = t.Texto,
                DataDeletado = t.DataDeletado ?? agora
            }));

            // Ordenar a lista unificada pela data de exclusão
            return itensLixeira.OrderByDescending(item => item.DataDeletado);
        }
    }
}
