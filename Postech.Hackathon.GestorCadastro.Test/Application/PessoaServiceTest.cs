using Microsoft.Extensions.Options;
using Moq;
using Postech.Hackathon.GestorCadastro.Application.DTO;
using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.Services;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Domain.Settings;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;
using Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Postech.Hackathon.GestorCadastro.Test.Application;

public class PessoaServiceTest
{
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
    private readonly Mock<IMedicoService> _mockMedicoService;
    private readonly PessoaService _pessoaService;
    
    public PessoaServiceTest()
    {
        _mockUsuarioRepository = new Mock<IUsuarioRepository>();
        _mockMedicoService = new Mock<IMedicoService>();
        _pessoaService = new PessoaService(_mockUsuarioRepository.Object, _mockMedicoService.Object);
    }

    [Fact]
    public async Task CadastrarAsync_ComDadosValidos_RetornaPessoaResponse()
    {
        // Arrange
        var request = new CadastrarRequest 
        { 
            Nome = "Novo Usuario", 
            Email = "novo@teste.com", 
            Senha = "Senha123!",
            CPF = "12345678901",
            TipoUsuario = ETipoUsuario.Paciente 
        };
        
        _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(null as Usuario);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorCpfAsync(request.CPF))
            .ReturnsAsync(null as Usuario);
        
        // Act
        var resultado = await _pessoaService.CadastrarAsync(request);
        
        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(request.Nome, resultado.Nome);
        Assert.Equal(request.Email, resultado.Email);
        Assert.Equal(request.CPF, resultado.CPF);
        Assert.Equal(request.TipoUsuario, resultado.TipoUsuario);
        _mockUsuarioRepository.Verify(x => x.CreateAsync(It.IsAny<Usuario>()), Times.Once);
    }
    
    [Fact]
    public async Task CadastrarAsync_ComEmailJaExistente_LancaExcecao()
    {
        // Arrange
        var request = new CadastrarRequest 
        { 
            Nome = "Novo Usuario", 
            Email = "existente@teste.com", 
            Senha = "Senha123!",
            CPF = "12345678901",
            TipoUsuario = ETipoUsuario.Paciente 
        };
        
        var usuarioExistente = new Usuario(
            "Usuario Existente", 
            "existente@teste.com", 
            "Senha123!",
            "12345678901",
            ETipoUsuario.Paciente);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(usuarioExistente);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _pessoaService.CadastrarAsync(request));
        _mockUsuarioRepository.Verify(x => x.CreateAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_ComCpfJaExistente_LancaExcecao()
    {
        // Arrange
        var request = new CadastrarRequest 
        { 
            Nome = "Novo Usuario", 
            Email = "novo@teste.com", 
            Senha = "Senha123!",
            CPF = "12345678901",
            TipoUsuario = ETipoUsuario.Paciente 
        };
        
        var usuarioExistente = new Usuario(
            "Usuario Existente", 
            "existente@teste.com", 
            "Senha123!",
            "12345678901",
            ETipoUsuario.Paciente);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(null as Usuario);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorCpfAsync(request.CPF))
            .ReturnsAsync(usuarioExistente);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _pessoaService.CadastrarAsync(request));
        _mockUsuarioRepository.Verify(x => x.CreateAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_ComSenhaValida_GeraHashCorreto()
    {
        // Arrange
        var request = new CadastrarRequest 
        { 
            Nome = "Novo Usuario", 
            Email = "novo@teste.com", 
            Senha = "Senha123!",
            CPF = "12345678901",
            TipoUsuario = ETipoUsuario.Paciente 
        };
        
        _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(null as Usuario);
        
        // Act
        await _pessoaService.CadastrarAsync(request);
        
        // Assert
        _mockUsuarioRepository.Verify(x => x.CreateAsync(It.Is<Usuario>(u => 
            u.SenhaHash != request.Senha && // Verifica se a senha foi hasheada
            u.ValidaSenha(request.Senha) // Verifica se o hash está correto
        )), Times.Once);
    }
} 