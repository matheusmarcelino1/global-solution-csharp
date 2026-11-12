
using Microsoft.AspNetCore.Mvc;
using OzClima.Api.DTOs.PontosApoio;
using OzClima.Api.Enums;
using OzClima.Api.Interfaces;

namespace OzClima.Api.Controllers;

[ApiController]
[Route("api/pontos-apoio")]
public class PontosApoioController : ControllerBase
{
    private readonly IPontoApoioService _pontoService;

    public PontosApoioController(IPontoApoioService pontoService)
    {
        _pontoService = pontoService;
    }


    [HttpPost]
    [ProducesResponseType(typeof(PontoApoioResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PontoApoioResponseDto>> Criar(
        [FromBody] CriarPontoApoioDto dto)
    {
        var ponto = await _pontoService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = ponto.Id },
            ponto
        );
    }


    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PontoApoioResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PontoApoioResponseDto>>> Listar()
    {
        var pontos = await _pontoService.ListarAsync();

        return Ok(pontos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PontoApoioResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PontoApoioResponseDto>> BuscarPorId(
        int id)
    {
        var ponto = await _pontoService.BuscarPorIdAsync(id);

        if (ponto is null)
            return NotFound(new { mensagem = "Ponto de apoio não encontrado." });

        return Ok(ponto);
    }

    [HttpGet("area/{areaId:int}")]
    [ProducesResponseType(typeof(IEnumerable<PontoApoioResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PontoApoioResponseDto>>> ListarPorArea(
        int areaId)
    {
        var pontos = await _pontoService.ListarPorAreaAsync(areaId);

        return Ok(pontos);
    }


    [HttpGet("tipo/{tipo}")]
    [ProducesResponseType(typeof(IEnumerable<PontoApoioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<PontoApoioResponseDto>>> ListarPorTipo(
        TipoPontoApoio tipo)
    {
        var pontos = await _pontoService.ListarPorTipoAsync(tipo);

        return Ok(pontos);
    }


    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(int id)
    {
        var desativado = await _pontoService.DesativarAsync(id);

        if (!desativado)
            return NotFound(new { mensagem = "Ponto de apoio não encontrado ou inativo." });

        return NoContent();
    }
}
