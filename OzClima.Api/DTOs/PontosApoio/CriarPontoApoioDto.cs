
using System.ComponentModel.DataAnnotations;
using OzClima.Api.Enums;

namespace OzClima.Api.DTOs.PontosApoio;

public class CriarPontoApoioDto
{
    [Range(1, int.MaxValue,
        ErrorMessage = "A área monitorada é obrigatória.")]
    public int AreaMonitoradaId { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O endereço é obrigatório.")]
    [StringLength(250, ErrorMessage = "O endereço deve ter no máximo 250 caracteres.")]
    public string Endereco { get; set; } = string.Empty;

    [EnumDataType(typeof(TipoPontoApoio),
        ErrorMessage = "Tipo de ponto de apoio inválido.")]
    public TipoPontoApoio Tipo { get; set; }

    [Range(-90, 90, ErrorMessage = "Latitude inválida.")]
    public double Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude inválida.")]
    public double Longitude { get; set; }
}
