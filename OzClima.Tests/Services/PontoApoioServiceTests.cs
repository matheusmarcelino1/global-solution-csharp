
using Microsoft.EntityFrameworkCore;
using OzClima.Api.Data;
using OzClima.Api.DTOs.PontosApoio;
using OzClima.Api.Enums;
using OzClima.Api.Models;
using OzClima.Api.Services;
using OzClima.Tests.Helpers;

namespace OzClima.Tests.Services;

public class PontoApoioServiceTests
{
    private static async Task<AreaMonitorada> CriarAreaAsync(
        AppDbContext context,
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

    private static CriarPontoApoioDto CriarDto(
        int areaId,
        TipoPontoApoio tipo = TipoPontoApoio.Hidratacao)
    {
        return new CriarPontoApoioDto
        {
            AreaMonitoradaId = areaId,
            Nome = "Ponto de Apoio Teste",
            Endereco = "Endereço fictício - Osasco",
            Tipo = tipo,
            Latitude = -23.532,
            Longitude = -46.791
        };
    }

    [Fact]
    public async Task DeveCadastrarPontoComSucesso()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new PontoApoioService(context);

        var resultado = await service.CriarAsync(CriarDto(area.Id));

        Assert.True(resultado.Id > 0);
        Assert.Equal(area.Id, resultado.AreaMonitoradaId);
        Assert.Equal(area.Nome, resultado.NomeArea);
        Assert.Equal(TipoPontoApoio.Hidratacao, resultado.Tipo);
        Assert.True(resultado.Ativo);

        context.ChangeTracker.Clear();

        var registro = await context.PontosApoio
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == resultado.Id);

        Assert.NotNull(registro);
        Assert.Equal(resultado.Nome, registro.Nome);
    }

    [Fact]
    public async Task DeveRejeitarPontoQuandoAreaNaoExistir()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var service = new PontoApoioService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CriarAsync(CriarDto(999)));

        Assert.Equal(0, await context.PontosApoio.CountAsync());
    }

    [Fact]
    public async Task DeveRejeitarPontoQuandoAreaEstiverInativa()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        area.Desativar();
        await context.SaveChangesAsync();

        var service = new PontoApoioService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CriarAsync(CriarDto(area.Id)));

        Assert.Empty(await context.PontosApoio.ToListAsync());
    }

    [Fact]
    public async Task DeveRejeitarPontoQuandoNomeForVazio()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new PontoApoioService(context);

        var dto = CriarDto(area.Id);
        dto.Nome = "";

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CriarAsync(dto));

        Assert.Empty(await context.PontosApoio.ToListAsync());
    }

    [Fact]
    public async Task DeveRejeitarPontoComTipoInvalido()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new PontoApoioService(context);

        var dto = CriarDto(area.Id);
        dto.Tipo = (TipoPontoApoio)999;

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CriarAsync(dto));

        Assert.Equal(0, await context.PontosApoio.CountAsync());
    }

    [Fact]
    public async Task DeveListarSomentePontosAtivos()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new PontoApoioService(context);

        var hidratacao = await service.CriarAsync(
            CriarDto(area.Id, TipoPontoApoio.Hidratacao));

        var areaVerde = await service.CriarAsync(
            CriarDto(area.Id, TipoPontoApoio.AreaVerde));

        await service.DesativarAsync(hidratacao.Id);

        var resultado = (await service.ListarAsync()).ToList();

        Assert.Single(resultado);
        Assert.Equal(areaVerde.Id, resultado[0].Id);
        Assert.True(resultado[0].Ativo);
    }

    [Fact]
    public async Task DeveBuscarPontoPorId()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new PontoApoioService(context);

        var ponto = await service.CriarAsync(CriarDto(area.Id));

        var resultado = await service.BuscarPorIdAsync(ponto.Id);

        Assert.NotNull(resultado);
        Assert.Equal(ponto.Id, resultado.Id);
        Assert.Equal("Ponto de Apoio Teste", resultado.Nome);
    }

    [Fact]
    public async Task DeveRetornarNullQuandoPontoNaoExistir()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var service = new PontoApoioService(context);

        var resultado = await service.BuscarPorIdAsync(999);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task DeveListarSomentePontosDaAreaInformada()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area1 = await CriarAreaAsync(context, "Centro");
        var area2 = await CriarAreaAsync(context, "Vila Yara");

        var service = new PontoApoioService(context);

        await service.CriarAsync(
            CriarDto(area1.Id, TipoPontoApoio.Hidratacao));

        await service.CriarAsync(
            CriarDto(area1.Id, TipoPontoApoio.AreaVerde));

        await service.CriarAsync(
            CriarDto(area2.Id, TipoPontoApoio.UnidadeSaude));

        var resultado = (await service.ListarPorAreaAsync(area1.Id))
            .ToList();

        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, x =>
            Assert.Equal(area1.Id, x.AreaMonitoradaId));
    }

    [Fact]
    public async Task DeveFiltrarPontosPorTipo()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new PontoApoioService(context);

        await service.CriarAsync(
            CriarDto(area.Id, TipoPontoApoio.Hidratacao));

        await service.CriarAsync(
            CriarDto(area.Id, TipoPontoApoio.AreaVerde));

        await service.CriarAsync(
            CriarDto(area.Id, TipoPontoApoio.Hidratacao));

        var resultado = (await service.ListarPorTipoAsync(
            TipoPontoApoio.Hidratacao)).ToList();

        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, x =>
            Assert.Equal(TipoPontoApoio.Hidratacao, x.Tipo));
    }

    [Fact]
    public async Task DeveDesativarPontoSemExcluirRegistro()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new PontoApoioService(context);

        var ponto = await service.CriarAsync(CriarDto(area.Id));

        var desativado = await service.DesativarAsync(ponto.Id);

        context.ChangeTracker.Clear();

        var consulta = await service.BuscarPorIdAsync(ponto.Id);

        var registro = await context.PontosApoio
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == ponto.Id);

        Assert.True(desativado);
        Assert.Null(consulta);
        Assert.NotNull(registro);
        Assert.False(registro.Ativo);
    }

    [Fact]
    public async Task DeveRetornarFalseAoDesativarNovamente()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new PontoApoioService(context);

        var ponto = await service.CriarAsync(CriarDto(area.Id));

        var primeira = await service.DesativarAsync(ponto.Id);
        var segunda = await service.DesativarAsync(ponto.Id);

        Assert.True(primeira);
        Assert.False(segunda);
    }

    [Fact]
    public async Task NaoDeveListarPontosDeAreaDesativada()
    {
        var (context, connection) = TestDbContextFactory.CriarContexto();
        using var conn = connection;
        using var db = context;

        var area = await CriarAreaAsync(context);
        var service = new PontoApoioService(context);

        var ponto = await service.CriarAsync(CriarDto(area.Id));

        area.Desativar();
        await context.SaveChangesAsync();

        var resultado = (await service.ListarAsync()).ToList();
        var consultaPorId = await service.BuscarPorIdAsync(ponto.Id);

        Assert.Empty(resultado);
        Assert.Null(consultaPorId);

        // O registro permanece armazenado no SQLite.
        Assert.Equal(1, await context.PontosApoio.CountAsync());
    }
}
