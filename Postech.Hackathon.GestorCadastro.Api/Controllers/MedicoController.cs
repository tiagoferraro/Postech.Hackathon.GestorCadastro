using Microsoft.AspNetCore.Mvc;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;

namespace Postech.Hackathon.GestorCadastro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicoController(IMedicoService _medicoService) : ControllerBase
    {
        [HttpGet("PorEspecialidade/{especialidadeId}")]
        [ProducesResponseType(typeof(IEnumerable<PessoaResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PorEspecialidade(Guid especialidadeId)
        {
            var response = await _medicoService.ObterPorEspecialidadeAsync(especialidadeId);
            return Ok(response);
        }


    }
}
