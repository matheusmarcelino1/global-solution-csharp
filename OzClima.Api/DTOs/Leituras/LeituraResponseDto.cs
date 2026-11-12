
using OzClima.Api.Enums;

namespace OzClima.Api.DTOs.Leituras;

public class LeituraResponseDto
{
    public int Id { get; set; }
    public int AreaMonitoradaId { get; set; }
    public decimal Temperatura { get; set; }
    public decimal Umidade { get; set; }
    public decimal Humidex { get; set; }
    public DateTime DataHora { get; set; }
    public NivelRiscoTermico NivelRisco { get; set; }
}
