
using Microsoft.EntityFrameworkCore;
using OzClima.Api.Data;
using OzClima.Api.DTOs.Leituras;
using OzClima.Api.Enums;
using OzClima.Api.Models;
using OzClima.Api.Services;
using OzClima.Tests.Helpers;

namespace OzClima.Tests.Services;

public class CicloVidaAlertasTests
{
    private static async Task<int> CriarAreaAsync(
        AppDbContext context)
    {
        var area = new AreaMonitorada(
            "Centro de Osasco",
            "Centro",
            -23.532,
            -46.791,
            NivelArborizacao.Baixo
        );

        context.AreasMonitoradas.Add(area);
        await context.SaveChangesAsync();

        return area.Id;
    }

    private static LeituraClimaticaService CriarService(
        AppDbContext context)
    {
        var riscoService = new RiscoTermicoService();
        var alertaService = new AlertaCalorService(context);

        return new LeituraClimaticaService(
            context,
            riscoService,
            alertaService
        );
    }

    private static async Task RegistrarAsync(
        LeituraClimaticaService service,
        int areaId,
        decimal temperatura)
    {
        await service.CriarAsync(new CriarLeituraDto
        {
            AreaMonitoradaId = areaId,
            Temperatura = temperatura,
            Umidade = 40
        });
    }

    [Fact]
    public async Task DeveCriarAlertaAltoNaPrimeiraLeitura()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using var conn = connection;
        using var db = context;

        var areaId = await CriarAreaAsync(context);
        var service = CriarService(context);

        await RegistrarAsync(service, areaId, 35);

        var alertas = await context.AlertasCalor
            .AsNoTracking()
            .ToListAsync();

        Assert.Single(alertas);
        Assert.Equal(NivelRiscoTermico.Alto, alertas[0].Nivel);
        Assert.True(alertas[0].Ativo);
    }

    [Fact]
    public async Task DeveSubstituirAlertaAltoPorCritico()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using var conn = connection;
        using var db = context;

        var areaId = await CriarAreaAsync(context);
        var service = CriarService(context);

        await RegistrarAsync(service, areaId, 35);
        await RegistrarAsync(service, areaId, 38);

        var alertas = await context.AlertasCalor
            .AsNoTracking()
            .ToListAsync();

        Assert.Equal(2, alertas.Count);

        Assert.Contains(alertas, a =>
            a.Nivel == NivelRiscoTermico.Alto && !a.Ativo);

        Assert.Contains(alertas, a =>
            a.Nivel == NivelRiscoTermico.Critico && a.Ativo);

        Assert.Single(alertas.Where(a => a.Ativo));
    }

    [Fact]
    public async Task NaoDeveEncerrarAlertaComUmaUnicaReducao()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using var conn = connection;
        using var db = context;

        var areaId = await CriarAreaAsync(context);
        var service = CriarService(context);

        await RegistrarAsync(service, areaId, 38);
        await RegistrarAsync(service, areaId, 31);

        var alertasAtivos = await context.AlertasCalor
            .AsNoTracking()
            .Where(a => a.Ativo)
            .ToListAsync();

        Assert.Single(alertasAtivos);
        Assert.Equal(
            NivelRiscoTermico.Critico,
            alertasAtivos[0].Nivel
        );
    }

    [Fact]
    public async Task DeveEncerrarAlertaAposDuasReducoes()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using var conn = connection;
        using var db = context;

        var areaId = await CriarAreaAsync(context);
        var service = CriarService(context);

        await RegistrarAsync(service, areaId, 38);
        await RegistrarAsync(service, areaId, 31);
        await RegistrarAsync(service, areaId, 30);

        var alertasAtivos = await context.AlertasCalor
            .AsNoTracking()
            .CountAsync(a => a.Ativo);

        var historico = await context.AlertasCalor
            .AsNoTracking()
            .ToListAsync();

        Assert.Equal(0, alertasAtivos);
        Assert.Single(historico);
        Assert.False(historico[0].Ativo);
    }

    [Fact]
    public async Task DeveReduzirDeCriticoParaAlto()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using var conn = connection;
        using var db = context;

        var areaId = await CriarAreaAsync(context);
        var service = CriarService(context);

        await RegistrarAsync(service, areaId, 38);
        await RegistrarAsync(service, areaId, 35);
        await RegistrarAsync(service, areaId, 34);

        var alertas = await context.AlertasCalor
            .AsNoTracking()
            .ToListAsync();

        Assert.Equal(2, alertas.Count);

        Assert.Contains(alertas, a =>
            a.Nivel == NivelRiscoTermico.Critico && !a.Ativo);

        Assert.Contains(alertas, a =>
            a.Nivel == NivelRiscoTermico.Alto && a.Ativo);

        Assert.Single(alertas.Where(a => a.Ativo));
    }

    [Fact]
    public async Task NaoDeveCriarAlertaDuplicadoNoMesmoNivel()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using var conn = connection;
        using var db = context;

        var areaId = await CriarAreaAsync(context);
        var service = CriarService(context);

        await RegistrarAsync(service, areaId, 35);
        await RegistrarAsync(service, areaId, 34);
        await RegistrarAsync(service, areaId, 35);

        var alertas = await context.AlertasCalor
            .AsNoTracking()
            .ToListAsync();

        Assert.Single(alertas);
        Assert.True(alertas[0].Ativo);
        Assert.Equal(NivelRiscoTermico.Alto, alertas[0].Nivel);

        Assert.Equal(
            3,
            await context.LeiturasClimaticas.CountAsync()
        );
    }
}
