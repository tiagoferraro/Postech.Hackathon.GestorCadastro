using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;

namespace Postech.Hackathon.GestorCadastro.Infra.Repositories;

public class EspecialidadeRepository : IEspecialidadeRepository
{
    private readonly string _connectionString;

    public EspecialidadeRepository(IOptions<DatabaseSettings> databaseSettings) => _connectionString = databaseSettings.Value.ConnectionString;

    public async Task<IEnumerable<Especialidade>> ObterTodasAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT EspecialidadeId, Nome, Descricao, DataCriacao, IndAtivo
            FROM Especialidade
            WHERE IndAtivo = 1
            ORDER BY Nome";

        return await connection.QueryAsync<Especialidade>(sql);
    }

    public async Task<Especialidade?> ObterPorIdAsync(Guid EspecialidadeId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT EspecialidadeId, Nome, Descricao, DataCriacao, IndAtivo
            FROM Especialidade
            WHERE EspecialidadeId = @EspecialidadeId AND IndAtivo = 1";

        return await connection.QueryFirstOrDefaultAsync<Especialidade>(sql, new { EspecialidadeId = EspecialidadeId });
    }

    public async Task<Especialidade> CreateAsync(Especialidade especialidade)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            INSERT INTO Especialidade (EspecialidadeId, Nome, Descricao, DataCriacao, IndAtivo)
            VALUES (@EspecialidadeId, @Nome, @Descricao, @DataCriacao, @IndAtivo)";

        await connection.ExecuteAsync(sql, especialidade);
        return especialidade;
    }

 
} 