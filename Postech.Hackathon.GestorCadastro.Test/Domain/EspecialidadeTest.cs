using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Exceptions;

namespace Postech.Hackathon.GestorCadastro.Test.Domain;

public class EspecialidadeTest
{
    [Fact]
    public void CriarEspecialidade_ComDadosValidos_DeveCriarComSucesso()
    {
        // Arrange
        var nome = "Cardiologia";
        var descricao = "Especialidade médica que trata do coração";

        // Act
        var especialidade = new Especialidade(nome, descricao);

        // Assert
        Assert.Equal(nome, especialidade.Nome);
        Assert.Equal(descricao, especialidade.Descricao);
        Assert.True(especialidade.IndAtivo);
        Assert.NotEqual(Guid.Empty, especialidade.EspecialidadeId);
        Assert.True(especialidade.DataCriacao <= DateTime.UtcNow);
    }

    [Fact]
    public void CriarEspecialidade_ComNomeVazio_DeveLancarExcecao()
    {
        // Arrange
        var nome = "";
        var descricao = "Descrição válida";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Especialidade(nome, descricao));
        Assert.Equal("O nome da especialidade não pode ser vazio.", exception.Message);
    }

    [Fact]
    public void CriarEspecialidade_ComDescricaoVazia_DeveLancarExcecao()
    {
        // Arrange
        var nome = "Nome válido";
        var descricao = "";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Especialidade(nome, descricao));
        Assert.Equal("A descrição da especialidade não pode ser vazia.", exception.Message);
    }

    [Fact]
    public void CriarEspecialidade_ComNomeNulo_DeveLancarExcecao()
    {
        // Arrange
        string nome = null;
        var descricao = "Descrição válida";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Especialidade(nome, descricao));
        Assert.Equal("O nome da especialidade não pode ser vazio.", exception.Message);
    }

    [Fact]
    public void CriarEspecialidade_ComDescricaoNula_DeveLancarExcecao()
    {
        // Arrange
        var nome = "Nome válido";
        string descricao = null;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Especialidade(nome, descricao));
        Assert.Equal("A descrição da especialidade não pode ser vazia.", exception.Message);
    }
}