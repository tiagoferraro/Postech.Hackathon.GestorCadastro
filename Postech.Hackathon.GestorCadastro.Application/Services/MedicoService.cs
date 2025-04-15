using Microsoft.Extensions.Caching.Distributed;
using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;
using System.Text.Json;

namespace Postech.Hackathon.GestorCadastro.Application.Services;

public class MedicoService : IMedicoService
{
    private readonly IMedicoRepository _medicoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IDistributedCache _cache;
    private const string CACHE_KEY_PREFIX = "medicos_especialidade_";
    private static readonly TimeSpan CACHE_EXPIRATION = TimeSpan.FromMinutes(30);

    public MedicoService(
        IMedicoRepository medicoRepository, 
        IUsuarioRepository usuarioRepository,
        IDistributedCache cache)
    {
        _medicoRepository = medicoRepository;
        _usuarioRepository = usuarioRepository;
        _cache = cache;
    }

    public async Task<MedicoResponse> CadastrarAsync(Guid idUsuario, MedicoRequest request)
    {
        var existingMedico = await _medicoRepository.ObterPorCrmAsync(request.CRM);
        if (existingMedico != null)
        {
            throw new InvalidOperationException("CRM já está em uso");
        }

        var medico = new Medico(request.CRM, idUsuario, request.EspecialidadeId, request.ValorConsulta);
        await _medicoRepository.CreateAsync(medico);

        // Invalida o cache para a especialidade
        await _cache.RemoveAsync($"{CACHE_KEY_PREFIX}{request.EspecialidadeId}");

        return new MedicoResponse(medico.CRM, medico.EspecialidadeId, medico.ValorConsulta);
    }

    public async Task<MedicoResponse> AlterarAsync(Guid idUsuario, MedicoRequest request)
    {
        var medico = await _medicoRepository.ObterPorIdAsync(idUsuario) ?? throw new KeyNotFoundException("Médico não encontrado");

        var existingMedicoByCrm = await _medicoRepository.ObterPorCrmAsync(request.CRM);
        if (existingMedicoByCrm != null && existingMedicoByCrm.MedicoId != medico.MedicoId)
        {
            throw new InvalidOperationException("CRM já está em uso");
        }

        medico.AtualizarDados(request.CRM, request.EspecialidadeId, request.ValorConsulta);
        await _medicoRepository.UpdateAsync(medico);

        // Invalida o cache para a especialidade antiga e nova
        await _cache.RemoveAsync($"{CACHE_KEY_PREFIX}{medico.EspecialidadeId}");
        await _cache.RemoveAsync($"{CACHE_KEY_PREFIX}{request.EspecialidadeId}");

        return new MedicoResponse(request.CRM, request.EspecialidadeId, request.ValorConsulta);
    }

    public async Task<IEnumerable<PessoaResponse>> ObterPorEspecialidadeAsync(Guid especialidadeId)
    {
        var cacheKey = $"{CACHE_KEY_PREFIX}{especialidadeId}";
        
        // Tenta obter do cache
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<IEnumerable<PessoaResponse>>(cachedData) ?? Enumerable.Empty<PessoaResponse>();
        }

        var medicos = await _medicoRepository.ObterPorEspecialidadeAsync(especialidadeId);
        var pessoas = new List<PessoaResponse>();
        
        foreach (var medico in medicos)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(medico.UsuarioId);
            if (usuario != null)
            {
                var medicoResponse = new MedicoResponse(medico.CRM, medico.EspecialidadeId, medico.ValorConsulta);
                pessoas.Add(new PessoaResponse(
                    Id: usuario.UsuarioId,
                    Nome: usuario.Nome,
                    Email: usuario.Email,
                    CPF: usuario.CPF,
                    TipoUsuario: usuario.TipoUsuario,
                    DataCriacao: usuario.DataCriacao,
                    UltimoLogin: usuario.UltimoLogin,
                    Medico: medicoResponse
                ));
            }
        }

        // Armazena no cache
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CACHE_EXPIRATION
        };

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(pessoas),
            cacheOptions);

        return pessoas;
    }
} 