using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Postech.Hackathon.GestorCadastro.Application.DTO.Request;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Services;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Postech.Hackathon.GestorCadastro.Test.Application;

public class MedicoServiceTest
{
    private readonly Mock<IMedicoRepository> _mockMedicoRepository;
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly MedicoService _medicoService;
    
    public MedicoServiceTest()
    {
        _mockMedicoRepository = new Mock<IMedicoRepository>();
        _mockUsuarioRepository = new Mock<IUsuarioRepository>();
        _mockCache = new Mock<IDistributedCache>();
        _medicoService = new MedicoService(_mockMedicoRepository.Object, _mockUsuarioRepository.Object, _mockCache.Object);
    }

    [Fact]
    public async Task CadastrarAsync_ComDadosValidos_RetornaMedicoResponse()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var request = new MedicoRequest 
        { 
            CRM = "123456",
            EspecialidadeId = Guid.NewGuid(),
            ValorConsulta = 150.00m
        };
        
        _mockMedicoRepository.Setup(x => x.ObterPorCrmAsync(request.CRM))
            .ReturnsAsync(null as Medico);
        
        // Act
        var resultado = await _medicoService.CadastrarAsync(idUsuario, request);
        
        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(request.CRM, resultado.CRM);
        Assert.Equal(request.EspecialidadeId, resultado.IdEspecialidade);
        _mockMedicoRepository.Verify(x => x.CreateAsync(It.IsAny<Medico>()), Times.Once);
    }
    
    [Fact]
    public async Task CadastrarAsync_ComCrmJaExistente_LancaExcecao()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var request = new MedicoRequest 
        { 
            CRM = "123456",
            EspecialidadeId = Guid.NewGuid(),
            ValorConsulta = 150.00m
        };
        
        var medicoExistente = new Medico(
            request.CRM,
            Guid.NewGuid(),
            request.EspecialidadeId,
            150.00m);
        
        _mockMedicoRepository.Setup(x => x.ObterPorCrmAsync(request.CRM))
            .ReturnsAsync(medicoExistente);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _medicoService.CadastrarAsync(idUsuario, request));
        _mockMedicoRepository.Verify(x => x.CreateAsync(It.IsAny<Medico>()), Times.Never);
    }

    [Fact]
    public async Task AlterarAsync_ComDadosValidos_RetornaMedicoResponse()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var request = new MedicoRequest 
        { 
            CRM = "123456",
            EspecialidadeId = Guid.NewGuid(),
            ValorConsulta = 150.00m
        };
        
        var medicoExistente = new Medico(
            "654321",
            idUsuario,
            Guid.NewGuid(),
            150.00m);
        
        _mockMedicoRepository.Setup(x => x.ObterPorIdAsync(idUsuario))
            .ReturnsAsync(medicoExistente);
        
        _mockMedicoRepository.Setup(x => x.ObterPorCrmAsync(request.CRM))
            .ReturnsAsync(null as Medico);
        
        // Act
        var resultado = await _medicoService.AlterarAsync(idUsuario, request);
        
        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(request.CRM, resultado.CRM);
        Assert.Equal(request.EspecialidadeId, resultado.IdEspecialidade);
        _mockMedicoRepository.Verify(x => x.UpdateAsync(It.IsAny<Medico>()), Times.Once);
    }
    
    [Fact]
    public async Task AlterarAsync_ComMedicoInexistente_LancaExcecao()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var request = new MedicoRequest 
        { 
            CRM = "123456",
            EspecialidadeId = Guid.NewGuid(),
            ValorConsulta = 150.00m
        };
        
        _mockMedicoRepository.Setup(x => x.ObterPorIdAsync(idUsuario))
            .ReturnsAsync(null as Medico);
        
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _medicoService.AlterarAsync(idUsuario, request));
        _mockMedicoRepository.Verify(x => x.UpdateAsync(It.IsAny<Medico>()), Times.Never);
    }
    
    [Fact]
    public async Task AlterarAsync_ComCrmJaExistente_LancaExcecao()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var request = new MedicoRequest 
        { 
            CRM = "123456",
            EspecialidadeId = Guid.NewGuid(),
            ValorConsulta = 150.00m
        };
        
        var medicoExistente = new Medico(
            "654321",
            idUsuario,
            Guid.NewGuid(),
            150.00m);
        
        var outroMedico = new Medico(
            request.CRM,
            Guid.NewGuid(),
            request.EspecialidadeId,
            150.00m);
        
        _mockMedicoRepository.Setup(x => x.ObterPorIdAsync(idUsuario))
            .ReturnsAsync(medicoExistente);
        
        _mockMedicoRepository.Setup(x => x.ObterPorCrmAsync(request.CRM))
            .ReturnsAsync(outroMedico);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _medicoService.AlterarAsync(idUsuario, request));
        _mockMedicoRepository.Verify(x => x.UpdateAsync(It.IsAny<Medico>()), Times.Never);
    }



    [Fact]
    public async Task CadastrarAsync_QuandoSucesso_RemoveCacheDaEspecialidade()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var especialidadeId = Guid.NewGuid();
        var request = new MedicoRequest 
        { 
            CRM = "123456",
            EspecialidadeId = especialidadeId,
            ValorConsulta = 150.00m
        };

        var cacheKey = $"medicos_especialidade_{especialidadeId}";
        _mockMedicoRepository.Setup(x => x.ObterPorCrmAsync(request.CRM))
            .ReturnsAsync(null as Medico);

        // Act
        await _medicoService.CadastrarAsync(idUsuario, request);

        // Assert
        _mockCache.Verify(x => x.RemoveAsync(cacheKey, default), Times.Once);
    }
} 