using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Infra;
using Postech.Hackathon.GestorCadastro.Infra.Repositories;
using Testcontainers.MsSql;

namespace Postech.Hackathon.GestorCadastro.TestIntegration.Infra.Fixtures;

public class EspecialidadeRepositoryFixture : IAsyncLifetime
{
    public EspecialidadeRepository Repository { get; private set; } = null!;
    public Especialidade TestEspecialidade { get; } = new(
        "Cardiologia",
        "Especialidade médica que se ocupa do diagnóstico e tratamento das doenças que acometem o coração");

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
        
        Repository = new EspecialidadeRepository(dbSettings);
        
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
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Especialidade')
            BEGIN
                CREATE TABLE Especialidade (
                    EspecialidadeId UNIQUEIDENTIFIER PRIMARY KEY,
                    Nome VARCHAR(100) NOT NULL,
                    Descricao VARCHAR(500) NOT NULL,
                    DataCriacao DATETIME NOT NULL,
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
        
        var cleanupSql = "DELETE FROM Especialidade";
        using var command = new Microsoft.Data.SqlClient.SqlCommand(cleanupSql, connection);
        await command.ExecuteNonQueryAsync();
    }
} 