using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;

namespace Postech.Hackathon.GestorCadastro.Application.Services;

public class MedicoService(IMedicoRepository _medicoRepository) : IMedicoService
{
    public async Task<MedicoResponse> CadastrarAsync(Guid idUsuario, MedicoRequest request)
    {
        var existingMedico = await _medicoRepository.ObterPorCrmAsync(request.CRM);
        if (existingMedico != null)
        {
            throw new InvalidOperationException("CRM já está em uso");
        }

        var medico = new Medico(request.CRM, idUsuario, request.EspecialidadeId);
        await _medicoRepository.CreateAsync(medico);

        return new MedicoResponse(medico.CRM, medico.EspecialidadeId);
    }

    public async Task<MedicoResponse> AlterarAsync(Guid idUsuario, MedicoRequest request)
    {
        var medico = await _medicoRepository.ObterPorIdAsync(idUsuario) ?? throw new KeyNotFoundException("Médico não encontrado");

        var existingMedicoByCrm = await _medicoRepository.ObterPorCrmAsync(request.CRM);
        if (existingMedicoByCrm != null && existingMedicoByCrm.MedicoId != medico.MedicoId)
        {
            throw new InvalidOperationException("CRM já está em uso");
        }

        medico.AtualizarDados(request.CRM, request.EspecialidadeId);
        await _medicoRepository.UpdateAsync(medico);

        return new MedicoResponse(request.CRM, request.EspecialidadeId);
    }
} 