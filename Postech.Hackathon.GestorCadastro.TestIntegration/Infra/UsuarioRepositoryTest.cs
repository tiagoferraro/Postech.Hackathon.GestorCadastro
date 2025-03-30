using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Infra;
using Postech.Hackathon.GestorCadastro.Infra.Repositories;
using Testcontainers.MsSql;

namespace Postech.Hackathon.GestorCadastro.TestIntegration.Infra;

public class UsuarioRepositoryTest : IAsyncLifetime
{
    
    private UsuarioRepository? _repository;
    private readonly Usuario _testUsuario = new (
            "Teste Usuario",
            "teste@example.com",
            "senha123",
            "12345678901",
            ETipoUsuario.Administrador);

    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
       .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
       .WithPortBinding(1434, true) // Changed port to 1434
       .Build();

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        
        var dbSettings = Options.Create(new DatabaseSettings
        {
            ConnectionString = _sqlContainer.GetConnectionString()
        });
        
        _repository = new UsuarioRepository(dbSettings);
        
        // Criar tabela de usuários
        await SetupDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await _sqlContainer.StopAsync();
    }

    private async Task SetupDatabaseAsync()
    {
        // Implementar criação da tabela Usuarios usando Dapper ou outro método
        using var connection = new Microsoft.Data.SqlClient.SqlConnection(_sqlContainer.GetConnectionString());
        await connection.OpenAsync();
        
        var createTableSql = @"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
            BEGIN
                CREATE TABLE Usuarios (
                    IdUsuario UNIQUEIDENTIFIER PRIMARY KEY,
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

    [Fact]
    public async Task CreateAsync_DeveInserirUsuarioERetornarComId()
    {
        // Arrange
        var usuario = _testUsuario;

        // Act
        var result = await _repository.CreateAsync(usuario);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(usuario.Id, result.Id);
        Assert.Equal(usuario.Nome, result.Nome);
        Assert.Equal(usuario.Email, result.Email);
        Assert.Equal(usuario.CPF, result.CPF);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarUsuarioQuandoExiste()
    {
        // Arrange
        await _repository.CreateAsync(_testUsuario);

        // Act
        var result = await _repository.ObterPorIdAsync(_testUsuario.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_testUsuario.Id, result.Id);
        Assert.Equal(_testUsuario.Nome, result.Nome);
        Assert.Equal(_testUsuario.Email, result.Email);
        Assert.Equal(_testUsuario.CPF, result.CPF);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNullQuandoNaoExiste()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var result = await _repository.ObterPorIdAsync(idInexistente);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_DeveRetornarUsuarioQuandoExiste()
    {
        // Arrange
        await _repository.CreateAsync(_testUsuario);

        // Act
        var result = await _repository.ObterPorEmailAsync(_testUsuario.Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_testUsuario.Id, result.Id);
        Assert.Equal(_testUsuario.Nome, result.Nome);
        Assert.Equal(_testUsuario.Email, result.Email);
        Assert.Equal(_testUsuario.CPF, result.CPF);
    }

    [Fact]
    public async Task GetByCpfAsync_DeveRetornarUsuarioQuandoExiste()
    {
        // Arrange
        await _repository.CreateAsync(_testUsuario);

        // Act
        var result = await _repository.ObterPorCpfAsync(_testUsuario.CPF);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_testUsuario.Id, result.Id);
        Assert.Equal(_testUsuario.Nome, result.Nome);
        Assert.Equal(_testUsuario.Email, result.Email);
        Assert.Equal(_testUsuario.CPF, result.CPF);
    }

    [Fact]
    public async Task UpdateAsync_DeveAtualizarUsuarioExistente()
    {
        // Arrange
        await _repository.CreateAsync(_testUsuario);
        
        var usuarioAtualizado = new Usuario(
            "Nome Atualizado", 
            "atualizado@example.com", 
            "senha123",
            "98765432100",
            _testUsuario.TipoUsuario);
        
        typeof(Usuario)?.GetProperty("Id")?.SetValue(usuarioAtualizado, _testUsuario.Id);

        // Act
        var result = await _repository.UpdateAsync(usuarioAtualizado);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(usuarioAtualizado.Id, result.Id);
        Assert.Equal(usuarioAtualizado.Nome, result.Nome);
        Assert.Equal(usuarioAtualizado.Email, result.Email);
        Assert.Equal(usuarioAtualizado.CPF, result.CPF);
    }
}

