using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postech.Hackathon.GestorCadastro.Application.Interfaces.Services
{
    public interface IPessoaService
    {
        Task<PessoaResponse> CadastrarAsync(CadastrarRequest request);
        Task<PessoaResponse> AlterarAsync(AlterarRequest request);
    }
}
