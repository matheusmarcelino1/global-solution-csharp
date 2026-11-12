
using Microsoft.EntityFrameworkCore;
using OzClima.Api.Data;
using OzClima.Api.DTOs.PontosApoio;
using OzClima.Api.Enums;
using OzClima.Api.Interfaces;
using OzClima.Api.Models;

namespace OzClima.Api.Services;

public class PontoApoioService : IPontoApoioService
{
    private readonly AppDbContext _context;

    public PontoApoioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PontoApoioResponseDto> CriarAsync(
        CriarPontoApoioDto dto)
    {
        var area = await _context.AreasMonitoradas
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == dto.AreaMonitoradaId && x.Ativa);

        if (area is null)
            throw new KeyNotFoundException(
                "Área monitorada não encontrada ou inativa.");

        var ponto = new PontoApoio(
            dto.AreaMonitoradaId,
            dto.Nome,
            dto.Endereco,
            dto.Tipo,
            dto.Latitude,
            dto.Longitude
        );

        await _context.PontosApoio.AddAsync(ponto);
        await _context.SaveChangesAsync();

        return MapearParaDto(ponto, area.Nome);
    }

    public async Task<IEnumerable<PontoApoioResponseDto>> ListarAsync()
    {
        return await ConsultarPontos()
            .ToListAsync();
    }

    public async Task<PontoApoioResponseDto?> BuscarPorIdAsync(int id)
    {
        return await ConsultarPontos()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<PontoApoioResponseDto>> ListarPorAreaAsync(
        int areaId)
    {
        return await ConsultarPontos()
            .Where(x => x.AreaMonitoradaId == areaId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PontoApoioResponseDto>> ListarPorTipoAsync(
        TipoPontoApoio tipo)
    {
        if (!Enum.IsDefined(tipo))
            throw new ArgumentException(
                "Tipo de ponto de apoio inválido.");

        return await ConsultarPontos()
            .Where(x => x.Tipo == tipo)
            .ToListAsync();
    }

    public async Task<bool> DesativarAsync(int id)
    {
        var ponto = await _context.PontosApoio
            .FirstOrDefaultAsync(x => x.Id == id && x.Ativo);

        if (ponto is null)
            return false;

        ponto.Desativar();

        await _context.SaveChangesAsync();

        return true;
    }

    private IQueryable<PontoApoioResponseDto> ConsultarPontos()
    {
        return _context.PontosApoio
            .AsNoTracking()
            .Where(x => x.Ativo && x.AreaMonitorada.Ativa)
            .OrderBy(x => x.Nome)
            .Select(x => new PontoApoioResponseDto
            {
                Id = x.Id,
                AreaMonitoradaId = x.AreaMonitoradaId,
                NomeArea = x.AreaMonitorada.Nome,
                Nome = x.Nome,
                Endereco = x.Endereco,
                Tipo = x.Tipo,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Ativo = x.Ativo
            });
    }

    private static PontoApoioResponseDto MapearParaDto(
        PontoApoio ponto,
        string nomeArea)
    {
        return new PontoApoioResponseDto
        {
            Id = ponto.Id,
            AreaMonitoradaId = ponto.AreaMonitoradaId,
            NomeArea = nomeArea,
            Nome = ponto.Nome,
            Endereco = ponto.Endereco,
            Tipo = ponto.Tipo,
            Latitude = ponto.Latitude,
            Longitude = ponto.Longitude,
            Ativo = ponto.Ativo
        };
    }
}

