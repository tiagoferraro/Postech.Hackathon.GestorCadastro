using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postech.Hackathon.GestorCadastro.Api.Models;
using Postech.Hackathon.GestorCadastro.Api.Settings;
using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;

namespace Postech.Hackathon.GestorCadastro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticadorController(IAutenticacaoService _authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpPost("login/cpf")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginPorCpf([FromBody] LoginPorCpfRequest request)
    {
        var response = await _authService.LoginPorCpfAsync(request);
        return Ok(response);
    }

    [HttpPost("login/crm")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginPorCrm([FromBody] LoginPorCrmRequest request)
    {
        var response = await _authService.LoginPorCrmAsync(request);
        return Ok(response);
    }

    [HttpGet("ValidarToken")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult ValidarToken([FromQuery] string token)
    {
        var isValid = _authService.ValidateToken(token);
        return Ok(new { isValid });
    }

    [HttpGet("DadosUsuarioLogado")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> DadosUsuarioLogado()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return this.ApiUnauthorized("Usuário não autenticado");
        }

        var user = await _authService.GetUserByEmailAsync(email);
        if (user == null)
        {
            return this.ApiNotFound("Usuário não encontrado");
        }

        return Ok(new
        {
            user.Id,
            user.Nome,
            user.Email,
            user.TipoUsuario,
            user.DataCriacao,
        });
    }

    [HttpPost("alterar-senha")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaRequest request)
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return this.ApiUnauthorized("Usuário não autenticado");
        }

        await _authService.AlterarSenhaAsync(email, request);
        return Ok();
    }
}
