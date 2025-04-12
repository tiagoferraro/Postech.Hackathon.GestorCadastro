using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;

namespace Postech.Hackathon.GestorCadastro.Infra.Repositories;

public class MedicoRepository : IMedicoRepository
{
    private readonly string _connectionString;

    public MedicoRepository(IOptions<DatabaseSettings> dbSettings) => _connectionString = dbSettings.Value.ConnectionString;

    public async Task<Medico?> ObterPorIdAsync(Guid medicoId)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Medico WHERE MedicoId = @MedicoId";
        return await connection.QueryFirstOrDefaultAsync<Medico>(sql, new { @MedicoId = medicoId });
    }

    public async Task<Medico?> ObterPorCrmAsync(string crm)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Medico WHERE CRM = @CRM";
        return await connection.QueryFirstOrDefaultAsync<Medico>(sql, new { CRM = crm });
    }

    public async Task<Medico> CreateAsync(Medico medico)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            INSERT INTO Medico (MedicoId, UsuarioID , EspecialidadeId, CRM)
            VALUES (@MedicoId,  @UsuarioID , @EspecialidadeId, @CRM);
            SELECT * FROM Medico WHERE MedicoId = @MedicoId";
        
        return await connection.QueryFirstAsync<Medico>(sql, medico);
    }

    public async Task<Medico> UpdateAsync(Medico medico)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            UPDATE Medico
            SET CRM = @CRM, 
                EspecialidadeId = @EspecialidadeId
            WHERE MedicoId = @MedicoId;
            SELECT * FROM Medico WHERE MedicoId = @MedicoId";
        
        return await connection.QueryFirstAsync<Medico>(sql, medico);
    }
} 