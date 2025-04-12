using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;

namespace Postech.Hackathon.GestorCadastro.Application.Services;

public class EspecialidadeService : IEspecialidadeService
{
    private readonly IEspecialidadeRepository _especialidadeRepository;

    public EspecialidadeService(IEspecialidadeRepository especialidadeRepository)
    {
        _especialidadeRepository = especialidadeRepository;
    }

    public async Task<IEnumerable<EspecialidadeResponse>> ObterTodasAsync()
    {
        var especialidades = await _especialidadeRepository.ObterTodasAsync();
        return especialidades.Select(e => new EspecialidadeResponse(
            e.EspecialidadeId,
            e.Nome,
            e.Descricao,
            e.DataCriacao
        ));
    }

    public async Task<EspecialidadeResponse?> ObterPorIdAsync(Guid id)
    {
        var especialidade = await _especialidadeRepository.ObterPorIdAsync(id);
        if (especialidade == null)
            return null;

        return new EspecialidadeResponse(
            especialidade.EspecialidadeId,
            especialidade.Nome,
            especialidade.Descricao,
            especialidade.DataCriacao
        );
    }

    public async Task<EspecialidadeResponse> CriarAsync(string nome, string descricao)
    {
        var especialidade = new Especialidade(nome, descricao);
        var especialidadeCriada = await _especialidadeRepository.CreateAsync(especialidade);

        return new EspecialidadeResponse(
            especialidadeCriada.EspecialidadeId,
            especialidadeCriada.Nome,
            especialidadeCriada.Descricao,
            especialidadeCriada.DataCriacao
        );
    }

} 