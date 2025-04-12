using Postech.Hackathon.GestorCadastro.Application.DTO.Response;

namespace Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;

public interface IEspecialidadeService
{
    Task<IEnumerable<EspecialidadeResponse>> ObterTodasAsync();
    Task<EspecialidadeResponse?> ObterPorIdAsync(Guid id);
    Task<EspecialidadeResponse> CriarAsync(string nome, string descricao);
} 