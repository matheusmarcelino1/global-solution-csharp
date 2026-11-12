
using Microsoft.AspNetCore.Mvc;
using OzClima.Api.DTOs.Areas;
using OzClima.Api.Interfaces;

namespace OzClima.Api.Controllers;

[ApiController]
[Route("api/areas")]
public class AreasController : ControllerBase
{
    private readonly IAreaMonitoradaService _areaService;

    public AreasController(IAreaMonitoradaService areaService)
    {
        _areaService = areaService;
    }


    [HttpPost]
    [ProducesResponseType(typeof(AreaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AreaResponseDto>> Criar(
        [FromBody] CriarAreaDto dto)
    {
        var area = await _areaService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = area.Id },
            area
        );
    }


    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AreaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AreaResponseDto>>> Listar()
    {
        var areas = await _areaService.ListarAsync();

        return Ok(areas);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AreaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AreaResponseDto>> BuscarPorId(int id)
    {
        var area = await _areaService.BuscarPorIdAsync(id);

        if (area is null)
            return NotFound(new { mensagem = "Área não encontrada." });

        return Ok(area);
    }


    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromBody] AtualizarAreaDto dto)
    {
        var atualizada = await _areaService.AtualizarAsync(id, dto);

        if (!atualizada)
            return NotFound(new { mensagem = "Área não encontrada." });

        return NoContent();
    }


    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(int id)
    {
        var desativada = await _areaService.DesativarAsync(id);

        if (!desativada)
            return NotFound(new { mensagem = "Área não encontrada." });

        return NoContent();
    }
}
