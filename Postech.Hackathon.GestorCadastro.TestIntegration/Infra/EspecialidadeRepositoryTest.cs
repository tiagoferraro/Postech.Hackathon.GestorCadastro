using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Infra;
using Postech.Hackathon.GestorCadastro.Infra.Repositories;
using Testcontainers.MsSql;
using Postech.Hackathon.GestorCadastro.TestIntegration.Infra.Fixtures;

namespace Postech.Hackathon.GestorCadastro.TestIntegration.Infra;

public class EspecialidadeRepositoryTest : IClassFixture<EspecialidadeRepositoryFixture>
{
    private readonly EspecialidadeRepositoryFixture _fixture;

    public EspecialidadeRepositoryTest(EspecialidadeRepositoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_DeveInserirEspecialidadeERetornarComId()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        var especialidade = _fixture.TestEspecialidade;

        // Act
        var result = await _fixture.Repository.CreateAsync(especialidade);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(especialidade.EspecialidadeId, result.EspecialidadeId);
        Assert.Equal(especialidade.Nome, result.Nome);
        Assert.Equal(especialidade.Descricao, result.Descricao);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarEspecialidadeQuandoExiste()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        await _fixture.Repository.CreateAsync(_fixture.TestEspecialidade);

        // Act
        var result = await _fixture.Repository.ObterPorIdAsync(_fixture.TestEspecialidade.EspecialidadeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_fixture.TestEspecialidade.EspecialidadeId, result.EspecialidadeId);
        Assert.Equal(_fixture.TestEspecialidade.Nome, result.Nome);
        Assert.Equal(_fixture.TestEspecialidade.Descricao, result.Descricao);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNullQuandoNaoExiste()
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
    public async Task ObterTodasAsync_DeveRetornarListaDeEspecialidades()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        await _fixture.Repository.CreateAsync(_fixture.TestEspecialidade);
        var outraEspecialidade = new Especialidade(
            "Dermatologia",
            "Especialidade médica que se ocupa do diagnóstico e tratamento das doenças que acometem a pele");
        await _fixture.Repository.CreateAsync(outraEspecialidade);

        // Act
        var result = await _fixture.Repository.ObterTodasAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, e => e.EspecialidadeId == _fixture.TestEspecialidade.EspecialidadeId);
        Assert.Contains(result, e => e.EspecialidadeId == outraEspecialidade.EspecialidadeId);
    }

 
} 