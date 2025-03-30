using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Domain.Entities;

namespace Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;

    public interface IAutenticacaoService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> LoginPorCpfAsync(LoginPorCpfRequest request);
        Task<LoginResponse> LoginPorCrmAsync(LoginPorCrmRequest request);
        bool ValidateToken(string token);
        Task<PessoaResponse> GetUserByEmailAsync(string email);
        string GenerateJwtToken(Usuario usuario);
        string GenerateRefreshToken();
        Task AlterarSenhaAsync(string email, AlterarSenhaRequest request);
    }
 