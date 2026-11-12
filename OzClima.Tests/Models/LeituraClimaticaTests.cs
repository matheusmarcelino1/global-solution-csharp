
using OzClima.Api.Models;

namespace OzClima.Tests.Models;

public class LeituraClimaticaTests
{
    [Fact]
    public void DeveCriarLeituraComDadosValidos()
    {
        var leitura = new LeituraClimatica(1, 34.5m, 40);

        Assert.Equal(1, leitura.AreaMonitoradaId);
        Assert.Equal(34.5m, leitura.Temperatura);
        Assert.Equal(40, leitura.Umidade);
        Assert.True(leitura.DataHora <= DateTime.UtcNow);
    }

    [Fact]
    public void DeveLancarExcecaoQuandoTemperaturaForInvalida()
    {
        Assert.Throws<ArgumentException>(() =>
            new LeituraClimatica(1, 80, 40));
    }

    [Fact]
    public void DeveLancarExcecaoQuandoUmidadeForNegativa()
    {
        Assert.Throws<ArgumentException>(() =>
            new LeituraClimatica(1, 30, -10));
    }

    [Fact]
    public void DeveLancarExcecaoQuandoUmidadeForMaiorQue100()
    {
        Assert.Throws<ArgumentException>(() =>
            new LeituraClimatica(1, 30, 110));
    }

    [Fact]
    public void DeveLancarExcecaoQuandoAreaForInvalida()
    {
        Assert.Throws<ArgumentException>(() =>
            new LeituraClimatica(0, 30, 50));
    }
}
