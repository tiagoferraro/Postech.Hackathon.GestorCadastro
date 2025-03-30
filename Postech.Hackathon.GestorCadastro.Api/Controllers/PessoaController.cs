using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Postech.Hackathon.GestorCadastro.Api.Models;
using Postech.Hackathon.GestorCadastro.Api.Settings;
using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;

namespace Postech.Hackathon.GestorCadastro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PessoaController(IPessoaService _pessoaService ) : ControllerBase
    {
        [HttpPost("Cadastrar")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarRequest request)
        {
            try
            {
                var response = await _pessoaService.CadastrarAsync(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return this.ApiBadRequest("Operação inválida", ex.Message);
            }
            catch (Exception ex)
            {
                return this.ApiBadRequest("Requisição inválida", ex.Message);
            }
        }

        [HttpPut("Alterar")]
        [Authorize]
        [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Alterar([FromBody] AlterarRequest request)
        {
            try
            {
                var response = await _pessoaService.AlterarAsync(request);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return this.ApiNotFound("Recurso não encontrado", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return this.ApiBadRequest("Operação inválida", ex.Message);
            }
            catch (Exception ex)
            {
                return this.ApiBadRequest("Requisição inválida", ex.Message);
            }
        }
    }
}
