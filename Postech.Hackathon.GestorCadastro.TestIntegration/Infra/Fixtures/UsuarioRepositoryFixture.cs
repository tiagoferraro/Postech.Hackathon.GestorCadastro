using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Infra;
using Postech.Hackathon.GestorCadastro.Infra.Repositories;
using Testcontainers.MsSql;

namespace Postech.Hackathon.GestorCadastro.TestIntegration.Infra.Fixtures;

public class UsuarioRepositoryFixture : IAsyncLifetime
{
    public UsuarioRepository Repository { get; private set; } = null!;
    public Usuario TestUsuario { get; } = new(
        "Teste Usuario",
        "teste@example.com",
        "senha123",
        "12345678901",
        ETipoUsuario.Administrador);

    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
       .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
       .WithPortBinding(1434, true)
       .Build();

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        
        var dbSettings = Options.Create(new DatabaseSettings
        {
            ConnectionString = _sqlContainer.GetConnectionString()
        });
        
        Repository = new UsuarioRepository(dbSettings);
        
        await SetupDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await _sqlContainer.StopAsync();
    }

    private async Task SetupDatabaseAsync()
    {
        using var connection = new Microsoft.Data.SqlClient.SqlConnection(_sqlContainer.GetConnectionString());
        await connection.OpenAsync();
        
        var createTableSql = @"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuario')
            BEGIN
                CREATE TABLE Usuario (
                    UsuarioId UNIQUEIDENTIFIER PRIMARY KEY,
                    Nome VARCHAR(100) NOT NULL,
                    Email VARCHAR(100) NOT NULL,
                    SenhaHash VARCHAR(500) NOT NULL,
                    CPF VARCHAR(11) NOT NULL,
                    TipoUsuario INT NOT NULL,
                    DataCriacao DATETIME NOT NULL,
                    UltimoLogin DATETIME NULL,
                    IndAtivo BIT NOT NULL DEFAULT 1
                );
            END";
        
        using var command = new Microsoft.Data.SqlClient.SqlCommand(createTableSql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task CleanupDatabaseAsync()
    {
        using var connection = new Microsoft.Data.SqlClient.SqlConnection(_sqlContainer.GetConnectionString());
        await connection.OpenAsync();
        
        var cleanupSql = "DELETE FROM Usuario";
        using var command = new Microsoft.Data.SqlClient.SqlCommand(cleanupSql, connection);
        await command.ExecuteNonQueryAsync();
    }
} 