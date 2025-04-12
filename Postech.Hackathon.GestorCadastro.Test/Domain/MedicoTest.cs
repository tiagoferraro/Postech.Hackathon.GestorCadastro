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

        // Act
        var medico = new Medico(crm, usuarioId, especialidadeId);

        // Assert
        Assert.NotNull(medico);
        Assert.Equal(crm, medico.CRM);
        Assert.Equal(usuarioId, medico.UsuarioId);
        Assert.Equal(especialidadeId, medico.EspecialidadeId);
    }

    [Fact]
    public void Construtor_ComCrmVazio_LancaExcecao()
    {
        // Arrange
        var crm = string.Empty;
        var usuarioId = Guid.NewGuid();
        var especialidadeId = Guid.NewGuid();

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => new Medico(crm, usuarioId, especialidadeId));
        Assert.Equal("O CRM do médico não pode ser vazio.", excecao.Message);
    }

    [Fact]
    public void Construtor_ComUsuarioIdVazio_LancaExcecao()
    {
        // Arrange
        var crm = "12345";
        var usuarioId = Guid.Empty;
        var especialidadeId = Guid.NewGuid();

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => new Medico(crm, usuarioId, especialidadeId));
        Assert.Equal("O ID do usuário não pode ser vazio.", excecao.Message);
    }

    [Fact]
    public void Construtor_ComEspecialidadeIdVazio_LancaExcecao()
    {
        // Arrange
        var crm = "12345";
        var usuarioId = Guid.NewGuid();
        var especialidadeId = Guid.Empty;

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => new Medico(crm, usuarioId, especialidadeId));
        Assert.Equal("O ID da especialidade não pode ser vazio.", excecao.Message);
    }

    [Fact]
    public void AtualizarDados_ComDadosValidos_AtualizaMedico()
    {
        // Arrange
        var medico = new Medico("12345", Guid.NewGuid(), Guid.NewGuid());
        var novoCrm = "54321";
        var novaEspecialidadeId = Guid.NewGuid();

        // Act
        medico.AtualizarDados(novoCrm, novaEspecialidadeId);

        // Assert
        Assert.Equal(novoCrm, medico.CRM);
        Assert.Equal(novaEspecialidadeId, medico.EspecialidadeId);
    }

    [Fact]
    public void AtualizarDados_ComCrmVazio_LancaExcecao()
    {
        // Arrange
        var medico = new Medico("12345", Guid.NewGuid(), Guid.NewGuid());
        var novoCrm = string.Empty;
        var novaEspecialidadeId = Guid.NewGuid();

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => medico.AtualizarDados(novoCrm, novaEspecialidadeId));
        Assert.Equal("O CRM do médico não pode ser vazio.", excecao.Message);
    }

    [Fact]
    public void AtualizarDados_ComEspecialidadeIdVazio_LancaExcecao()
    {
        // Arrange
        var medico = new Medico("12345", Guid.NewGuid(), Guid.NewGuid());
        var novoCrm = "54321";
        var novaEspecialidadeId = Guid.Empty;

        // Act & Assert
        var excecao = Assert.Throws<DomainException>(() => medico.AtualizarDados(novoCrm, novaEspecialidadeId));
        Assert.Equal("O ID da especialidade não pode ser vazio.", excecao.Message);
    }
}