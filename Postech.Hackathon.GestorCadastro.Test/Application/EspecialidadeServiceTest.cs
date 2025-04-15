using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Postech.Hackathon.GestorCadastro.Application.DTO.Response;
using Postech.Hackathon.GestorCadastro.Application.Services;
using Postech.Hackathon.GestorCadastro.Domain.Entities;
using Postech.Hackathon.GestorCadastro.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Xunit;

namespace Postech.Hackathon.GestorCadastro.Test.Application;

public class EspecialidadeServiceTest
{
    private readonly Mock<IEspecialidadeRepository> _mockEspecialidadeRepository;
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly EspecialidadeService _especialidadeService;
    
    public EspecialidadeServiceTest()
    {
        _mockEspecialidadeRepository = new Mock<IEspecialidadeRepository>();
        _mockCache = new Mock<IDistributedCache>();
        _especialidadeService = new EspecialidadeService(_mockEspecialidadeRepository.Object, _mockCache.Object);
    }

    [Fact]
    public async Task ObterTodasAsync_ComEspecialidadesExistentes_RetornaListaDeEspecialidades()
    {
        // Arrange
        var especialidades = new List<Especialidade>
        {
            new Especialidade("Cardiologia", "Especialidade em doenças do coração"),
            new Especialidade("Ortopedia", "Especialidade em doenças dos ossos")
        };

        _mockEspecialidadeRepository.Setup(x => x.ObterTodasAsync())
            .ReturnsAsync(especialidades);

        // Act
        var resultado = await _especialidadeService.ObterTodasAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        Assert.Contains(resultado, r => r.Nome == "Cardiologia");
        Assert.Contains(resultado, r => r.Nome == "Ortopedia");
        _mockEspecialidadeRepository.Verify(x => x.ObterTodasAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterTodasAsync_SemEspecialidades_RetornaListaVazia()
    {
        // Arrange
        _mockEspecialidadeRepository.Setup(x => x.ObterTodasAsync())
            .ReturnsAsync(new List<Especialidade>());

        // Act
        var resultado = await _especialidadeService.ObterTodasAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
        _mockEspecialidadeRepository.Verify(x => x.ObterTodasAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComEspecialidadeExistente_RetornaEspecialidade()
    {
        // Arrange
        var id = Guid.NewGuid();
        var especialidade = new Especialidade("Cardiologia", "Especialidade em doenças do coração");

        _mockEspecialidadeRepository.Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync(especialidade);

        // Act
        var resultado = await _especialidadeService.ObterPorIdAsync(id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(especialidade.Nome, resultado.Nome);
        Assert.Equal(especialidade.Descricao, resultado.Descricao);
        _mockEspecialidadeRepository.Verify(x => x.ObterPorIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComEspecialidadeInexistente_RetornaNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockEspecialidadeRepository.Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync(null as Especialidade);

        // Act
        var resultado = await _especialidadeService.ObterPorIdAsync(id);

        // Assert
        Assert.Null(resultado);
        _mockEspecialidadeRepository.Verify(x => x.ObterPorIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidos_RetornaEspecialidadeCriada()
    {
        // Arrange
        var nome = "Cardiologia";
        var descricao = "Especialidade em doenças do coração";
        var especialidade = new Especialidade(nome, descricao);

        _mockEspecialidadeRepository.Setup(x => x.CreateAsync(It.IsAny<Especialidade>()))
            .ReturnsAsync(especialidade);

        // Act
        var resultado = await _especialidadeService.CriarAsync(nome, descricao);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(nome, resultado.Nome);
        Assert.Equal(descricao, resultado.Descricao);
        _mockEspecialidadeRepository.Verify(x => x.CreateAsync(It.IsAny<Especialidade>()), Times.Once);
    }

    [Fact]
    public async Task ObterTodasAsync_QuandoExisteNoCache_RetornaDoCache()
    {
        // Arrange
        var especialidades = new List<EspecialidadeResponse>
        {
            new EspecialidadeResponse(
                Id: Guid.NewGuid(),
                Nome: "Cardiologia",
                Descricao: "Especialidade médica que trata do coração",
                DataCriacao: DateTime.Now
            )
        };

        var serializedData = JsonSerializer.Serialize(especialidades);
        _mockCache.Setup(x => x.GetStringAsync("especialidades_all", default))
            .ReturnsAsync(serializedData);

        // Act
        var resultado = await _especialidadeService.ObterTodasAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado);
        _mockEspecialidadeRepository.Verify(x => x.ObterTodasAsync(), Times.Never);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoExisteNoCache_RetornaDoCache()
    {
        // Arrange
        var id = Guid.NewGuid();
        var especialidade = new EspecialidadeResponse(
            Id: id,
            Nome: "Cardiologia",
            Descricao: "Especialidade médica que trata do coração",
            DataCriacao: DateTime.Now
        );

        var cacheKey = $"especialidade_{id}";
        var serializedData = JsonSerializer.Serialize(especialidade);
        _mockCache.Setup(x => x.GetStringAsync(cacheKey, default))
            .ReturnsAsync(serializedData);

        // Act
        var resultado = await _especialidadeService.ObterPorIdAsync(id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.EspecialidadeId);
        _mockEspecialidadeRepository.Verify(x => x.ObterPorIdAsync(id), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_QuandoSucesso_RemoveCacheDeTodasEspecialidades()
    {
        // Arrange
        var nome = "Cardiologia";
        var descricao = "Especialidade médica que trata do coração";
        var especialidade = new Especialidade(nome, descricao);
        
        _mockEspecialidadeRepository.Setup(x => x.CreateAsync(It.IsAny<Especialidade>()))
            .ReturnsAsync(especialidade);

        // Act
        await _especialidadeService.CriarAsync(nome, descricao);

        // Assert
        _mockCache.Verify(x => x.RemoveAsync("especialidades_all", default), Times.Once);
    }
} 