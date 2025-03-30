using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Settings;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;

namespace Postech.Hackathon.GestorCadastro.Infra.Repositories;

public class MedicoRepository : IMedicoRepository
{
    private readonly string _connectionString;

    public MedicoRepository(IOptions<DatabaseSettings> dbSettings)
    {
        _connectionString = dbSettings.Value.ConnectionString;
    }

    public async Task<Medico?> ObterPorIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Medicos WHERE IdMedico = @Id";
        return await connection.QueryFirstOrDefaultAsync<Medico>(sql, new { Id = id });
    }

    public async Task<Medico?> ObterPorCrmAsync(string crm)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Medicos WHERE CRM = @CRM";
        return await connection.QueryFirstOrDefaultAsync<Medico>(sql, new { CRM = crm });
    }

    public async Task<Medico> CreateAsync(Medico medico)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            INSERT INTO Medicos (IdMedico, IdUsuario, IdEspecialidade, CRM)
            VALUES (@IdMedico, @IdUsuario, @IdEspecialidade, @CRM);
            SELECT * FROM Medicos WHERE IdMedico = @IdMedico";
        
        return await connection.QueryFirstAsync<Medico>(sql, medico);
    }

    public async Task<Medico> UpdateAsync(Medico medico)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            UPDATE Medicos 
            SET CRM = @CRM, 
                IdEspecialidade = @IdEspecialidade
            WHERE IdMedico = @IdMedico;
            SELECT * FROM Medicos WHERE IdMedico = @IdMedico";
        
        return await connection.QueryFirstAsync<Medico>(sql, medico);
    }
} 