
using System.ComponentModel.DataAnnotations;

namespace OzClima.Api.DTOs.Leituras;

public class CriarLeituraDto
{
    [Range(1, int.MaxValue,
        ErrorMessage = "A área monitorada é obrigatória.")]
    public int AreaMonitoradaId { get; set; }

    [Range(typeof(decimal), "-20", "60",
        ErrorMessage = "A temperatura deve estar entre -20°C e 60°C.")]
    public decimal Temperatura { get; set; }

    [Range(typeof(decimal), "0", "100",
        ErrorMessage = "A umidade deve estar entre 0% e 100%.")]
    public decimal Umidade { get; set; }
}
