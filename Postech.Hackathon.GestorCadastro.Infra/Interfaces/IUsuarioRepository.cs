using Postech.Hackathon.GestorCadastro.Domain.Entities;

namespace Postech.Hackathon.GestorCadastro.Infra.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(Guid id);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario?> ObterPorCpfAsync(string cpf);
    Task<Usuario> CreateAsync(Usuario usuario);
    Task<Usuario> UpdateAsync(Usuario usuario); 
}
