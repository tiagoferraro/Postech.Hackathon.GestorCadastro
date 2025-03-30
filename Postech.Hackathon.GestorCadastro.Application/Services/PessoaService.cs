using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Settings;
using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postech.Hackathon.GestorCadastro.Application.Services;

public class PessoaService : IPessoaService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMedicoService _medicoService;

    public PessoaService(IUsuarioRepository usuarioRepository, IMedicoService medicoService)
    {
        _usuarioRepository = usuarioRepository;
        _medicoService = medicoService;
    }

    public async Task<PessoaResponse> CadastrarAsync(CadastrarRequest request)
    {
        var existingUser = await _usuarioRepository.ObterPorEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email já está em uso");
        }

        var existingUserByCpf = await _usuarioRepository.ObterPorCpfAsync(request.CPF);
        if (existingUserByCpf != null)
        {
            throw new InvalidOperationException("CPF já está em uso");
        }

        var usuario = new Usuario(
             request.Nome,
             request.Email,
             request.Senha,
             request.CPF,
             request.TipoUsuario
            );

        await _usuarioRepository.CreateAsync(usuario);

        MedicoResponse? medicoResponse = null;

        if (request.TipoUsuario == ETipoUsuario.Medico && request.Medico != null)
        {
            medicoResponse = await _medicoService.CadastrarAsync(usuario.Id, request.Medico);
        }

        return new PessoaResponse(
            Id: usuario.Id,
            Nome: usuario.Nome,
            Email: usuario.Email,
            CPF: usuario.CPF,
            TipoUsuario: usuario.TipoUsuario,
            DataCriacao: usuario.DataCriacao,
            UltimoLogin: usuario.UltimoLogin,
            Medico: medicoResponse
        );
    }

    public async Task<PessoaResponse> AlterarAsync(AlterarRequest request)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(request.Id) ?? throw new KeyNotFoundException($"Usuário com ID {request.Id} não encontrado");

        var existingUserByEmail = await _usuarioRepository.ObterPorEmailAsync(request.Email);
        if (existingUserByEmail != null && existingUserByEmail.Id != request.Id)
        {
            throw new InvalidOperationException("Email já está em uso");
        }

        var existingUserByCpf = await _usuarioRepository.ObterPorCpfAsync(request.CPF);
        if (existingUserByCpf != null && existingUserByCpf.Id != request.Id)
        {
            throw new InvalidOperationException("CPF já está em uso");
        }

        usuario.AtualizarDados(request.Nome, request.Email, request.Senha, request.CPF, request.TipoUsuario);
        await _usuarioRepository.UpdateAsync(usuario);

        MedicoResponse? medicoResponse = null;

        if (request.TipoUsuario == ETipoUsuario.Medico && request.Medico != null)
        {
            medicoResponse = await _medicoService.AlterarAsync(usuario.Id, request.Medico);
        }

        return new PessoaResponse(
            Id: usuario.Id,
            Nome: usuario.Nome,
            Email: usuario.Email,
            CPF: usuario.CPF,
            TipoUsuario: usuario.TipoUsuario,
            DataCriacao: usuario.DataCriacao,
            UltimoLogin: usuario.UltimoLogin,
            Medico: medicoResponse
        );
    }
}
    

