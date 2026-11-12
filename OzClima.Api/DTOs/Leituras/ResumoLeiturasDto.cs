
namespace OzClima.Api.DTOs.Leituras;

public class ResumoLeiturasDto
{
    public int AreaMonitoradaId { get; set; }
    public int TotalLeituras { get; set; }
    public decimal TemperaturaMedia { get; set; }
    public decimal TemperaturaMaxima { get; set; }
    public decimal TemperaturaMinima { get; set; }
    public decimal UmidadeMedia { get; set; }
}
