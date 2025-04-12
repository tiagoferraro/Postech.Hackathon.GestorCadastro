using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Exceptions;
using Xunit;

namespace Postech.Hackathon.GestorCadastro.Test.Domain;

public class MedicoTest
{
    [Fact]
    public void Construtor_ComDadosValidos_CriaMedico()
    {
        // Arrange
        var crm = "12345";
        var usuarioId = Guid.NewGuid();
        var especialidadeId = Guid.NewGuid();
        var valorConsulta = 150.00m;

        // Act
        var medico = new Medico(crm, usuarioId, especialidadeId, valorConsulta);

        // Assert
        Assert.NotNull(medico);
        Assert.Equal(crm, medico.CRM);
        Assert.Equal(usuarioId, medico.UsuarioId);
        Assert.Equal(especialidadeId, medico.EspecialidadeId);
        Assert.Equal(valorConsulta, medico.ValorConsulta);
    }

    [Fact]
    public void Construtor_ComCrmVazio_LancaExcecao()
    {
        // Arrange
        var crm = string.Empty;
        var usuarioId = Guid.NewGuid();
        var especialidadeId = Guid.NewGuid();
        var valorConsulta = 150.00m;

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => new Medico(crm, usuarioId, especialidadeId, valorConsulta));
        Assert.Equal("O CRM do médico não pode ser vazio.", excecao.Message);
    }

    [Fact]
    public void Construtor_ComUsuarioIdVazio_LancaExcecao()
    {
        // Arrange
        var crm = "12345";
        var usuarioId = Guid.Empty;
        var especialidadeId = Guid.NewGuid();
        var valorConsulta = 150.00m;

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => new Medico(crm, usuarioId, especialidadeId, valorConsulta));
        Assert.Equal("O ID do usuário não pode ser vazio.", excecao.Message);
    }

    [Fact]
    public void Construtor_ComEspecialidadeIdVazio_LancaExcecao()
    {
        // Arrange
        var crm = "12345";
        var usuarioId = Guid.NewGuid();
        var especialidadeId = Guid.Empty;
        var valorConsulta = 150.00m;

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => new Medico(crm, usuarioId, especialidadeId, valorConsulta));
        Assert.Equal("O ID da especialidade não pode ser vazio.", excecao.Message);
    }

    [Fact]
    public void Construtor_ComValorConsultaNegativo_LancaExcecao()
    {
        // Arrange
        var crm = "12345";
        var usuarioId = Guid.NewGuid();
        var especialidadeId = Guid.NewGuid();
        var valorConsulta = -150.00m;

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => new Medico(crm, usuarioId, especialidadeId, valorConsulta));
        Assert.Equal("O valor da consulta não pode ser negativo ou zero.", excecao.Message);
    }

    [Fact]
    public void Construtor_ComValorConsultaZero_LancaExcecao()
    {
        // Arrange
        var crm = "12345";
        var usuarioId = Guid.NewGuid();
        var especialidadeId = Guid.NewGuid();
        var valorConsulta = 0.00m;

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => new Medico(crm, usuarioId, especialidadeId, valorConsulta));
        Assert.Equal("O valor da consulta não pode ser negativo ou zero.", excecao.Message);
    }

    [Fact]
    public void AtualizarDados_ComDadosValidos_AtualizaMedico()
    {
        // Arrange
        var medico = new Medico("12345", Guid.NewGuid(), Guid.NewGuid(), 150.00m);
        var novoCrm = "54321";
        var novaEspecialidadeId = Guid.NewGuid();

        // Act
        medico.AtualizarDados(novoCrm, novaEspecialidadeId, 200.00m);

        // Assert
        Assert.Equal(novoCrm, medico.CRM);
        Assert.Equal(novaEspecialidadeId, medico.EspecialidadeId);
        Assert.Equal(200.00m, medico.ValorConsulta);
    }

    [Fact]
    public void AtualizarDados_ComCrmVazio_LancaExcecao()
    {
        // Arrange
        var medico = new Medico("12345", Guid.NewGuid(), Guid.NewGuid(), 150.00m);
        var novoCrm = string.Empty;
        var novaEspecialidadeId = Guid.NewGuid();

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => medico.AtualizarDados(novoCrm, novaEspecialidadeId, 200.00m));
        Assert.Equal("O CRM do médico não pode ser vazio.", excecao.Message);
    }

    [Fact]
    public void AtualizarDados_ComEspecialidadeIdVazio_LancaExcecao()
    {
        // Arrange
        var medico = new Medico("12345", Guid.NewGuid(), Guid.NewGuid(), 150.00m);
        var novoCrm = "54321";
        var novaEspecialidadeId = Guid.Empty;

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => medico.AtualizarDados(novoCrm, novaEspecialidadeId, 200.00m));
        Assert.Equal("O ID da especialidade não pode ser vazio.", excecao.Message);
    }

    [Fact]
    public void AtualizarDados_ComValorConsultaNegativo_LancaExcecao()
    {
        // Arrange
        var medico = new Medico("12345", Guid.NewGuid(), Guid.NewGuid(), 150.00m);
        var novoCrm = "54321";
        var novaEspecialidadeId = Guid.NewGuid();

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => medico.AtualizarDados(novoCrm, novaEspecialidadeId, -200.00m));
        Assert.Equal("O valor da consulta não pode ser negativo ou zero.", excecao.Message);
    }

    [Fact]
    public void AtualizarDados_ComValorConsultaZero_LancaExcecao()
    {
        // Arrange
        var medico = new Medico("12345", Guid.NewGuid(), Guid.NewGuid(), 150.00m);
        var novoCrm = "54321";
        var novaEspecialidadeId = Guid.NewGuid();

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => medico.AtualizarDados(novoCrm, novaEspecialidadeId, 0.00m));
        Assert.Equal("O valor da consulta não pode ser negativo ou zero.", excecao.Message);
    }
}