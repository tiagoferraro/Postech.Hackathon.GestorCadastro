using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;

namespace Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;

public interface IMedicoService
{
    Task<MedicoResponse> CadastrarAsync(Guid idUsuario, MedicoRequest request);
    Task<MedicoResponse> AlterarAsync(Guid idUsuario, MedicoRequest request);
} 