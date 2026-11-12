
using Microsoft.AspNetCore.Mvc;
using OzClima.Api.DTOs.Leituras;
using OzClima.Api.Interfaces;

namespace OzClima.Api.Controllers;

[ApiController]
[Route("api/leituras")]
public class LeiturasController : ControllerBase
{
    private readonly ILeituraClimaticaService _leituraService;

    public LeiturasController(
        ILeituraClimaticaService leituraService)
    {
        _leituraService = leituraService;
    }


    [HttpPost]
    [ProducesResponseType(typeof(LeituraResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeituraResponseDto>> Criar(
        [FromBody] CriarLeituraDto dto)
    {
        var leitura = await _leituraService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = leitura.Id },
            leitura
        );
    }


    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LeituraResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LeituraResponseDto>>> Listar()
    {
        var leituras = await _leituraService.ListarAsync();

        return Ok(leituras);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LeituraResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeituraResponseDto>> BuscarPorId(int id)
    {
        var leitura = await _leituraService.BuscarPorIdAsync(id);

        if (leitura is null)
            return NotFound(new { mensagem = "Leitura não encontrada." });

        return Ok(leitura);
    }

    [HttpGet("area/{areaId:int}")]
    [ProducesResponseType(typeof(IEnumerable<LeituraResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LeituraResponseDto>>> ListarPorArea(
        int areaId)
    {
        var leituras = await _leituraService.ListarPorAreaAsync(areaId);

        return Ok(leituras);
    }

    [HttpGet("area/{areaId:int}/resumo")]
    [ProducesResponseType(typeof(ResumoLeiturasDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResumoLeiturasDto>> ObterResumoPorArea(
        int areaId)
    {
        var resumo = await _leituraService.ObterResumoPorAreaAsync(areaId);

        if (resumo is null)
            return NotFound(new
            {
                mensagem = "Nenhuma leitura encontrada para esta área."
            });

        return Ok(resumo);
    }
}
