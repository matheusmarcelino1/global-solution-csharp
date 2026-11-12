
using Microsoft.EntityFrameworkCore;
using Moq;
using OzClima.Api.DTOs.Alertas;
using OzClima.Api.DTOs.Leituras;
using OzClima.Api.Enums;
using OzClima.Api.Interfaces;
using OzClima.Api.Models;
using OzClima.Api.Services;
using OzClima.Tests.Helpers;

namespace OzClima.Tests.Services;

public class LeituraClimaticaServiceTests
{
    private static AreaMonitorada CriarArea(
        string nome = "Centro de Osasco")
    {
        return new AreaMonitorada(
            nome,
            "Centro",
            -23.532,
            -46.791,
            NivelArborizacao.Baixo
        );
    }

    private static CriarLeituraDto CriarDto(
        int areaId,
        decimal temperatura = 28,
        decimal umidade = 60)
    {
        return new CriarLeituraDto
        {
            AreaMonitoradaId = areaId,
            Temperatura = temperatura,
            Umidade = umidade
        };
    }


    [Fact]
    public async Task DeveDesfazerLeituraQuandoProcessamentoDoAlertaFalhar()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using var conn = connection;
        using var db = context;

        var area = CriarArea();

        context.AreasMonitoradas.Add(area);
        await context.SaveChangesAsync();

        var alertaMock = new Mock<IAlertaCalorService>();

        alertaMock
            .Setup(x => x.ProcessarLeituraAsync(
                It.IsAny<int>(),
                It.IsAny<NivelRiscoTermico>(),
                It.IsAny<NivelRiscoTermico?>()))
            .ThrowsAsync(new InvalidOperationException(
                "Falha simulada no processamento do alerta."
            ));

        var service = new LeituraClimaticaService(
            context,
            new RiscoTermicoService(),
            alertaMock.Object
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CriarAsync(
                CriarDto(area.Id, 35, 40)
            )
        );

        context.ChangeTracker.Clear();

        var total = await context.LeiturasClimaticas.CountAsync();

        Assert.Equal(0, total);
    }



    [Fact]
    public async Task DeveProcessarAlertaQuandoRiscoForCritico()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using var conn = connection;
        using var db = context;

        var area = CriarArea();

        context.AreasMonitoradas.Add(area);
        await context.SaveChangesAsync();

        var alertaMock = new Mock<IAlertaCalorService>();

        alertaMock
            .Setup(x => x.ProcessarLeituraAsync(
                It.IsAny<int>(),
                It.IsAny<NivelRiscoTermico>(),
                It.IsAny<NivelRiscoTermico?>()))
            .Returns(Task.CompletedTask);

        var service = new LeituraClimaticaService(
            context,
            new RiscoTermicoService(),
            alertaMock.Object
        );

        var resultado = await service.CriarAsync(
    CriarDto(area.Id, 38, 40)
        );

        Assert.Equal(NivelRiscoTermico.Critico, resultado.NivelRisco);

        alertaMock.Verify(
            x => x.ProcessarLeituraAsync(
                area.Id,
                NivelRiscoTermico.Critico,
                null),
            Times.Once
        );
    }


    [Fact]
    public async Task DeveProcessarAlertaQuandoRiscoForAlto()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using var conn = connection;
        using var db = context;

        var area = CriarArea();

        context.AreasMonitoradas.Add(area);
        await context.SaveChangesAsync();

        var alertaMock = new Mock<IAlertaCalorService>();

        alertaMock
            .Setup(x => x.ProcessarLeituraAsync(
                It.IsAny<int>(),
                It.IsAny<NivelRiscoTermico>(),
                It.IsAny<NivelRiscoTermico?>()))
            .Returns(Task.CompletedTask);

        var service = new LeituraClimaticaService(
            context,
            new RiscoTermicoService(),
            alertaMock.Object
        );

        var resultado = await service.CriarAsync(
            CriarDto(area.Id, 35, 40)
        );

        Assert.Equal(NivelRiscoTermico.Alto, resultado.NivelRisco);

        alertaMock.Verify(
            x => x.ProcessarLeituraAsync(
                area.Id,
                NivelRiscoTermico.Alto,
                null),
            Times.Once
        );
    }


    [Fact]
    public async Task DeveProcessarLeituraMesmoQuandoRiscoForBaixo()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using var conn = connection;
        using var db = context;

        var area = CriarArea();

        context.AreasMonitoradas.Add(area);
        await context.SaveChangesAsync();

        var alertaMock = new Mock<IAlertaCalorService>();

        alertaMock
            .Setup(x => x.ProcessarLeituraAsync(
                It.IsAny<int>(),
                It.IsAny<NivelRiscoTermico>(),
                It.IsAny<NivelRiscoTermico?>()))
            .Returns(Task.CompletedTask);

        var service = new LeituraClimaticaService(
            context,
            new RiscoTermicoService(),
            alertaMock.Object
        );

        await service.CriarAsync(
    CriarDto(area.Id, 27, 30)
        );

        alertaMock.Verify(
            x => x.ProcessarLeituraAsync(
                area.Id,
                NivelRiscoTermico.Baixo,
                null),
            Times.Once
        );
    }


    [Fact]
    public async Task DeveCadastrarLeituraComSucesso()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using (connection)
        using (context)
        {
            var area = CriarArea();

            context.AreasMonitoradas.Add(area);
            await context.SaveChangesAsync();

            var alertaMock = new Mock<IAlertaCalorService>();

            var service = new LeituraClimaticaService(
                context,
                new RiscoTermicoService(),
                alertaMock.Object
            );

            // Act
            var resultado = await service.CriarAsync(
    CriarDto(area.Id, 28, 30)
);

            // Assert
            Assert.True(resultado.Id > 0);
            Assert.Equal(area.Id, resultado.AreaMonitoradaId);
            Assert.Equal(28, resultado.Temperatura);
            Assert.Equal(NivelRiscoTermico.Baixo, resultado.NivelRisco);

            var leituraSalva = await context.LeiturasClimaticas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == resultado.Id);

            Assert.NotNull(leituraSalva);
        }
    }

    [Fact]
    public async Task DeveRejeitarLeituraQuandoAreaNaoExistir()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using (connection)
        using (context)
        {
            var alertaMock = new Mock<IAlertaCalorService>();

            var service = new LeituraClimaticaService(
                context,
                new RiscoTermicoService(),
                alertaMock.Object
            );

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.CriarAsync(
                    CriarDto(999, 35, 40)
                )
            );

            Assert.Empty(await context.LeiturasClimaticas.ToListAsync());
        }
    }

    [Fact]
    public async Task DeveRejeitarLeituraQuandoAreaEstiverInativa()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using (connection)
        using (context)
        {
            var area = CriarArea();
            area.Desativar();

            context.AreasMonitoradas.Add(area);
            await context.SaveChangesAsync();

            var alertaMock = new Mock<IAlertaCalorService>();

            var service = new LeituraClimaticaService(
                context,
                new RiscoTermicoService(),
                alertaMock.Object
            );

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.CriarAsync(
                    CriarDto(area.Id, 35, 40)
                )
            );

            Assert.Empty(await context.LeiturasClimaticas.ToListAsync());
        }
    }

    [Fact]
    public async Task NaoDeveGerarAlertaQuandoRiscoForBaixo()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using (connection)
        using (context)
        {
            var area = CriarArea();

            context.AreasMonitoradas.Add(area);
            await context.SaveChangesAsync();

            var alertaMock = new Mock<IAlertaCalorService>();

            var service = new LeituraClimaticaService(
                context,
                new RiscoTermicoService(),
                alertaMock.Object
            );

            // Act
            await service.CriarAsync(CriarDto(area.Id, 27, 60));

            // Assert
            alertaMock.Verify(
                x => x.CriarAsync(It.IsAny<CriarAlertaDto>()),
                Times.Never
            );
        }
    }

    [Fact]
    public async Task DeveListarSomenteLeiturasDaAreaInformada()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using (connection)
        using (context)
        {
            var area1 = CriarArea("Centro");
            var area2 = CriarArea("Vila Yara");

            context.AreasMonitoradas.AddRange(area1, area2);
            await context.SaveChangesAsync();

            context.LeiturasClimaticas.AddRange(
                new LeituraClimatica(area1.Id, 28, 60),
                new LeituraClimatica(area1.Id, 29, 55),
                new LeituraClimatica(area2.Id, 31, 50)
            );

            await context.SaveChangesAsync();

            var alertaMock = new Mock<IAlertaCalorService>();

            var service = new LeituraClimaticaService(
                context,
                new RiscoTermicoService(),
                alertaMock.Object
            );

            // Act
            var resultado = (await service.ListarPorAreaAsync(area1.Id))
                .ToList();

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, x =>
                Assert.Equal(area1.Id, x.AreaMonitoradaId));
        }
    }

    [Fact]
    public async Task DeveCalcularResumoClimaticoCorretamente()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using (connection)
        using (context)
        {
            var area = CriarArea();

            context.AreasMonitoradas.Add(area);
            await context.SaveChangesAsync();

            context.LeiturasClimaticas.AddRange(
                new LeituraClimatica(area.Id, 28, 60),
                new LeituraClimatica(area.Id, 32, 50),
                new LeituraClimatica(area.Id, 36, 40)
            );

            await context.SaveChangesAsync();

            var alertaMock = new Mock<IAlertaCalorService>();

            var service = new LeituraClimaticaService(
                context,
                new RiscoTermicoService(),
                alertaMock.Object
            );

            // Act
            var resumo = await service.ObterResumoPorAreaAsync(area.Id);

            // Assert
            Assert.NotNull(resumo);
            Assert.Equal(3, resumo.TotalLeituras);
            Assert.Equal(32, resumo.TemperaturaMedia);
            Assert.Equal(36, resumo.TemperaturaMaxima);
            Assert.Equal(28, resumo.TemperaturaMinima);
            Assert.Equal(50, resumo.UmidadeMedia);
        }
    }
}
