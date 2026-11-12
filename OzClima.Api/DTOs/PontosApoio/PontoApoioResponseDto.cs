
using OzClima.Api.Enums;

namespace OzClima.Api.DTOs.PontosApoio;

public class PontoApoioResponseDto
{
    public int Id { get; set; }
    public int AreaMonitoradaId { get; set; }
    public string NomeArea { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public TipoPontoApoio Tipo { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool Ativo { get; set; }
}
