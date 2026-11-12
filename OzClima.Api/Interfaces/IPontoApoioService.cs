
using OzClima.Api.DTOs.PontosApoio;
using OzClima.Api.Enums;

namespace OzClima.Api.Interfaces;

public interface IPontoApoioService
{
    Task<PontoApoioResponseDto> CriarAsync(
        CriarPontoApoioDto dto);

    Task<IEnumerable<PontoApoioResponseDto>> ListarAsync();

    Task<PontoApoioResponseDto?> BuscarPorIdAsync(
        int id);

    Task<IEnumerable<PontoApoioResponseDto>> ListarPorAreaAsync(
        int areaId);

    Task<IEnumerable<PontoApoioResponseDto>> ListarPorTipoAsync(
        TipoPontoApoio tipo);

    Task<bool> DesativarAsync(int id);
}
