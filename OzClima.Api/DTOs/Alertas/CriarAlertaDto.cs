
using OzClima.Api.Enums;

namespace OzClima.Api.DTOs.Alertas;

public class CriarAlertaDto
{
    public int AreaMonitoradaId { get; set; }
    public NivelRiscoTermico Nivel { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}
