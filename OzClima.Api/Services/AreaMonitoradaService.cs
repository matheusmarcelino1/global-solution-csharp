
using Microsoft.EntityFrameworkCore;
using OzClima.Api.Data;
using OzClima.Api.DTOs.Areas;
using OzClima.Api.Interfaces;
using OzClima.Api.Models;

namespace OzClima.Api.Services;

public class AreaMonitoradaService : IAreaMonitoradaService
{
    private readonly AppDbContext _context;

    public AreaMonitoradaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AreaResponseDto> CriarAsync(CriarAreaDto dto)
    {
        var area = new AreaMonitorada(
            dto.Nome,
            dto.Bairro,
            dto.Latitude,
            dto.Longitude,
            dto.NivelArborizacao
        );

        await _context.AreasMonitoradas.AddAsync(area);
        await _context.SaveChangesAsync();

        return MapearParaDto(area);
    }

    public async Task<IEnumerable<AreaResponseDto>> ListarAsync()
    {
        return await _context.AreasMonitoradas
            .AsNoTracking()
            .Where(x => x.Ativa)
            .OrderBy(x => x.Bairro)
            .ThenBy(x => x.Nome)
            .Select(x => new AreaResponseDto
            {
                Id = x.Id,
                Nome = x.Nome,
                Bairro = x.Bairro,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                NivelArborizacao = x.NivelArborizacao,
                Ativa = x.Ativa
            })
            .ToListAsync();
    }

    public async Task<AreaResponseDto?> BuscarPorIdAsync(int id)
    {
        var area = await _context.AreasMonitoradas
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.Ativa);

        if (area is null)
            return null;

        return MapearParaDto(area);
    }

    public async Task<bool> AtualizarAsync(
        int id,
        AtualizarAreaDto dto)
    {
        var area = await _context.AreasMonitoradas
            .FirstOrDefaultAsync(x => x.Id == id && x.Ativa);

        if (area is null)
            return false;

        area.Atualizar(
            dto.Nome,
            dto.Bairro,
            dto.Latitude,
            dto.Longitude,
            dto.NivelArborizacao
        );

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DesativarAsync(int id)
    {
        var area = await _context.AreasMonitoradas
            .FirstOrDefaultAsync(x => x.Id == id && x.Ativa);

        if (area is null)
            return false;

        area.Desativar();

        await _context.SaveChangesAsync();

        return true;
    }

    private static AreaResponseDto MapearParaDto(AreaMonitorada area)
    {
        return new AreaResponseDto
        {
            Id = area.Id,
            Nome = area.Nome,
            Bairro = area.Bairro,
            Latitude = area.Latitude,
            Longitude = area.Longitude,
            NivelArborizacao = area.NivelArborizacao,
            Ativa = area.Ativa
        };
    }
}
