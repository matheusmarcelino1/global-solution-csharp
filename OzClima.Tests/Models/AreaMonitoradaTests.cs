
using OzClima.Api.Enums;
using OzClima.Api.Models;

namespace OzClima.Tests.Models;

public class AreaMonitoradaTests
{
    [Fact]
    public void DeveCriarAreaComDadosValidos()
    {
        var area = new AreaMonitorada(
            "Centro de Osasco",
            "Centro",
            -23.532,
            -46.791,
            NivelArborizacao.Baixo
        );

        Assert.Equal("Centro de Osasco", area.Nome);
        Assert.Equal("Centro", area.Bairro);
        Assert.True(area.Ativa);
    }

    [Fact]
    public void DeveLancarExcecaoQuandoNomeForVazio()
    {
        Assert.Throws<ArgumentException>(() =>
            new AreaMonitorada(
                "",
                "Centro",
                -23.532,
                -46.791,
                NivelArborizacao.Baixo
            ));
    }

    [Fact]
    public void DeveLancarExcecaoQuandoLatitudeForInvalida()
    {
        Assert.Throws<ArgumentException>(() =>
            new AreaMonitorada(
                "Centro",
                "Centro",
                -150,
                -46.791,
                NivelArborizacao.Baixo
            ));
    }

    [Fact]
    public void DeveAtualizarDadosDaArea()
    {
        var area = CriarArea();

        area.Atualizar(
            "Centro Atualizado",
            "Centro",
            -23.533,
            -46.792,
            NivelArborizacao.Medio
        );

        Assert.Equal("Centro Atualizado", area.Nome);
        Assert.Equal(NivelArborizacao.Medio, area.NivelArborizacao);
    }

    [Fact]
    public void DeveDesativarArea()
    {
        var area = CriarArea();

        area.Desativar();

        Assert.False(area.Ativa);
    }

    private static AreaMonitorada CriarArea()
    {
        return new AreaMonitorada(
            "Centro de Osasco",
            "Centro",
            -23.532,
            -46.791,
            NivelArborizacao.Baixo
        );
    }
}
