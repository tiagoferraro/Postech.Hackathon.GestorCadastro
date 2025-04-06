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
    public class PessoaController(IPessoaService _pessoaService) : ControllerBase
    {
        [HttpPost("Cadastrar")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarRequest request)
        {
            var response = await _pessoaService.CadastrarAsync(request);
            return Ok(response);
        }

        [HttpPut("Alterar")]
        [Authorize]
        [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Alterar([FromBody] AlterarRequest request)
        {
            var response = await _pessoaService.AlterarAsync(request);
            return Ok(response);
        }
    }
}
