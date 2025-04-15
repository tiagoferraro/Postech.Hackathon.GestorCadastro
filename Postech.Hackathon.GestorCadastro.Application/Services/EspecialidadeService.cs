using Microsoft.Extensions.Caching.Distributed;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;
using System.Text.Json;

namespace Postech.Hackathon.GestorCadastro.Application.Services;

public class EspecialidadeService : IEspecialidadeService
{
    private readonly IEspecialidadeRepository _especialidadeRepository;
    private readonly IDistributedCache _cache;
    private const string CACHE_KEY_ALL = "especialidades_all";    
    private static readonly TimeSpan CACHE_EXPIRATION = TimeSpan.FromMinutes(30);

    public EspecialidadeService(IEspecialidadeRepository especialidadeRepository, IDistributedCache cache)
    {
        _especialidadeRepository = especialidadeRepository;
        _cache = cache;
    }

    public async Task<IEnumerable<EspecialidadeResponse>> ObterTodasAsync()
    {
        // Tenta obter do cache
        var cachedData = await _cache.GetStringAsync(CACHE_KEY_ALL);
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<IEnumerable<EspecialidadeResponse>>(cachedData) ?? Enumerable.Empty<EspecialidadeResponse>();
        }

        var especialidades = await _especialidadeRepository.ObterTodasAsync();
        var response = especialidades.Select(e => new EspecialidadeResponse(
            e.EspecialidadeId,
            e.Nome,
            e.Descricao,
            e.DataCriacao
        )).ToList();

        // Armazena no cache
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CACHE_EXPIRATION
        };

        await _cache.SetStringAsync(
            CACHE_KEY_ALL,
            JsonSerializer.Serialize(response),
            cacheOptions);

        return response;
    }

    public async Task<EspecialidadeResponse?> ObterPorIdAsync(Guid id)
    {      

        var especialidade = await _especialidadeRepository.ObterPorIdAsync(id);
        if (especialidade == null)
            return null;

        var response = new EspecialidadeResponse(
            especialidade.EspecialidadeId,
            especialidade.Nome,
            especialidade.Descricao,
            especialidade.DataCriacao
        );
        
        return response;
    }

    public async Task<EspecialidadeResponse> CriarAsync(string nome, string descricao)
    {
        var especialidade = new Especialidade(nome, descricao);
        var especialidadeCriada = await _especialidadeRepository.CreateAsync(especialidade);

        // Invalida o cache de todas as especialidades
        await _cache.RemoveAsync(CACHE_KEY_ALL);

        return new EspecialidadeResponse(
            especialidadeCriada.EspecialidadeId,
            especialidadeCriada.Nome,
            especialidadeCriada.Descricao,
            especialidadeCriada.DataCriacao
        );
    }
} 