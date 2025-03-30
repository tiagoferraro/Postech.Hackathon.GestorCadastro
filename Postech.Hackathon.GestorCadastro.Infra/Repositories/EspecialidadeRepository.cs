using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;

namespace Postech.Hackathon.GestorCadastro.Infra.Repositories;

public class EspecialidadeRepository : IEspecialidadeRepository
{
    private readonly string _connectionString;

    public EspecialidadeRepository(IOptions<DatabaseSettings> databaseSettings)
    {
        _connectionString = databaseSettings.Value.ConnectionString;
    }

    public async Task<IEnumerable<Especialidade>> ObterTodasAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT IdEspecialidade, Nome, Descricao, DataCriacao, IndAtivo
            FROM Especialidade
            WHERE IndAtivo = 1
            ORDER BY Nome";

        return await connection.QueryAsync<Especialidade>(sql);
    }

    public async Task<Especialidade?> ObterPorIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT IdEspecialidade, Nome, Descricao, DataCriacao, IndAtivo
            FROM Especialidade
            WHERE IdEspecialidade = @Id AND IndAtivo = 1";

        return await connection.QueryFirstOrDefaultAsync<Especialidade>(sql, new { Id = id });
    }

    public async Task<Especialidade> CreateAsync(Especialidade especialidade)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            INSERT INTO Especialidade (IdEspecialidade, Nome, Descricao, DataCriacao, IndAtivo)
            VALUES (@IdEspecialidade, @Nome, @Descricao, @DataCriacao, @IndAtivo)";

        await connection.ExecuteAsync(sql, especialidade);
        return especialidade;
    }

    public async Task<Especialidade> UpdateAsync(Especialidade especialidade)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            UPDATE Especialidade
            SET Nome = @Nome,
                Descricao = @Descricao
            WHERE IdEspecialidade = @IdEspecialidade AND IndAtivo = 1";

        await connection.ExecuteAsync(sql, especialidade);
        return especialidade;
    }

 
} 