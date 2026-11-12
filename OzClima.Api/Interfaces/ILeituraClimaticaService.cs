
using OzClima.Api.DTOs.Leituras;

namespace OzClima.Api.Interfaces;

public interface ILeituraClimaticaService
{
    Task<LeituraResponseDto> CriarAsync(
        CriarLeituraDto dto);

    Task<IEnumerable<LeituraResponseDto>> ListarAsync();

    Task<LeituraResponseDto?> BuscarPorIdAsync(
        int id);

    Task<IEnumerable<LeituraResponseDto>> ListarPorAreaAsync(
        int areaId);

    Task<ResumoLeiturasDto?> ObterResumoPorAreaAsync(
        int areaId);
}
