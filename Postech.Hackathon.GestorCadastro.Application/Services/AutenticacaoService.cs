using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Settings;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Postech.Hackathon.GestorCadastro.Application.Services;

public class AutenticacaoService(IUsuarioRepository _usuarioRepository, IMedicoRepository _medicoRepository, IOptions<JwtSettings> _jwtSettings) : IAutenticacaoService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email) ?? throw new UnauthorizedAccessException("Usuário ou senha inválidos");

        if (!usuario.ValidaSenha(request.Senha))
        {
            throw new UnauthorizedAccessException("Usuário ou senha inválidos");
        }

        usuario.Logar();
        await _usuarioRepository.UpdateAsync(usuario);

        var token = GenerateJwtToken(usuario);
        var refreshToken = GenerateRefreshToken();

        return new LoginResponse(
            Token: token,
            RefreshToken: refreshToken,
            Expiration: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpirationInMinutes),
            Username: usuario.Nome,
            Role: usuario.TipoUsuario.ToString()
        );
    }

    public async Task<LoginResponse> LoginPorCpfAsync(LoginPorCpfRequest request)
    {
        var usuario = await _usuarioRepository.ObterPorCpfAsync(request.CPF) ?? throw new UnauthorizedAccessException("Usuário ou senha inválidos");

        if (!usuario.ValidaSenha(request.Senha))
        {
            throw new UnauthorizedAccessException("Usuário ou senha inválidos");
        }

        usuario.Logar();
        await _usuarioRepository.UpdateAsync(usuario);

        var token = GenerateJwtToken(usuario);
        var refreshToken = GenerateRefreshToken();

        return new LoginResponse(
            Token: token,
            RefreshToken: refreshToken,
            Expiration: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpirationInMinutes),
            Username: usuario.Nome,
            Role: usuario.TipoUsuario.ToString()
        );
    }

    public async Task<LoginResponse> LoginPorCrmAsync(LoginPorCrmRequest request)
    {
        var medico = await _medicoRepository.ObterPorCrmAsync(request.CRM) ?? throw new UnauthorizedAccessException("CRM ou senha inválidos");

        var usuario = await _usuarioRepository.ObterPorIdAsync(medico.IdUsuario) ?? throw new UnauthorizedAccessException("Usuário não encontrado");

        if (!usuario.ValidaSenha(request.Senha))
        {
            throw new UnauthorizedAccessException("CRM ou senha inválidos");
        }

        if (usuario.TipoUsuario != Domain.Enum.ETipoUsuario.Medico)
        {
            throw new UnauthorizedAccessException("Usuário não é um médico");
        }

        usuario.Logar();
        await _usuarioRepository.UpdateAsync(usuario);

        var token = GenerateJwtToken(usuario);
        var refreshToken = GenerateRefreshToken();

        return new LoginResponse(
            Token: token,
            RefreshToken: refreshToken,
            Expiration: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpirationInMinutes),
            Username: usuario.Nome,
            Role: usuario.TipoUsuario.ToString()
        );
    }

    public string GenerateJwtToken(Usuario usuario)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Value.Issuer,
            audience: _jwtSettings.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Value.SecretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Value.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Value.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<PessoaResponse> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("Email não pode ser nulo ou vazio", nameof(email));
        }
      
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email) 
            ?? throw new KeyNotFoundException($"Usuário com email {email} não encontrado");

        return new PessoaResponse(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.CPF,
            usuario.TipoUsuario, 
            usuario.DataCriacao,
            usuario.UltimoLogin
        );
    }

    public async Task AlterarSenhaAsync(string email, AlterarSenhaRequest request)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("Email não pode ser nulo ou vazio", nameof(email));
        }

        var usuario = await _usuarioRepository.ObterPorEmailAsync(email)
            ?? throw new KeyNotFoundException($"Usuário com email {email} não encontrado");

        if (!usuario.ValidaSenha(request.SenhaAtual))
        {
            throw new UnauthorizedAccessException("Senha atual incorreta");
        }

        usuario.AlterarSenha(request.NovaSenha);
        await _usuarioRepository.UpdateAsync(usuario);
    }
}
