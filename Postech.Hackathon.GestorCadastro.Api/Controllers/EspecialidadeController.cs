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
public class EspecialidadeController(IEspecialidadeService _especialidadeService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EspecialidadeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodas()
    {
        var especialidades = await _especialidadeService.ObterTodasAsync();
        return Ok(especialidades);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EspecialidadeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var especialidade = await _especialidadeService.ObterPorIdAsync(id);
        if (especialidade == null)
            return this.ApiNotFound("Especialidade não encontrada");

        return Ok(especialidade);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(EspecialidadeResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] CriarEspecialidadeRequest request)
    {
        var especialidade = await _especialidadeService.CriarAsync(request.Nome, request.Descricao);
        return CreatedAtAction(nameof(ObterPorId), new { id = especialidade.IdEspecialidade }, especialidade);
    }

} 