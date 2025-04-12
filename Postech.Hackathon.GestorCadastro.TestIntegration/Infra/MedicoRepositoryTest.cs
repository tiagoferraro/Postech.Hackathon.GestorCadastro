using Microsoft.Extensions.Options;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Infra;
using Postech.Hackathon.GestorCadastro.Infra.Repositories;
using Testcontainers.MsSql;
using Postech.Hackathon.GestorCadastro.TestIntegration.Infra.Fixtures;

namespace Postech.Hackathon.GestorCadastro.TestIntegration.Infra;

public class MedicoRepositoryTest : IClassFixture<MedicoRepositoryFixture>
{
    private readonly MedicoRepositoryFixture _fixture;

    public MedicoRepositoryTest(MedicoRepositoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_DeveInserirMedicoERetornarComId()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        var usuario = await _fixture.UsuarioRepository.CreateAsync(_fixture.TestUsuario);
        var especialidade = await _fixture.EspecialidadeRepository.CreateAsync(_fixture.TestEspecialidade);
        
        var medico = new Medico(
            "12345",
            usuario.UsuarioId,
            especialidade.EspecialidadeId,
            150.00m);

        // Act
        var result = await _fixture.Repository.CreateAsync(medico);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(medico.MedicoId, result.MedicoId);
        Assert.Equal(medico.CRM, result.CRM);
        Assert.Equal(medico.UsuarioId, result.UsuarioId);
        Assert.Equal(medico.EspecialidadeId, result.EspecialidadeId);
        Assert.Equal(medico.ValorConsulta, result.ValorConsulta);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarMedicoQuandoExiste()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        var usuario = await _fixture.UsuarioRepository.CreateAsync(_fixture.TestUsuario);
        var especialidade = await _fixture.EspecialidadeRepository.CreateAsync(_fixture.TestEspecialidade);
        
        var medico = new Medico(
            "12345",
            usuario.UsuarioId,
            especialidade.EspecialidadeId,
            150.00m);
        
        await _fixture.Repository.CreateAsync(medico);

        // Act
        var result = await _fixture.Repository.ObterPorIdAsync(medico.MedicoId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(medico.MedicoId, result.MedicoId);
        Assert.Equal(medico.CRM, result.CRM);
        Assert.Equal(medico.UsuarioId, result.UsuarioId);
        Assert.Equal(medico.EspecialidadeId, result.EspecialidadeId);
        Assert.Equal(medico.ValorConsulta, result.ValorConsulta);
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
    public async Task ObterPorCrmAsync_DeveRetornarMedicoQuandoExiste()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        var usuario = await _fixture.UsuarioRepository.CreateAsync(_fixture.TestUsuario);
        var especialidade = await _fixture.EspecialidadeRepository.CreateAsync(_fixture.TestEspecialidade);
        
        var medico = new Medico(
            "12345",
            usuario.UsuarioId,
            especialidade.EspecialidadeId,
            150.00m);
        
        await _fixture.Repository.CreateAsync(medico);

        // Act
        var result = await _fixture.Repository.ObterPorCrmAsync(medico.CRM);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(medico.MedicoId, result.MedicoId);
        Assert.Equal(medico.CRM, result.CRM);
        Assert.Equal(medico.UsuarioId, result.UsuarioId);
        Assert.Equal(medico.EspecialidadeId, result.EspecialidadeId);
        Assert.Equal(medico.ValorConsulta, result.ValorConsulta);
    }

    [Fact]
    public async Task ObterPorCrmAsync_DeveRetornarNullQuandoNaoExiste()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        var crmInexistente = "99999";

        // Act
        var result = await _fixture.Repository.ObterPorCrmAsync(crmInexistente);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_DeveAtualizarMedicoExistente()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        var usuario = await _fixture.UsuarioRepository.CreateAsync(_fixture.TestUsuario);
        var especialidade = await _fixture.EspecialidadeRepository.CreateAsync(_fixture.TestEspecialidade);
        
        var medico = new Medico(
            "12345",
            usuario.UsuarioId,
            especialidade.EspecialidadeId,
            150.00m);
        
        await _fixture.Repository.CreateAsync(medico);
        
        var novaEspecialidade = new Especialidade(
            "Dermatologia",
            "Especialidade médica que se ocupa do diagnóstico e tratamento das doenças que acometem a pele");
        
        await _fixture.EspecialidadeRepository.CreateAsync(novaEspecialidade);
        
        var medicoAtualizado = new Medico(
            "54321",
            usuario.UsuarioId,
            novaEspecialidade.EspecialidadeId,
            200.00m);

        // Usar reflexão para definir o MedicoId já que é privado
        typeof(Medico).GetProperty("MedicoId")?.SetValue(medicoAtualizado, medico.MedicoId);

        // Act
        var result = await _fixture.Repository.UpdateAsync(medicoAtualizado);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(medicoAtualizado.MedicoId, result.MedicoId);
        Assert.Equal(medicoAtualizado.CRM, result.CRM);
        Assert.Equal(medicoAtualizado.UsuarioId, result.UsuarioId);
        Assert.Equal(medicoAtualizado.EspecialidadeId, result.EspecialidadeId);
        Assert.Equal(medicoAtualizado.ValorConsulta, result.ValorConsulta);
    }

    [Fact]
    public async Task ListarPorEspecialidadeAsync_DeveRetornarMedicosDaEspecialidade()
    {
        // Arrange
        await _fixture.CleanupDatabaseAsync();
        var usuario = await _fixture.UsuarioRepository.CreateAsync(_fixture.TestUsuario);
        var especialidade1 = await _fixture.EspecialidadeRepository.CreateAsync(_fixture.TestEspecialidade);
        
        var especialidade2 = new Especialidade(
            "Dermatologia",
            "Especialidade médica que se ocupa do diagnóstico e tratamento das doenças que acometem a pele");
        await _fixture.EspecialidadeRepository.CreateAsync(especialidade2);
        
        var medico1 = new Medico("12345", usuario.UsuarioId, especialidade1.EspecialidadeId, 150.00m);
        var medico2 = new Medico("54321", usuario.UsuarioId, especialidade2.EspecialidadeId, 200.00m);
        
        await _fixture.Repository.CreateAsync(medico1);
        await _fixture.Repository.CreateAsync(medico2);

        // Act
        var result = await _fixture.Repository.ObterPorEspecialidadeAsync(especialidade1.EspecialidadeId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(medico1.MedicoId, result.First().MedicoId);
    }

 
} 