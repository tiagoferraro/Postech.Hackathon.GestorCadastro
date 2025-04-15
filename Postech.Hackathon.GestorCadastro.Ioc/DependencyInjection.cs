using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;
using Postech.Hackathon.GestorCadastro.Application.Services;
using Postech.Hackathon.GestorCadastro.Domain.Settings;
using Postech.Hackathon.GestorCadastro.Infra;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;
using Postech.Hackathon.GestorCadastro.Infra.Repositories;
using StackExchange.Redis;

namespace Postech.Hackathon.GestorCadastro.Ioc;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurações
        services.Configure<JwtSettings>(options => configuration.GetSection("JwtSettings").Bind(options));
        services.Configure<DatabaseSettings>(options => configuration.GetSection("DatabaseSettings").Bind(options));
        services.Configure<RedisSettings>(options => configuration.GetSection("RedisSettings").Bind(options));

        // Redis
        var redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisSettings?.ConnectionString;
            options.InstanceName = redisSettings?.InstanceName;
        });

        // Repositórios
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IMedicoRepository, MedicoRepository>();
        services.AddScoped<IEspecialidadeRepository, EspecialidadeRepository>();

        // Serviços
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();
        services.AddScoped<IPessoaService, PessoaService>();
        services.AddScoped<IMedicoService, MedicoService>();
        services.AddScoped<IEspecialidadeService, EspecialidadeService>();

        return services;
    }
}

