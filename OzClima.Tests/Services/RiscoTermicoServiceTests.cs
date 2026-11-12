
using OzClima.Api.Enums;
using OzClima.Api.Services;

namespace OzClima.Tests.Services;

public class RiscoTermicoServiceTests
{
    private readonly RiscoTermicoService _service = new();


    [Theory]
    [InlineData(29, 25, NivelRiscoTermico.Baixo)]
    [InlineData(25, 60, NivelRiscoTermico.Moderado)]
    [InlineData(29, 70, NivelRiscoTermico.Moderado)]
    [InlineData(33, 45, NivelRiscoTermico.Alto)]
    [InlineData(35, 50, NivelRiscoTermico.Alto)]
    [InlineData(34, 60, NivelRiscoTermico.Critico)]
    public void DeveClassificarCorretamenteOsLimitesDoHumidex(
        int temperatura,
        int umidade,
        NivelRiscoTermico nivelEsperado)
    {
        // Act
        var resultado = _service.Calcular(
            temperatura,
            umidade
        );

        // Assert
        Assert.Equal(nivelEsperado, resultado);
    }


    [Fact]
    public void DeveAumentarRiscoQuandoUmidadeAumentar()
    {
        // Arrange
        decimal temperatura = 30;

        // Act
        var riscoComBaixaUmidade = _service.Calcular(
            temperatura,
            30
        );

        var riscoComAltaUmidade = _service.Calcular(
            temperatura,
            80
        );

        // Assert
        Assert.Equal(
            NivelRiscoTermico.Moderado,
            riscoComBaixaUmidade
        );

        Assert.Equal(
            NivelRiscoTermico.Alto,
            riscoComAltaUmidade
        );

        Assert.True(
            riscoComAltaUmidade > riscoComBaixaUmidade
        );
    }


    [Fact]
    public void DeveCalcularHumidexComPrecisaoDeDuasCasas()
    {
        // Arrange
        decimal temperatura = 35;
        decimal umidade = 40;

        // Act
        var resultado = _service.CalcularHumidex(
            temperatura,
            umidade
        );

        // Assert
        Assert.Equal(41.91m, resultado);
    }



    [Fact]
    public void DeveRetornarAltoQuandoHumidexForAlto()
    {
        // 33°C com 60% de umidade
        // Humidex aproximado: 44,17
        var resultado = _service.Calcular(33, 60);

        Assert.Equal(NivelRiscoTermico.Alto, resultado);
    }

    [Fact]
    public void DeveRetornarBaixoComHumidexInferiorA30()
    {
        // 28°C com 30% de umidade
        // Humidex aproximado: 28,73
        var resultado = _service.Calcular(28, 30);

        Assert.Equal(NivelRiscoTermico.Baixo, resultado);
    }

    [Fact]
    public void DeveRetornarBaixoQuandoCalorForMenosIntenso()
    {
        // 27°C com 30% de umidade
        // Humidex aproximado: 27,37
        var resultado = _service.Calcular(27, 30);

        Assert.Equal(NivelRiscoTermico.Baixo, resultado);
    }

    [Fact]
    public void DeveRetornarCriticoQuandoHumidexForElevado()
    {
        // 38°C com 40% de umidade
        // Humidex aproximado: 47,12
        var resultado = _service.Calcular(38, 40);

        Assert.Equal(NivelRiscoTermico.Critico, resultado);
    }

    [Fact]
    public void DeveRetornarModeradoAntesDoNivelAlto()
    {
        // 33°C com 30% de umidade
        // Humidex aproximado: 35,81
        var resultado = _service.Calcular(33, 30);

        Assert.Equal(NivelRiscoTermico.Moderado, resultado);
    }


    [Fact]
    public void DeveRetornarModeradoQuandoTemperaturaFor30()
    {
        var resultado = _service.Calcular(30, 50);

        Assert.Equal(NivelRiscoTermico.Moderado, resultado);
    }

    [Fact]
    public void DeveRetornarAltoAntesDoLimiteCritico()
    {
        var resultado = _service.Calcular(35.99m, 40);

        Assert.Equal(NivelRiscoTermico.Alto, resultado);
    }

    [Fact]
    public void DeveLancarExcecaoQuandoTemperaturaForInvalida()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.Calcular(80, 50));
    }

    [Fact]
    public void DeveLancarExcecaoQuandoUmidadeForNegativa()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.Calcular(30, -1));
    }

    [Fact]
    public void DeveLancarExcecaoQuandoUmidadeForMaiorQue100()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.Calcular(30, 101));
    }
}
