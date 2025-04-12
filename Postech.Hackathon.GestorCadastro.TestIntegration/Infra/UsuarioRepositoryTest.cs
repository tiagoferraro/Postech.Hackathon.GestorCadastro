using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Infra;
using Postech.Hackathon.GestorCadastro.Infra.Repositories;
using Testcontainers.MsSql;
using Postech.Hackathon.GestorCadastro.TestIntegration.Infra.Fixtures;

namespace Postech.Hackathon.GestorCadastro.TestIntegration.Infra;

public class UsuarioRepositoryTest : IClassFixture<UsuarioRepositoryFixture>
{
    private readonly UsuarioRepositoryFixture _fixture;

    public UsuarioRepositoryTest(UsuarioRepositoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_DeveInserirUsuarioERetornarComId()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        var usuario = _fixture.TestUsuario;

        // Act
        var result = await _fixture.Repository.CreateAsync(usuario);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(usuario.UsuarioId, result.UsuarioId);
        Assert.Equal(usuario.Nome, result.Nome);
        Assert.Equal(usuario.Email, result.Email);
        Assert.Equal(usuario.CPF, result.CPF);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarUsuarioQuandoExiste()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        await _fixture.Repository.CreateAsync(_fixture.TestUsuario);

        // Act
        var result = await _fixture.Repository.ObterPorIdAsync(_fixture.TestUsuario.UsuarioId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_fixture.TestUsuario.UsuarioId, result.UsuarioId);
        Assert.Equal(_fixture.TestUsuario.Nome, result.Nome);
        Assert.Equal(_fixture.TestUsuario.Email, result.Email);
        Assert.Equal(_fixture.TestUsuario.CPF, result.CPF);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNullQuandoNaoExiste()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        var idInexistente = Guid.NewGuid();

        // Act
        var result = await _fixture.Repository.ObterPorIdAsync(idInexistente);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_DeveRetornarUsuarioQuandoExiste()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        await _fixture.Repository.CreateAsync(_fixture.TestUsuario);

        // Act
        var result = await _fixture.Repository.ObterPorEmailAsync(_fixture.TestUsuario.Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_fixture.TestUsuario.UsuarioId, result.UsuarioId);
        Assert.Equal(_fixture.TestUsuario.Nome, result.Nome);
        Assert.Equal(_fixture.TestUsuario.Email, result.Email);
        Assert.Equal(_fixture.TestUsuario.CPF, result.CPF);
    }

    [Fact]
    public async Task GetByCpfAsync_DeveRetornarUsuarioQuandoExiste()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        await _fixture.Repository.CreateAsync(_fixture.TestUsuario);

        // Act
        var result = await _fixture.Repository.ObterPorCpfAsync(_fixture.TestUsuario.CPF);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_fixture.TestUsuario.UsuarioId, result.UsuarioId);
        Assert.Equal(_fixture.TestUsuario.Nome, result.Nome);
        Assert.Equal(_fixture.TestUsuario.Email, result.Email);
        Assert.Equal(_fixture.TestUsuario.CPF, result.CPF);
    }

    [Fact]
    public async Task UpdateAsync_DeveAtualizarUsuarioExistente()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        await _fixture.Repository.CreateAsync(_fixture.TestUsuario);
        
        var usuarioAtualizado = new Usuario(
            "Nome Atualizado", 
            "atualizado@example.com", 
            "senha123",
            "98765432100",
            _fixture.TestUsuario.TipoUsuario);

        // Usar reflexão para definir o UsuarioId já que é privado
        typeof(Usuario).GetProperty("UsuarioId")?.SetValue(usuarioAtualizado, _fixture.TestUsuario.UsuarioId);

        // Act
        var result = await _fixture.Repository.UpdateAsync(usuarioAtualizado);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(usuarioAtualizado.UsuarioId, result.UsuarioId);
        Assert.Equal(usuarioAtualizado.Nome, result.Nome);
        Assert.Equal(usuarioAtualizado.Email, result.Email);
        Assert.Equal(usuarioAtualizado.CPF, result.CPF);
    }
}

