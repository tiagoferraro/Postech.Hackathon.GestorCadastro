using Postech.Hackathon.GestorCadastro.Domain.Entities;

namespace Postech.Hackathon.GestorCadastro.Infra.Interfaces;

public interface IMedicoRepository
{
    Task<Medico?> ObterPorIdAsync(Guid id);
    Task<Medico?> ObterPorCrmAsync(string crm);
    Task<Medico> CreateAsync(Medico medico);
    Task<Medico> UpdateAsync(Medico medico);
} 