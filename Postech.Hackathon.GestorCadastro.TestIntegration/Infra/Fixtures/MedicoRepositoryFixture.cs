using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Infra;
using Postech.Hackathon.GestorCadastro.Infra.Repositories;
using Testcontainers.MsSql;

namespace Postech.Hackathon.GestorCadastro.TestIntegration.Infra.Fixtures;

public class MedicoRepositoryFixture : IAsyncLifetime
{
    public MedicoRepository Repository { get; private set; } = null!;
    public UsuarioRepository UsuarioRepository { get; private set; } = null!;
    public EspecialidadeRepository EspecialidadeRepository { get; private set; } = null!;
    
    public Usuario TestUsuario { get; } = new(
        "Dr. Teste",
        "teste@example.com",
        "senha123",
        "12345678901",
        ETipoUsuario.Medico);

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
        
        Repository = new MedicoRepository(dbSettings);
        UsuarioRepository = new UsuarioRepository(dbSettings);
        EspecialidadeRepository = new EspecialidadeRepository(dbSettings);
        
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
        
        var createTablesSql = @"
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
            END

            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Especialidade')
            BEGIN
                CREATE TABLE Especialidade (
                    EspecialidadeId UNIQUEIDENTIFIER PRIMARY KEY,
                    Nome VARCHAR(100) NOT NULL,
                    Descricao VARCHAR(500) NOT NULL,
                    DataCriacao DATETIME NOT NULL,
                    IndAtivo BIT NOT NULL DEFAULT 1
                );
            END

            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Medico')
            BEGIN
                CREATE TABLE Medico (
                    MedicoId UNIQUEIDENTIFIER PRIMARY KEY,
                    UsuarioId UNIQUEIDENTIFIER NOT NULL,
                    EspecialidadeId UNIQUEIDENTIFIER NOT NULL,
                    CRM VARCHAR(20) NOT NULL,
                    ValorConsulta DECIMAL(10,2) NOT NULL,
                    FOREIGN KEY (UsuarioId) REFERENCES Usuario(UsuarioId),
                    FOREIGN KEY (EspecialidadeId) REFERENCES Especialidade(EspecialidadeId)
                );
            END";
        
        using var command = new Microsoft.Data.SqlClient.SqlCommand(createTablesSql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task CleanupDatabaseAsync()
    {
        using var connection = new Microsoft.Data.SqlClient.SqlConnection(_sqlContainer.GetConnectionString());
        await connection.OpenAsync();
        
        var cleanupSql = @"
            DELETE FROM Medico;
            DELETE FROM Usuario;
            DELETE FROM Especialidade;";
            
        using var command = new Microsoft.Data.SqlClient.SqlCommand(cleanupSql, connection);
        await command.ExecuteNonQueryAsync();
    }
} 