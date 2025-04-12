using Microsoft.Extensions.Options;
using Moq;
using Postech.Hackathon.GestorCadastro.Application.DTO;
using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Services;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Domain.Exceptions;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;
using Postech.Hackathon.GestorCadastro.Application.Interfaces.Services;
using System;
using System.Threading.Tasks;
using Xunit;
using System.ComponentModel.DataAnnotations;

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

    [Fact]
    public async Task CadastrarAsync_ComTipoMedicoSemDadosMedico_LancaExcecao()
    {
        // Arrange
        var request = new CadastrarRequest 
        { 
            Nome = "Novo Medico", 
            Email = "medico@teste.com", 
            Senha = "Senha123!",
            CPF = "12345678901",
            TipoUsuario = ETipoUsuario.Medico,
            Medico = null
        };
        
        _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(null as Usuario);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorCpfAsync(request.CPF))
            .ReturnsAsync(null as Usuario);
        
        // Act & Assert
        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _pessoaService.CadastrarAsync(request));
        Assert.Equal("Dados do médico são obrigatórios para o tipo de usuário 'Médico'", excecao.Message);
        _mockUsuarioRepository.Verify(x => x.CreateAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_ComTipoMedicoComDadosMedico_RetornaPessoaResponseComMedico()
    {
        // Arrange
        var request = new CadastrarRequest 
        { 
            Nome = "Novo Medico", 
            Email = "medico@teste.com", 
            Senha = "Senha123!",
            CPF = "12345678901",
            TipoUsuario = ETipoUsuario.Medico,
            Medico = new MedicoRequest 
            { 
                CRM = "12345",
                EspecialidadeId = Guid.NewGuid()
            }
        };
        
        _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(null as Usuario);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorCpfAsync(request.CPF))
            .ReturnsAsync(null as Usuario);
        
        _mockMedicoService.Setup(x => x.CadastrarAsync(It.IsAny<Guid>(), request.Medico))
            .ReturnsAsync(new MedicoResponse(request.Medico.CRM, request.Medico.EspecialidadeId));
        
        // Act
        var resultado = await _pessoaService.CadastrarAsync(request);
        
        // Assert
        Assert.NotNull(resultado);
        Assert.NotNull(resultado.Medico);
        Assert.Equal(request.Medico.CRM, resultado.Medico.CRM);
        Assert.Equal(request.Medico.EspecialidadeId, resultado.Medico.IdEspecialidade);
        _mockUsuarioRepository.Verify(x => x.CreateAsync(It.IsAny<Usuario>()), Times.Once);
        _mockMedicoService.Verify(x => x.CadastrarAsync(It.IsAny<Guid>(), request.Medico), Times.Once);
    }

    [Fact]
    public async Task AlterarAsync_ComDadosValidos_RetornaPessoaResponse()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var request = new AlterarRequest 
        { 
            Id = usuarioId,
            Nome = "Usuario Atualizado", 
            Email = "atualizado@teste.com", 
            Senha = "NovaSenha123!",
            CPF = "98765432109",
            TipoUsuario = ETipoUsuario.Paciente 
        };
        
        var usuarioExistente = new Usuario(
            "Usuario Original", 
            "original@teste.com", 
            "Senha123!",
            "12345678901",
            ETipoUsuario.Paciente);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorIdAsync(request.Id))
            .ReturnsAsync(usuarioExistente);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(null as Usuario);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorCpfAsync(request.CPF))
            .ReturnsAsync(null as Usuario);
        
        // Act
        var resultado = await _pessoaService.AlterarAsync(request);
        
        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(request.Nome, resultado.Nome);
        Assert.Equal(request.Email, resultado.Email);
        Assert.Equal(request.CPF, resultado.CPF);
        Assert.Equal(request.TipoUsuario, resultado.TipoUsuario);
        _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task AlterarAsync_ComUsuarioNaoEncontrado_LancaExcecao()
    {
        // Arrange
        var request = new AlterarRequest 
        { 
            Id = Guid.NewGuid(),
            Nome = "Usuario Atualizado", 
            Email = "atualizado@teste.com", 
            Senha = "NovaSenha123!",
            CPF = "98765432109",
            TipoUsuario = ETipoUsuario.Paciente 
        };
        
        _mockUsuarioRepository.Setup(x => x.ObterPorIdAsync(request.Id))
            .ReturnsAsync(null as Usuario);
        
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _pessoaService.AlterarAsync(request));
        _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task AlterarAsync_ComEmailJaExistente_LancaExcecao()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var request = new AlterarRequest 
        { 
            Id = usuarioId,
            Nome = "Usuario Atualizado", 
            Email = "existente@teste.com", 
            Senha = "NovaSenha123!",
            CPF = "98765432109",
            TipoUsuario = ETipoUsuario.Paciente 
        };
        
        var usuarioExistente = new Usuario(
            "Usuario Original", 
            "original@teste.com", 
            "Senha123!",
            "12345678901",
            ETipoUsuario.Paciente);
        
        var outroUsuario = new Usuario(
            "Outro Usuario", 
            "existente@teste.com", 
            "Senha123!",
            "98765432109",
            ETipoUsuario.Paciente);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorIdAsync(request.Id))
            .ReturnsAsync(usuarioExistente);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(outroUsuario);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _pessoaService.AlterarAsync(request));
        _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task AlterarAsync_ComCpfJaExistente_LancaExcecao()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var request = new AlterarRequest 
        { 
            Id = usuarioId,
            Nome = "Usuario Atualizado", 
            Email = "atualizado@teste.com", 
            Senha = "NovaSenha123!",
            CPF = "98765432109",
            TipoUsuario = ETipoUsuario.Paciente 
        };
        
        var usuarioExistente = new Usuario(
            "Usuario Original", 
            "original@teste.com", 
            "Senha123!",
            "12345678901",
            ETipoUsuario.Paciente);
        
        var outroUsuario = new Usuario(
            "Outro Usuario", 
            "outro@teste.com", 
            "Senha123!",
            "98765432109",
            ETipoUsuario.Paciente);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorIdAsync(request.Id))
            .ReturnsAsync(usuarioExistente);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(null as Usuario);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorCpfAsync(request.CPF))
            .ReturnsAsync(outroUsuario);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _pessoaService.AlterarAsync(request));
        _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
    }

   

    [Fact]
    public async Task AlterarAsync_ComTipoMedicoComDadosMedico_RetornaPessoaResponseComMedico()
    {
        // Arrange


        var usuarioExistente = new Usuario(
      "Medico Original",
      "original@teste.com",
      "Senha123!",
      "12345678901",
      ETipoUsuario.Medico);

        var request = new AlterarRequest 
        { 
            Id = usuarioExistente.UsuarioId,
            Nome = "Medico Atualizado", 
            Email = "medico@teste.com", 
            Senha = "NovaSenha123!",
            CPF = "98765432109",
            TipoUsuario = ETipoUsuario.Medico,
            Medico = new MedicoRequest 
            { 
                CRM = "54321",
                EspecialidadeId = Guid.NewGuid()
            }
        };
        
  
        
        _mockUsuarioRepository.Setup(x => x.ObterPorIdAsync(request.Id))
            .ReturnsAsync(usuarioExistente);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(null as Usuario);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorCpfAsync(request.CPF))
            .ReturnsAsync(null as Usuario);
        
        _mockMedicoService.Setup(x => x.AlterarAsync(usuarioExistente.UsuarioId, request.Medico))
            .ReturnsAsync(new MedicoResponse(request.Medico.CRM, request.Medico.EspecialidadeId));
        
        // Act
        var resultado = await _pessoaService.AlterarAsync(request);
        
        // Assert
        Assert.NotNull(resultado);
        Assert.NotNull(resultado.Medico);
        Assert.Equal(request.Medico.CRM, resultado.Medico.CRM);
        Assert.Equal(request.Medico.EspecialidadeId, resultado.Medico.IdEspecialidade);
        _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Once);
        _mockMedicoService.Verify(x => x.AlterarAsync(usuarioExistente.UsuarioId, request.Medico), Times.Once);
    }
    [Fact]
    public async Task AlterarAsync_ComTipoMedicoSemDadosMedico_LancaExcecao()
    {
        // Arrange
        var request = new AlterarRequest 
        { 
            Id = Guid.NewGuid(),
            Nome = "Novo Medico", 
            Email = "medico@teste.com", 
            Senha = "Senha123!",
            CPF = "12345678901",
            TipoUsuario = ETipoUsuario.Medico,
            Medico = null
        };

        _mockUsuarioRepository.Setup(x => x.ObterPorIdAsync(request.Id))
        .ReturnsAsync(new Usuario(request.Nome,request.Email,request.Senha,request.CPF,ETipoUsuario.Medico));

        _mockUsuarioRepository.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(null as Usuario);
        
        _mockUsuarioRepository.Setup(x => x.ObterPorCpfAsync(request.CPF))
            .ReturnsAsync(null as Usuario);
        
        // Act & Assert
        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _pessoaService.AlterarAsync(request));
        Assert.Equal("Dados do médico são obrigatórios para o tipo de usuário 'Médico'", excecao.Message);
        _mockUsuarioRepository.Verify(x => x.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
    }
} 