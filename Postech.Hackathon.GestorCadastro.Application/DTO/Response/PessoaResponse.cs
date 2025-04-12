using Postech.Hackathon.GestorCadastro.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postech.Hackathon.GestorCadastro.Application.DTO.Response;

public record MedicoResponse(string CRM, Guid IdEspecialidade, decimal ValorConsulta);

public record PessoaResponse(
    Guid Id, 
    string Nome, 
    string Email,
    string CPF,
    ETipoUsuario TipoUsuario, 
    DateTime DataCriacao, 
    DateTime? UltimoLogin,
    MedicoResponse? Medico = null);


