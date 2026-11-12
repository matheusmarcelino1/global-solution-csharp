
using OzClima.Api.DTOs.Alertas;
using OzClima.Api.Enums;

namespace OzClima.Api.Interfaces;

public interface IAlertaCalorService
{
    Task<AlertaResponseDto?> CriarAsync(
        CriarAlertaDto dto);

    Task ProcessarLeituraAsync(
        int areaId,
        NivelRiscoTermico nivelAtual,
        NivelRiscoTermico? nivelAnterior);

    Task<IEnumerable<AlertaResponseDto>> ListarAsync();

    Task<IEnumerable<AlertaResponseDto>> ListarAtivosAsync();

    Task<IEnumerable<AlertaResponseDto>> ListarPorAreaAsync(
        int areaId);

    Task<bool> EncerrarAsync(int id);
}
