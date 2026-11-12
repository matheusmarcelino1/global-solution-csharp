
using OzClima.Api.Enums;

namespace OzClima.Api.DTOs.Alertas;

public class AlertaResponseDto
{
    public int Id { get; set; }
    public int AreaMonitoradaId { get; set; }
    public string NomeArea { get; set; } = string.Empty;
    public NivelRiscoTermico Nivel { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public bool Ativo { get; set; }
}
