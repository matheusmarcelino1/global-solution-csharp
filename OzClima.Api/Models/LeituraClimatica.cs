
namespace OzClima.Api.Models;

public class LeituraClimatica
{
    public int Id { get; private set; }
    public int AreaMonitoradaId { get; private set; }
    public decimal Temperatura { get; private set; }
    public decimal Umidade { get; private set; }
    public DateTime DataHora { get; private set; }

    public AreaMonitorada AreaMonitorada { get; private set; } = null!;

    private LeituraClimatica() { }

    public LeituraClimatica(
        int areaMonitoradaId,
        decimal temperatura,
        decimal umidade)
    {
        if (areaMonitoradaId <= 0)
            throw new ArgumentException("A área monitorada é inválida.");

        if (temperatura < -20 || temperatura > 60)
            throw new ArgumentException(
                "A temperatura deve estar entre -20°C e 60°C.");

        if (umidade < 0 || umidade > 100)
            throw new ArgumentException(
                "A umidade deve estar entre 0% e 100%.");

        AreaMonitoradaId = areaMonitoradaId;
        Temperatura = temperatura;
        Umidade = umidade;
        DataHora = DateTime.UtcNow;
    }
}
