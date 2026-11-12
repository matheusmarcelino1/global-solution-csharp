
using Microsoft.EntityFrameworkCore;
using OzClima.Api.DTOs.Alertas;
using OzClima.Api.Enums;
using OzClima.Api.Models;
using OzClima.Api.Services;
using OzClima.Tests.Helpers;

namespace OzClima.Tests.Services;

public class AlertaCalorServiceTests
{
    private static async Task<AreaMonitorada> CriarAreaAsync(
        OzClima.Api.Data.AppDbContext context,
        string nome = "Centro de Osasco")
    {
        var area = new AreaMonitorada(
            nome,
            "Centro",
            -23.532,
            -46.791,
            NivelArborizacao.Baixo
        );

        context.AreasMonitoradas.Add(area);
        await context.SaveChangesAsync();

        return area;
    }

    private static CriarAlertaDto CriarDto(
        int areaId,
        NivelRiscoTermico nivel)
    {
        return new CriarAlertaDto
        {
            AreaMonitoradaId = areaId,
            Nivel = nivel,
            Mensagem = "Alerta de teste do OZClima."
        };
    }

    [Fact]
    public async Task DeveCriarAlertaAltoComSucesso()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new AlertaCalorService(context);

        var resultado = await service.CriarAsync(
            CriarDto(area.Id, NivelRiscoTermico.Alto));

        Assert.NotNull(resultado);
        Assert.True(resultado.Id > 0);
        Assert.Equal(NivelRiscoTermico.Alto, resultado.Nivel);
        Assert.Equal("Centro de Osasco", resultado.NomeArea);
        Assert.True(resultado.Ativo);

        var salvo = await context.AlertasCalor.FindAsync(resultado.Id);
        Assert.NotNull(salvo);
    }

    [Fact]
    public async Task DeveCriarAlertaCriticoComSucesso()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new AlertaCalorService(context);

        var resultado = await service.CriarAsync(
            CriarDto(area.Id, NivelRiscoTermico.Critico));

        Assert.NotNull(resultado);
        Assert.Equal(NivelRiscoTermico.Critico, resultado.Nivel);
        Assert.True(resultado.Ativo);
    }

    [Fact]
    public async Task NaoDeveCriarAlertaQuandoRiscoForBaixo()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new AlertaCalorService(context);

        var resultado = await service.CriarAsync(
            CriarDto(area.Id, NivelRiscoTermico.Baixo));

        Assert.Null(resultado);
        Assert.Equal(0, await context.AlertasCalor.CountAsync());
    }

    [Fact]
    public async Task NaoDeveCriarAlertaQuandoRiscoForModerado()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new AlertaCalorService(context);

        var resultado = await service.CriarAsync(
            CriarDto(area.Id, NivelRiscoTermico.Moderado));

        Assert.Null(resultado);
        Assert.Empty(await context.AlertasCalor.ToListAsync());
    }

    [Fact]
    public async Task NaoDeveDuplicarAlertaAtivoDoMesmoNivel()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new AlertaCalorService(context);

        var dto = CriarDto(area.Id, NivelRiscoTermico.Alto);

        var primeiro = await service.CriarAsync(dto);
        var segundo = await service.CriarAsync(dto);

        Assert.NotNull(primeiro);
        Assert.Null(segundo);

        var total = await context.AlertasCalor.CountAsync();
        Assert.Equal(1, total);
    }

    [Fact]
    public async Task DeveRejeitarAlertaQuandoAreaNaoExistir()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var service = new AlertaCalorService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CriarAsync(
                CriarDto(999, NivelRiscoTermico.Alto)));

        Assert.Empty(await context.AlertasCalor.ToListAsync());
    }

    [Fact]
    public async Task DeveRejeitarAlertaQuandoAreaEstiverInativa()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        area.Desativar();
        await context.SaveChangesAsync();

        var service = new AlertaCalorService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CriarAsync(
                CriarDto(area.Id, NivelRiscoTermico.Alto)));

        Assert.Equal(0, await context.AlertasCalor.CountAsync());
    }

    [Fact]
    public async Task DeveListarSomenteAlertasAtivos()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new AlertaCalorService(context);

        var alto = await service.CriarAsync(
            CriarDto(area.Id, NivelRiscoTermico.Alto));

        var critico = await service.CriarAsync(
            CriarDto(area.Id, NivelRiscoTermico.Critico));

        Assert.NotNull(alto);
        Assert.NotNull(critico);

        await service.EncerrarAsync(alto.Id);

        var ativos = (await service.ListarAtivosAsync()).ToList();

        Assert.Single(ativos);
        Assert.Equal(critico.Id, ativos[0].Id);
        Assert.True(ativos[0].Ativo);
    }

    [Fact]
    public async Task DeveListarSomenteAlertasDaAreaInformada()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area1 = await CriarAreaAsync(context, "Centro");
        var area2 = await CriarAreaAsync(context, "Vila Yara");

        var service = new AlertaCalorService(context);

        await service.CriarAsync(
            CriarDto(area1.Id, NivelRiscoTermico.Alto));

        await service.CriarAsync(
            CriarDto(area2.Id, NivelRiscoTermico.Critico));

        var resultado = (await service.ListarPorAreaAsync(area1.Id))
            .ToList();

        Assert.Single(resultado);
        Assert.All(resultado, x =>
            Assert.Equal(area1.Id, x.AreaMonitoradaId));
    }

    [Fact]
    public async Task DeveEncerrarAlertaComSucesso()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new AlertaCalorService(context);

        var alerta = await service.CriarAsync(
            CriarDto(area.Id, NivelRiscoTermico.Alto));

        Assert.NotNull(alerta);

        var encerrado = await service.EncerrarAsync(alerta.Id);

        context.ChangeTracker.Clear();

        var registro = await context.AlertasCalor
            .AsNoTracking()
            .FirstAsync(x => x.Id == alerta.Id);

        Assert.True(encerrado);
        Assert.False(registro.Ativo);
    }

    [Fact]
    public async Task DeveRetornarFalseQuandoAlertaJaEstiverEncerrado()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new AlertaCalorService(context);

        var alerta = await service.CriarAsync(
            CriarDto(area.Id, NivelRiscoTermico.Alto));

        Assert.NotNull(alerta);

        var primeiro = await service.EncerrarAsync(alerta.Id);
        var segundo = await service.EncerrarAsync(alerta.Id);

        Assert.True(primeiro);
        Assert.False(segundo);
    }

    [Fact]
    public async Task DeveListarAlertasAtivosEEncerrados()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new AlertaCalorService(context);

        var alto = await service.CriarAsync(
            CriarDto(area.Id, NivelRiscoTermico.Alto));

        await service.CriarAsync(
            CriarDto(area.Id, NivelRiscoTermico.Critico));

        Assert.NotNull(alto);

        await service.EncerrarAsync(alto.Id);

        var alertas = (await service.ListarAsync()).ToList();

        Assert.Equal(2, alertas.Count);
        Assert.Single(alertas.Where(x => x.Ativo));
        Assert.Single(alertas.Where(x => !x.Ativo));
    }
}
