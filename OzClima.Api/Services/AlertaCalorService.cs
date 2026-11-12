
using Microsoft.EntityFrameworkCore;
using OzClima.Api.Data;
using OzClima.Api.DTOs.Alertas;
using OzClima.Api.Enums;
using OzClima.Api.Interfaces;
using OzClima.Api.Models;

namespace OzClima.Api.Services;

public class AlertaCalorService : IAlertaCalorService
{
    private readonly AppDbContext _context;

    public AlertaCalorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AlertaResponseDto?> CriarAsync(
        CriarAlertaDto dto)
    {
        if (dto.Nivel != NivelRiscoTermico.Alto &&
            dto.Nivel != NivelRiscoTermico.Critico)
        {
            return null;
        }

        var area = await _context.AreasMonitoradas
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == dto.AreaMonitoradaId && x.Ativa);

        if (area is null)
            throw new KeyNotFoundException(
                "Área monitorada não encontrada ou inativa.");

        var alertaExistente = await _context.AlertasCalor
            .AnyAsync(x =>
                x.AreaMonitoradaId == dto.AreaMonitoradaId &&
                x.Nivel == dto.Nivel &&
                x.Ativo);

        if (alertaExistente)
            return null;

        var alerta = new AlertaCalor(
            dto.AreaMonitoradaId,
            dto.Nivel,
            dto.Mensagem
        );

        await _context.AlertasCalor.AddAsync(alerta);
        await _context.SaveChangesAsync();

        return MapearParaDto(alerta, area.Nome);
    }


    public async Task ProcessarLeituraAsync(
        int areaId,
        NivelRiscoTermico nivelAtual,
        NivelRiscoTermico? nivelAnterior)
    {
        if (areaId <= 0)
            throw new ArgumentException("Área monitorada inválida.");

        if (!Enum.IsDefined(nivelAtual) ||
            (nivelAnterior.HasValue && !Enum.IsDefined(nivelAnterior.Value)))
            throw new ArgumentException("Nível de risco inválido.");

        var area = await _context.AreasMonitoradas
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == areaId && x.Ativa);

        if (area is null)
            throw new KeyNotFoundException(
                "Área monitorada não encontrada ou inativa.");

        var alertasAtivos = await _context.AlertasCalor
            .Where(x => x.AreaMonitoradaId == areaId && x.Ativo)
            .OrderByDescending(x => x.Nivel)
            .ToListAsync();

        var alertaPrincipal = alertasAtivos.FirstOrDefault();

        // Não há alerta ativo
        if (alertaPrincipal is null)
        {
            if (nivelAtual == NivelRiscoTermico.Alto ||
                nivelAtual == NivelRiscoTermico.Critico)
            {
                var novoAlerta = new AlertaCalor(
                    areaId,
                    nivelAtual,
                    GerarMensagem(nivelAtual)
                );

                await _context.AlertasCalor.AddAsync(novoAlerta);
                await _context.SaveChangesAsync();
            }

            return;
        }

        // O nível de risco permanece igual
        if (nivelAtual == alertaPrincipal.Nivel)
        {
            // Corrige eventuais alertas ativos antigos duplicados
            foreach (var alerta in alertasAtivos.Skip(1))
                alerta.Encerrar();

            await _context.SaveChangesAsync();
            return;
        }

        // O risco aumentou: atualiza imediatamente
        if (nivelAtual > alertaPrincipal.Nivel)
        {
            foreach (var alerta in alertasAtivos)
                alerta.Encerrar();

            var novoAlerta = new AlertaCalor(
                areaId,
                nivelAtual,
                GerarMensagem(nivelAtual)
            );

            await _context.AlertasCalor.AddAsync(novoAlerta);
            await _context.SaveChangesAsync();

            return;
        }

        // O risco diminuiu: exige confirmação da leitura anterior
        var reducaoConfirmada =
            nivelAnterior.HasValue &&
            nivelAnterior.Value < alertaPrincipal.Nivel;

        if (!reducaoConfirmada)
            return;

        // Encerra os alertas anteriores
        foreach (var alerta in alertasAtivos)
            alerta.Encerrar();

        // Se o novo risco ainda for Alto, cria um novo alerta
        if (nivelAtual == NivelRiscoTermico.Alto)
        {
            var novoAlerta = new AlertaCalor(
                areaId,
                nivelAtual,
                GerarMensagem(nivelAtual)
            );

            await _context.AlertasCalor.AddAsync(novoAlerta);
        }

        await _context.SaveChangesAsync();
    }

    private static string GerarMensagem(NivelRiscoTermico nivel)
    {
        return nivel switch
        {
            NivelRiscoTermico.Alto =>
                "Temperatura elevada registrada na região.",

            NivelRiscoTermico.Critico =>
                "Temperatura muito elevada registrada na região.",

            _ => "Condição climática registrada."
        };
    }


    public async Task<IEnumerable<AlertaResponseDto>> ListarAsync()
    {
        return await ConsultarAlertas()
            .ToListAsync();
    }

    public async Task<IEnumerable<AlertaResponseDto>> ListarAtivosAsync()
    {
        return await ConsultarAlertas()
            .Where(x => x.Ativo)
            .ToListAsync();
    }

    public async Task<IEnumerable<AlertaResponseDto>> ListarPorAreaAsync(
        int areaId)
    {
        return await ConsultarAlertas()
            .Where(x => x.AreaMonitoradaId == areaId)
            .ToListAsync();
    }

    public async Task<bool> EncerrarAsync(int id)
    {
        var alerta = await _context.AlertasCalor
            .FirstOrDefaultAsync(x => x.Id == id && x.Ativo);

        if (alerta is null)
            return false;

        alerta.Encerrar();

        await _context.SaveChangesAsync();

        return true;
    }

    private IQueryable<AlertaResponseDto> ConsultarAlertas()
    {
        return _context.AlertasCalor
            .AsNoTracking()
            .OrderByDescending(x => x.DataHora)
            .Select(x => new AlertaResponseDto
            {
                Id = x.Id,
                AreaMonitoradaId = x.AreaMonitoradaId,
                NomeArea = x.AreaMonitorada.Nome,
                Nivel = x.Nivel,
                Mensagem = x.Mensagem,
                DataHora = x.DataHora,
                Ativo = x.Ativo
            });
    }

    private static AlertaResponseDto MapearParaDto(
        AlertaCalor alerta,
        string nomeArea)
    {
        return new AlertaResponseDto
        {
            Id = alerta.Id,
            AreaMonitoradaId = alerta.AreaMonitoradaId,
            NomeArea = nomeArea,
            Nivel = alerta.Nivel,
            Mensagem = alerta.Mensagem,
            DataHora = alerta.DataHora,
            Ativo = alerta.Ativo
        };
    }
}
