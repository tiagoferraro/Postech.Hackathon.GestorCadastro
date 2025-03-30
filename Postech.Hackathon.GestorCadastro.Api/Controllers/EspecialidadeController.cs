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
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObterTodas()
    {
        try
        {
            var especialidades = await _especialidadeService.ObterTodasAsync();
            return Ok(especialidades);
        }
        catch (Exception ex)
        {
            return this.ApiBadRequest("Erro ao obter especialidades", ex.Message);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EspecialidadeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        try
        {
            var especialidade = await _especialidadeService.ObterPorIdAsync(id);
            if (especialidade == null)
                return this.ApiNotFound("Especialidade não encontrada");

            return Ok(especialidade);
        }
        catch (Exception ex)
        {
            return this.ApiBadRequest("Erro ao obter especialidade", ex.Message);
        }
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(EspecialidadeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarEspecialidadeRequest request)
    {
        try
        {
            var especialidade = await _especialidadeService.CriarAsync(request.Nome, request.Descricao);
            return CreatedAtAction(nameof(ObterPorId), new { id = especialidade.IdEspecialidade }, especialidade);
        }
        catch (Exception ex)
        {
            return this.ApiBadRequest("Erro ao criar especialidade", ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(EspecialidadeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarEspecialidadeRequest request)
    {
        try
        {
            var especialidade = await _especialidadeService.AtualizarAsync(id, request.Nome, request.Descricao);
            return Ok(especialidade);
        }
        catch (KeyNotFoundException ex)
        {
            return this.ApiNotFound("Especialidade não encontrada", ex.Message);
        }
        catch (Exception ex)
        {
            return this.ApiBadRequest("Erro ao atualizar especialidade", ex.Message);
        }
    }

} 