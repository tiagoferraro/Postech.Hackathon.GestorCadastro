using Microsoft.Extensions.Options;
using Moq;
using Postech.Hackathon.GestorCadastro.Application.DTO;
using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Services;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Domain.Exceptions;
using Postech.Hackathon.GestorCadastro.Domain.Settings;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Postech.Hackathon.GestorCadastro.Test.Application;

public class AutenticacaoServiceTest
{
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
    private readonly Mock<IMedicoRepository> _mockMedicoRepository;
    private readonly Mock<IOptions<JwtSettings>> _mockJwtSettings;
    private readonly AutenticacaoService _autenticacaoService;
    
    public AutenticacaoServiceTest()
    {
        _mockUsuarioRepository = new Mock<IUsuarioRepository>();
        _mockMedicoRepository = new Mock<IMedicoRepository>();
        _mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        
        var jwtSettings = new JwtSettings
        {
            SecretKey = "chave_secreta_muito_segura_123456789",
            Issuer = "teste",
            Audience = "teste",
            ExpirationInMinutes = 60
        };
        
        _mockJwtSettings.Setup(x => x.Value)
            .Returns(jwtSettings);
            
        _autenticacaoService = new AutenticacaoService(
            _mockUsuarioRepository.Object,
            _mockMedicoRepository.Object,
            _mockJwtSettings.Object);
    }

    public class LoginTests : AutenticacaoServiceTest
    {
        [Fact]
        public async Task LoginAsync_ComCredenciaisValidas_RetornaLoginResponse()
        {
            // Arrange
            var request = new LoginRequest { Email = "teste@teste.com", Senha = "Senha123!" };
            var usuario = new Usuario("Teste Usuario", "teste@teste.com", "Senha123!", "12345678901", ETipoUsuario.Paciente);
            
            _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync(usuario);
            
            // Act
            var resultado = await _autenticacaoService.LoginAsync(request);
            
            // Assert
            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado.Token);
            Assert.NotEmpty(resultado.RefreshToken);
            Assert.Equal(usuario.Nome, resultado.Username);
            Assert.Equal(usuario.TipoUsuario.ToString(), resultado.Role);
            _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Once);
        }
        
        [Fact]
        public async Task LoginAsync_ComCredenciaisInvalidas_LancaExcecao()
        {
            // Arrange
            var request = new LoginRequest { Email = "teste@teste.com", Senha = "SenhaErrada" };
            var usuario = new Usuario("Teste Usuario", "teste@teste.com", "Senha123!", "12345678901", ETipoUsuario.Paciente);
            
            _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync(usuario);
            
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _autenticacaoService.LoginAsync(request));
            _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }
        
        [Fact]
        public async Task LoginAsync_ComUsuarioInexistente_LancaExcecao()
        {
            // Arrange
            var request = new LoginRequest { Email = "inexistente@teste.com", Senha = "Senha123!" };
            
            _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync(null as Usuario);
            
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _autenticacaoService.LoginAsync(request));
        }
    }

    public class TokenTests : AutenticacaoServiceTest
    {
        [Fact]
        public void GenerateJwtToken_RetornaTokenValido()
        {
            // Arrange
            var usuario = new Usuario("Teste Usuario", "teste@teste.com", "Senha123!", "12345678901", ETipoUsuario.Paciente);
            
            // Act
            var token = _autenticacaoService.GenerateJwtToken(usuario);
            
            // Assert
            Assert.NotEmpty(token);
            Assert.True(_autenticacaoService.ValidateToken(token));
        }
        
        [Fact]
        public void GenerateRefreshToken_RetornaTokenNaoVazio()
        {
            // Act
            var refreshToken = _autenticacaoService.GenerateRefreshToken();
            
            // Assert
            Assert.NotEmpty(refreshToken);
        }

        [Fact]
        public void GenerateRefreshToken_GeraTokensUnicos()
        {
            // Act
            var refreshToken1 = _autenticacaoService.GenerateRefreshToken();
            var refreshToken2 = _autenticacaoService.GenerateRefreshToken();
            
            // Assert
            Assert.NotEqual(refreshToken1, refreshToken2);
        }
        
        [Fact]
        public void ValidateToken_ComTokenValido_RetornaTrue()
        {
            // Arrange
            var usuario = new Usuario("Teste Usuario", "teste@teste.com", "Senha123!", "12345678901", ETipoUsuario.Paciente);
            var token = _autenticacaoService.GenerateJwtToken(usuario);
            
            // Act
            var resultado = _autenticacaoService.ValidateToken(token);
            
            // Assert
            Assert.True(resultado);
        }
        
        [Fact]
        public void ValidateToken_ComTokenInvalido_RetornaFalse()
        {
            // Arrange
            var tokenInvalido = "token_invalido";
            
            // Act
            var resultado = _autenticacaoService.ValidateToken(tokenInvalido);
            
            // Assert
            Assert.False(resultado);
        }
    }

    public class UsuarioTests : AutenticacaoServiceTest
    {
        [Fact]
        public async Task GetUserByEmailAsync_ComEmailValido_RetornaUsuario()
        {
            // Arrange
            var email = "teste@teste.com";
            var usuario = new Usuario("Teste Usuario", email, "Senha123!", "12345678901", ETipoUsuario.Paciente);
            
            _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(email))
                .ReturnsAsync(usuario);
            
            // Act
            var resultado = await _autenticacaoService.GetUserByEmailAsync(email);
            
            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(usuario.UsuarioId, resultado.Id);
            Assert.Equal(usuario.Nome, resultado.Nome);
            Assert.Equal(usuario.Email, resultado.Email);
            Assert.Equal(usuario.TipoUsuario, resultado.TipoUsuario);
        }
        
        [Fact]
        public async Task GetUserByEmailAsync_ComEmailInexistente_LancaExcecao()
        {
            // Arrange
            var email = "inexistente@teste.com";
            
            _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(email))
                .ReturnsAsync(null as Usuario);
            
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _autenticacaoService.GetUserByEmailAsync(email));
        }
        
        [Fact]
        public async Task GetUserByEmailAsync_ComEmailVazio_LancaExcecao()
        {
            // Arrange
            string email = "";
            
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _autenticacaoService.GetUserByEmailAsync(email));
        }
    }

    public class LoginPorCpfTests : AutenticacaoServiceTest
    {
        [Fact]
        public async Task LoginPorCpfAsync_ComCredenciaisValidas_RetornaLoginResponse()
        {
            // Arrange
            var request = new LoginPorCpfRequest("12345678901", "Senha123!");
            var usuario = new Usuario("Teste Usuario", "teste@teste.com", "Senha123!", "12345678901", ETipoUsuario.Paciente);
            
            _mockUsuarioRepository.Setup(x => x.ObterPorCpfAsync(request.CPF))
                .ReturnsAsync(usuario);
            
            // Act
            var resultado = await _autenticacaoService.LoginPorCpfAsync(request);
            
            // Assert
            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado.Token);
            Assert.NotEmpty(resultado.RefreshToken);
            Assert.Equal(usuario.Nome, resultado.Username);
            Assert.Equal(usuario.TipoUsuario.ToString(), resultado.Role);
            _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Once);
        }
        
        [Fact]
        public async Task LoginPorCpfAsync_ComCredenciaisInvalidas_LancaExcecao()
        {
            // Arrange
            var request = new LoginPorCpfRequest("12345678901", "SenhaErrada");
            var usuario = new Usuario("Teste Usuario", "teste@teste.com", "Senha123!", "12345678901", ETipoUsuario.Paciente);
            
            _mockUsuarioRepository.Setup(x => x.ObterPorCpfAsync(request.CPF))
                .ReturnsAsync(usuario);
            
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _autenticacaoService.LoginPorCpfAsync(request));
            _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }
        
        [Fact]
        public async Task LoginPorCpfAsync_ComCpfInexistente_LancaExcecao()
        {
            // Arrange
            var request = new LoginPorCpfRequest("99999999999", "Senha123!");
            
            _mockUsuarioRepository.Setup(x => x.ObterPorCpfAsync(request.CPF))
                .ReturnsAsync(null as Usuario);
            
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _autenticacaoService.LoginPorCpfAsync(request));
        }
    }

    public class LoginPorCrmTests : AutenticacaoServiceTest
    {
        [Fact]
        public async Task LoginPorCrmAsync_ComCredenciaisValidas_RetornaLoginResponse()
        {
            // Arrange
            var request = new LoginPorCrmRequest("12345", "Senha123!");
            var usuario = new Usuario("Teste Medico", "medico@teste.com", "Senha123!", "12345678901", ETipoUsuario.Medico);
            var medico = new Medico("12345", usuario.UsuarioId, Guid.NewGuid(), 150.00m);
            
            _mockMedicoRepository.Setup(x => x.ObterPorCrmAsync(request.CRM))
                .ReturnsAsync(medico);
            
            _mockUsuarioRepository.Setup(x => x.ObterPorIdAsync(medico.UsuarioId))
                .ReturnsAsync(usuario);
            
            // Act
            var resultado = await _autenticacaoService.LoginPorCrmAsync(request);
            
            // Assert
            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado.Token);
            Assert.NotEmpty(resultado.RefreshToken);
            Assert.Equal(usuario.Nome, resultado.Username);
            Assert.Equal(usuario.TipoUsuario.ToString(), resultado.Role);
            _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Once);
        }
        
        [Fact]
        public async Task LoginPorCrmAsync_ComCredenciaisInvalidas_LancaExcecao()
        {
            // Arrange
            var request = new LoginPorCrmRequest("12345", "SenhaErrada");
            var usuario = new Usuario("Teste Medico", "medico@teste.com", "Senha123!", "12345678901", ETipoUsuario.Medico);
            var medico = new Medico("12345", usuario.UsuarioId, Guid.NewGuid(), 150.00m);
            
            _mockMedicoRepository.Setup(x => x.ObterPorCrmAsync(request.CRM))
                .ReturnsAsync(medico);
            
            _mockUsuarioRepository.Setup(x => x.ObterPorIdAsync(medico.UsuarioId))
                .ReturnsAsync(usuario);
            
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _autenticacaoService.LoginPorCrmAsync(request));
            _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }
        
        [Fact]
        public async Task LoginPorCrmAsync_ComCrmInexistente_LancaExcecao()
        {
            // Arrange
            var request = new LoginPorCrmRequest("99999", "Senha123!");
            
            _mockMedicoRepository.Setup(x => x.ObterPorCrmAsync(request.CRM))
                .ReturnsAsync(null as Medico);
            
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _autenticacaoService.LoginPorCrmAsync(request));
        }
        
        [Fact]
        public async Task LoginPorCrmAsync_ComUsuarioNaoMedico_LancaExcecao()
        {
            // Arrange
            var request = new LoginPorCrmRequest("12345", "Senha123!");
            var usuario = new Usuario("Teste Paciente", "paciente@teste.com", "Senha123!", "12345678901", ETipoUsuario.Paciente);
            var medico = new Medico("12345", usuario.UsuarioId, Guid.NewGuid(), 150.00m);
            
            _mockMedicoRepository.Setup(x => x.ObterPorCrmAsync(request.CRM))
                .ReturnsAsync(medico);
            
            _mockUsuarioRepository.Setup(x => x.ObterPorIdAsync(medico.UsuarioId))
                .ReturnsAsync(usuario);
            
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _autenticacaoService.LoginPorCrmAsync(request));
        }
    }

    public class AlterarSenhaTests : AutenticacaoServiceTest
    {
        [Fact]
        public async Task AlterarSenhaAsync_ComDadosValidos_AlteraSenhaComSucesso()
        {
            // Arrange
            var email = "teste@teste.com";
            var request = new AlterarSenhaRequest("Senha123!", "NovaSenha123!");
            var usuario = new Usuario("Teste Usuario", email, "Senha123!", "12345678901", ETipoUsuario.Paciente);
            
            _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(email))
                .ReturnsAsync(usuario);
            
            // Act
            await _autenticacaoService.AlterarSenhaAsync(email, request);
            
            // Assert
            _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.Is<Usuario>(u => 
                u.ValidaSenha(request.NovaSenha) && 
                !u.ValidaSenha(request.SenhaAtual)
            )), Times.Once);
        }
        
        [Fact]
        public async Task AlterarSenhaAsync_ComSenhaAtualIncorreta_LancaExcecao()
        {
            // Arrange
            var email = "teste@teste.com";
            var request = new AlterarSenhaRequest("SenhaErrada", "NovaSenha123!");
            var usuario = new Usuario("Teste Usuario", email, "Senha123!", "12345678901", ETipoUsuario.Paciente);
            
            _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(email))
                .ReturnsAsync(usuario);
            
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _autenticacaoService.AlterarSenhaAsync(email, request));
            _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }
        
        [Fact]
        public async Task AlterarSenhaAsync_ComEmailInexistente_LancaExcecao()
        {
            // Arrange
            var email = "inexistente@teste.com";
            var request = new AlterarSenhaRequest("Senha123!", "NovaSenha123!");
            
            _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(email))
                .ReturnsAsync(null as Usuario);
            
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _autenticacaoService.AlterarSenhaAsync(email, request));
            _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }
        
        [Fact]
        public async Task AlterarSenhaAsync_ComEmailVazio_LancaExcecao()
        {
            // Arrange
            string email = "";
            var request = new AlterarSenhaRequest("Senha123!", "NovaSenha123!");
            
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _autenticacaoService.AlterarSenhaAsync(email, request));
            _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task AlterarSenhaAsync_ComSenhaVazia_LancaExcecao()
        {
            // Arrange
            var email = "usuario@teste.com";
            var request = new AlterarSenhaRequest("Senha123!", string.Empty);
            
            var usuario = new Usuario(
                "Usuario Teste", 
                email, 
                "Senha123!",
                "12345678901",
                ETipoUsuario.Paciente);
            
            _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(email))
                .ReturnsAsync(usuario);
            
            // Act & Assert
            var excecao = await Assert.ThrowsAsync<DomainException>(() => _autenticacaoService.AlterarSenhaAsync(email, request));
            Assert.Equal("A nova senha não pode ser vazia.", excecao.Message);
            _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }
    }

    public class TokenValidationTests : AutenticacaoServiceTest
    {
        [Fact]
        public void ValidateToken_ComTokenComIssuerInvalido_RetornaFalse()
        {
            // Arrange
            var usuario = new Usuario("Teste Usuario", "teste@teste.com", "Senha123!", "12345678901", ETipoUsuario.Paciente);
            var token = _autenticacaoService.GenerateJwtToken(usuario);
            
            // Simular issuer inválido
            var jwtSettings = new JwtSettings
            {
                SecretKey = "chave_secreta_para_testes_com_tamanho_suficiente_para_hmacsha256",
                Issuer = "issuer_invalido",
                Audience = "teste_audience",
                ExpirationInMinutes = 60
            };
            
            _mockJwtSettings.Setup(x => x.Value).Returns(jwtSettings);
            
            // Act
            var resultado = _autenticacaoService.ValidateToken(token);
            
            // Assert
            Assert.False(resultado);
        }
        
        [Fact]
        public void ValidateToken_ComTokenComAudienceInvalido_RetornaFalse()
        {
            // Arrange
            var usuario = new Usuario("Teste Usuario", "teste@teste.com", "Senha123!", "12345678901", ETipoUsuario.Paciente);
            var token = _autenticacaoService.GenerateJwtToken(usuario);
            
            // Simular audience inválido
            var jwtSettings = new JwtSettings
            {
                SecretKey = "chave_secreta_para_testes_com_tamanho_suficiente_para_hmacsha256",
                Issuer = "teste_issuer",
                Audience = "audience_invalido",
                ExpirationInMinutes = 60
            };
            
            _mockJwtSettings.Setup(x => x.Value).Returns(jwtSettings);
            
            // Act
            var resultado = _autenticacaoService.ValidateToken(token);
            
            // Assert
            Assert.False(resultado);
        }
    }
}

