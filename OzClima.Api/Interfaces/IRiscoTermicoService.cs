
using OzClima.Api.Enums;

namespace OzClima.Api.Interfaces;

public interface IRiscoTermicoService
{
    NivelRiscoTermico Calcular(
        decimal temperatura,
        decimal umidade);

    decimal CalcularHumidex(
        decimal temperatura,
        decimal umidade);
}
