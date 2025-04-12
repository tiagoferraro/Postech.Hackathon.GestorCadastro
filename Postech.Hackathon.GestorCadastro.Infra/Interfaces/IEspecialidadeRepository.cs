using Postech.Hackathon.GestorCadastro.Domain.Entities;

namespace Postech.Hackathon.GestorCadastro.Infra.Interfaces;

public interface IEspecialidadeRepository
{
    Task<IEnumerable<Especialidade>> ObterTodasAsync();
    Task<Especialidade?> ObterPorIdAsync(Guid id);
    Task<Especialidade> CreateAsync(Especialidade especialidade);   
} 