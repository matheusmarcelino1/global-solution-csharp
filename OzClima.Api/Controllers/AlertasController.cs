
using Microsoft.AspNetCore.Mvc;
using OzClima.Api.DTOs.Alertas;
using OzClima.Api.Interfaces;

namespace OzClima.Api.Controllers;

[ApiController]
[Route("api/alertas")]
public class AlertasController : ControllerBase
{
    private readonly IAlertaCalorService _alertaService;

    public AlertasController(IAlertaCalorService alertaService)
    {
        _alertaService = alertaService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<AlertaResponseDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertaResponseDto>>> Listar()
    {
        var alertas = await _alertaService.ListarAsync();

        return Ok(alertas);
    }

    [HttpGet("ativos")]
    [ProducesResponseType(
        typeof(IEnumerable<AlertaResponseDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertaResponseDto>>> ListarAtivos()
    {
        var alertas = await _alertaService.ListarAtivosAsync();

        return Ok(alertas);
    }

    [HttpGet("area/{areaId:int}")]
    [ProducesResponseType(
        typeof(IEnumerable<AlertaResponseDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertaResponseDto>>> ListarPorArea(
        int areaId)
    {
        var alertas = await _alertaService.ListarPorAreaAsync(areaId);

        return Ok(alertas);
    }

    [HttpPatch("{id:int}/encerrar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Encerrar(int id)
    {
        var encerrado = await _alertaService.EncerrarAsync(id);

        if (!encerrado)
            return NotFound(new
            {
                mensagem = "Alerta não encontrado ou já encerrado."
            });

        return NoContent();
    }
}
