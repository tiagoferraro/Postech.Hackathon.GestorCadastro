using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Domain.Exceptions;

namespace Postech.Hackathon.GestorCadastro.Test.Domain;

public class UsuarioTest
{
    [Fact]
    public void CriarUsuario_ComDadosValidos_DeveRetornarUsuarioValido()
    {
        // Arrange
        string nome = "João Silva";
        string email = "joao.silva@exemplo.com";
        string senha = "Senha@123";
        string cpf = "12345678901";
        ETipoUsuario tipoUsuario = ETipoUsuario.Medico;

        // Act
        var usuario = new Usuario(nome, email, senha, cpf, tipoUsuario);

        // Assert
        Assert.NotNull(usuario);
        Assert.Equal(nome, usuario.Nome);
        Assert.Equal(email, usuario.Email);
        Assert.Equal(cpf, usuario.CPF);
        Assert.Equal(tipoUsuario, usuario.TipoUsuario);
        Assert.True(usuario.ValidaSenha(senha));
        Assert.NotEqual(Guid.Empty, usuario.UsuarioId);        
        Assert.Null(usuario.UltimoLogin);
    }

    [Theory]
    [InlineData("", "email@exemplo.com", "Senha@123", "12345678901", ETipoUsuario.Medico, "O nome do usuário não pode ser vazio.")]
    [InlineData("João Silva", "", "Senha@123", "12345678901", ETipoUsuario.Medico, "O email do usuário não pode ser vazio.")]
    [InlineData("João Silva", "email-invalido", "Senha@123", "12345678901", ETipoUsuario.Medico, "O email informado não é válido.")]
    [InlineData("João Silva", "email@exemplo", "Senha@123", "12345678901", ETipoUsuario.Medico, "O email informado não é válido.")]
    [InlineData("João Silva", "email@exemplo.com", "", "12345678901", ETipoUsuario.Medico, "A senha do usuário não pode ser vazia.")]
    [InlineData("João Silva", "email@exemplo.com", "Senha@123", "", ETipoUsuario.Medico, "O CPF do usuário não pode ser vazio.")]
    [InlineData("João Silva", "email@exemplo.com", "Senha@123", "123", ETipoUsuario.Medico, "O CPF deve conter exatamente 11 caracteres.")]
    [InlineData("João Silva", "email@exemplo.com", "Senha@123", "1234567890a", ETipoUsuario.Medico, "O CPF deve conter apenas números.")]
    [InlineData("João Silva", "email@exemplo.com", "Senha@123", "123456789012", ETipoUsuario.Medico, "O CPF deve conter exatamente 11 caracteres.")]    
    public void CriarUsuario_ComDadosInvalidos_DeveLancarExcecao(string nome, string email, string senha, string cpf, ETipoUsuario tipoUsuario, string mensagemEsperada)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Usuario(nome, email, senha, cpf, tipoUsuario));
        Assert.Equal(mensagemEsperada, exception.Message);
    }

    [Fact]
    public void ValidaSenha_ComSenhaCorreta_DeveRetornarTrue()
    {
        // Arrange
        string senha = "Senha@123";
        var usuario = new Usuario("João Silva", "joao.silva@exemplo.com", senha, "12345678901", ETipoUsuario.Medico);

        // Act
        bool resultado = usuario.ValidaSenha(senha);

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void ValidaSenha_ComSenhaIncorreta_DeveRetornarFalse()
    {
        // Arrange
        var usuario = new Usuario("João Silva", "joao.silva@exemplo.com", "Senha@123", "12345678901", ETipoUsuario.Medico);

        // Act
        bool resultado = usuario.ValidaSenha("SenhaErrada");

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void Logar_DeveAtualizarUltimoLogin()
    {
        // Arrange
        var usuario = new Usuario("João Silva", "joao.silva@exemplo.com", "Senha@123", "12345678901", ETipoUsuario.Medico);
        Assert.Null(usuario.UltimoLogin);

        // Act
        usuario.Logar();

        // Assert
        Assert.NotNull(usuario.UltimoLogin);
        Assert.True(DateTime.UtcNow.Subtract(usuario.UltimoLogin.Value).TotalMilliseconds >= 0); // Verifica se o timestamp não é futuro
    }

    [Fact]
    public void CriarUsuario_DeveTerIdUnico()
    {
        // Arrange & Act
        var usuario1 = new Usuario("João Silva", "joao@exemplo.com", "Senha@123", "12345678901", ETipoUsuario.Medico);
        var usuario2 = new Usuario("Maria Silva", "maria@exemplo.com", "Senha@456", "98765432100", ETipoUsuario.Medico);

        // Assert
        Assert.NotEqual(usuario1.UsuarioId, usuario2.UsuarioId);
    }

    [Fact]
    public void CriarUsuario_DeveTerDataCriacaoCorreta()
    {
        // Arrange & Act
        var antes = DateTime.UtcNow.AddSeconds(-1);
        var usuario = new Usuario("João Silva", "joao@exemplo.com", "Senha@123", "12345678901", ETipoUsuario.Medico);
        var depois = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(usuario.DataCriacao >= antes);
        Assert.True(usuario.DataCriacao <= depois);
    }

    [Theory]
    [InlineData(ETipoUsuario.Administrador)]
    [InlineData(ETipoUsuario.Medico)]
    [InlineData(ETipoUsuario.Paciente)]
    public void CriarUsuario_ComDiferentesTipos_DeveDefinirTipoCorretamente(ETipoUsuario tipoUsuario)
    {
        // Arrange & Act
        var usuario = new Usuario("João Silva", "joao@exemplo.com", "Senha@123", "12345678901", tipoUsuario);

        // Assert
        Assert.Equal(tipoUsuario, usuario.TipoUsuario);
    }
}

