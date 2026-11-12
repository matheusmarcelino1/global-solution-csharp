
using System.ComponentModel.DataAnnotations;
using OzClima.Api.Enums;

namespace OzClima.Api.DTOs.Areas;

public class AtualizarAreaDto
{
    [Required(ErrorMessage = "O nome da área é obrigatório.")]
    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O bairro é obrigatório.")]
    [StringLength(100, ErrorMessage = "O bairro deve ter no máximo 100 caracteres.")]
    public string Bairro { get; set; } = string.Empty;

    [Range(-90, 90, ErrorMessage = "Latitude inválida.")]
    public double Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude inválida.")]
    public double Longitude { get; set; }

    [EnumDataType(typeof(NivelArborizacao),
        ErrorMessage = "Nível de arborização inválido.")]
    public NivelArborizacao NivelArborizacao { get; set; }
}
