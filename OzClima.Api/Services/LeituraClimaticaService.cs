
using Microsoft.EntityFrameworkCore;
using OzClima.Api.Data;
using OzClima.Api.DTOs.Leituras;
using OzClima.Api.Interfaces;
using OzClima.Api.Models;
using OzClima.Api.DTOs.Alertas;
using OzClima.Api.Enums;

namespace OzClima.Api.Services;

public class LeituraClimaticaService : ILeituraClimaticaService
{

    private readonly AppDbContext _context;
    private readonly IRiscoTermicoService _riscoTermicoService;
    private readonly IAlertaCalorService _alertaCalorService;

    public LeituraClimaticaService(
        AppDbContext context,
        IRiscoTermicoService riscoTermicoService,
        IAlertaCalorService alertaCalorService)
    {
        _context = context;
        _riscoTermicoService = riscoTermicoService;
        _alertaCalorService = alertaCalorService;
    }




    public async Task<LeituraResponseDto> CriarAsync(CriarLeituraDto dto)
    {
        var areaExiste = await _context.AreasMonitoradas
            .AnyAsync(x =>
                x.Id == dto.AreaMonitoradaId && x.Ativa);

        if (!areaExiste)
            throw new KeyNotFoundException(
                "Área monitorada não encontrada ou inativa.");

        var leitura = new LeituraClimatica(
            dto.AreaMonitoradaId,
            dto.Temperatura,
            dto.Umidade
        );

        var nivelAtual = _riscoTermicoService.Calcular(
            leitura.Temperatura,
            leitura.Umidade
        );

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var leituraAnterior = await _context.LeiturasClimaticas
            .AsNoTracking()
            .Where(x => x.AreaMonitoradaId == dto.AreaMonitoradaId)
            .OrderByDescending(x => x.DataHora)
            .ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync();

        NivelRiscoTermico? nivelAnterior = null;

        if (leituraAnterior is not null)
        {
            nivelAnterior = _riscoTermicoService.Calcular(
                leituraAnterior.Temperatura,
                leituraAnterior.Umidade
            );
        }

        await _context.LeiturasClimaticas.AddAsync(leitura);
        await _context.SaveChangesAsync();

        await _alertaCalorService.ProcessarLeituraAsync(
            dto.AreaMonitoradaId,
            nivelAtual,
            nivelAnterior
        );

        await transaction.CommitAsync();

        return MapearParaDto(leitura);
    }



    public async Task<IEnumerable<LeituraResponseDto>> ListarAsync()
    {
        var leituras = await _context.LeiturasClimaticas
            .AsNoTracking()
            .OrderByDescending(x => x.DataHora)
            .ToListAsync();

        return leituras.Select(MapearParaDto).ToList();
    }

    public async Task<LeituraResponseDto?> BuscarPorIdAsync(int id)
    {
        var leitura = await _context.LeiturasClimaticas
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (leitura is null)
            return null;

        return MapearParaDto(leitura);
    }

    public async Task<IEnumerable<LeituraResponseDto>> ListarPorAreaAsync(
        int areaId)
    {
        var leituras = await _context.LeiturasClimaticas
            .AsNoTracking()
            .Where(x => x.AreaMonitoradaId == areaId)
            .OrderByDescending(x => x.DataHora)
            .ToListAsync();

        return leituras.Select(MapearParaDto).ToList();
    }

    public async Task<ResumoLeiturasDto?> ObterResumoPorAreaAsync(
        int areaId)
    {
        var leituras = await _context.LeiturasClimaticas
            .AsNoTracking()
            .Where(x => x.AreaMonitoradaId == areaId)
            .ToListAsync();

        if (leituras.Count == 0)
            return null;

        return new ResumoLeiturasDto
        {
            AreaMonitoradaId = areaId,
            TotalLeituras = leituras.Count,
            TemperaturaMedia = leituras.Average(x => x.Temperatura),
            TemperaturaMaxima = leituras.Max(x => x.Temperatura),
            TemperaturaMinima = leituras.Min(x => x.Temperatura),
            UmidadeMedia = leituras.Average(x => x.Umidade)
        };
    }

    private LeituraResponseDto MapearParaDto(LeituraClimatica leitura)
    {
        return new LeituraResponseDto
        {
            Id = leitura.Id,
            AreaMonitoradaId = leitura.AreaMonitoradaId,
            Temperatura = leitura.Temperatura,
            Umidade = leitura.Umidade,
            Humidex = _riscoTermicoService.CalcularHumidex(
                leitura.Temperatura,
                leitura.Umidade
            ),
            DataHora = leitura.DataHora,
            NivelRisco = _riscoTermicoService.Calcular(
                leitura.Temperatura,
                leitura.Umidade
            )
        };
    }
}
