
using OzClima.Api.Enums;

namespace OzClima.Api.DTOs.Areas;

public class AreaResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public NivelArborizacao NivelArborizacao { get; set; }
    public bool Ativa { get; set; }
}
