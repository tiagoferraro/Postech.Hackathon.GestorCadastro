using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Settings;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;

namespace Postech.Hackathon.GestorCadastro.Infra.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly string _connectionString;

    public UsuarioRepository(IOptions<DatabaseSettings> dbSettings)
    {
        _connectionString = dbSettings.Value.ConnectionString;
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Usuarios WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Usuarios WHERE Email = @Email";
        return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Email = email });
    }

    public async Task<Usuario?> ObterPorCpfAsync(string cpf)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Usuarios WHERE CPF = @CPF";
        return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { CPF = cpf });
    }

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            INSERT INTO Usuarios (Id, Nome, Email, CPF, Senha, DataNascimento, Telefone)
            VALUES (@Id, @Nome, @Email, @CPF, @Senha, @DataNascimento, @Telefone);
            SELECT * FROM Usuarios WHERE Id = @Id";
        
        return await connection.QueryFirstAsync<Usuario>(sql, usuario);
    }

    public async Task<Usuario> UpdateAsync(Usuario usuario)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            UPDATE Usuarios 
            SET Nome = @Nome, 
                Email = @Email, 
                CPF = @CPF, 
                Senha = @Senha, 
                DataNascimento = @DataNascimento, 
                Telefone = @Telefone
            WHERE Id = @Id;
            SELECT * FROM Usuarios WHERE Id = @Id";
        
        return await connection.QueryFirstAsync<Usuario>(sql, usuario);
    }


}
