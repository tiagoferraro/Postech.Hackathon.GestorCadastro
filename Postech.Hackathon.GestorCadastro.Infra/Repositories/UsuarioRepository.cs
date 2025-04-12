using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;

namespace Postech.Hackathon.GestorCadastro.Infra.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly string _connectionString;

    public UsuarioRepository(IOptions<DatabaseSettings> dbSettings) => _connectionString = dbSettings.Value.ConnectionString;

    public async Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Usuario WHERE UsuarioId = @Id";
        return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {                    
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Usuario WHERE Email = @Email";
            return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Email = email });       
    }

    public async Task<Usuario?> ObterPorCpfAsync(string cpf)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Usuario WHERE CPF = @CPF";
        return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { CPF = cpf });
    }

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            INSERT INTO Usuario (UsuarioId, Nome, Email, CPF, SenhaHash, TipoUsuario, DataCriacao, UltimoLogin, IndAtivo)
            VALUES (@UsuarioId, @Nome, @Email, @CPF, @SenhaHash, @TipoUsuario, @DataCriacao, @UltimoLogin, @IndAtivo);
            SELECT * FROM Usuario WHERE UsuarioId = @UsuarioId";
        
        return await connection.QueryFirstAsync<Usuario>(sql, new
        {
            usuario.UsuarioId,
            usuario.Nome,
            usuario.Email,
            usuario.CPF,
            usuario.SenhaHash,
            TipoUsuario = (int)usuario.TipoUsuario,
            usuario.DataCriacao,
            usuario.UltimoLogin,
            usuario.IndAtivo
        });
    }

    public async Task<Usuario> UpdateAsync(Usuario usuario)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            UPDATE Usuario 
            SET Nome = @Nome, 
                Email = @Email, 
                CPF = @CPF, 
                SenhaHash = @SenhaHash,
                TipoUsuario = @TipoUsuario,
                UltimoLogin = @UltimoLogin,
                IndAtivo = @IndAtivo
            WHERE UsuarioId = @UsuarioId;
            SELECT * FROM Usuario WHERE UsuarioId = @UsuarioId";
        
        return await connection.QueryFirstAsync<Usuario>(sql, new
        {
            usuario.UsuarioId,
            usuario.Nome,
            usuario.Email,
            usuario.CPF,
            usuario.SenhaHash,
            TipoUsuario = (int)usuario.TipoUsuario,
            usuario.UltimoLogin,
            usuario.IndAtivo
        });
    }

}
