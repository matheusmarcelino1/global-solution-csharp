
using OzClima.Api.Enums;
using OzClima.Api.Interfaces;

namespace OzClima.Api.Services;

public class RiscoTermicoService : IRiscoTermicoService
{
    public NivelRiscoTermico Calcular(
        decimal temperatura,
        decimal umidade)
    {
        var humidex = CalcularHumidex(temperatura, umidade);

        var indice = Math.Round(
            humidex,
            0,
            MidpointRounding.AwayFromZero);

        if (indice >= 46)
            return NivelRiscoTermico.Critico;

        if (indice >= 40)
            return NivelRiscoTermico.Alto;

        if (indice >= 30)
            return NivelRiscoTermico.Moderado;

        return NivelRiscoTermico.Baixo;
    }

    public decimal CalcularHumidex(
        decimal temperatura,
        decimal umidade)
    {
        Validar(temperatura, umidade);

        double t = (double)temperatura;
        double ur = (double)umidade;

        // Pressão de vapor estimada em hPa
        double pressaoVapor =
            6.112 *
            Math.Pow(10, (7.5 * t) / (237.7 + t)) *
            (ur / 100.0);

        // Índice Humidex
        double humidex =
            t + (5.0 / 9.0) * (pressaoVapor - 10.0);

        return Math.Round(
            (decimal)humidex,
            2,
            MidpointRounding.AwayFromZero);
    }

    private static void Validar(
        decimal temperatura,
        decimal umidade)
    {
        if (temperatura < -20 || temperatura > 60)
            throw new ArgumentException(
                "A temperatura deve estar entre -20°C e 60°C.");

        if (umidade < 0 || umidade > 100)
            throw new ArgumentException(
                "A umidade deve estar entre 0% e 100%.");
    }
}
