
using OzClima.Api.DTOs.Areas;

namespace OzClima.Api.Interfaces;

public interface IAreaMonitoradaService
{
    Task<AreaResponseDto> CriarAsync(
        CriarAreaDto dto);

    Task<IEnumerable<AreaResponseDto>> ListarAsync();

    Task<AreaResponseDto?> BuscarPorIdAsync(
        int id);

    Task<bool> AtualizarAsync(
        int id,
        AtualizarAreaDto dto);

    Task<bool> DesativarAsync(
        int id);
}
