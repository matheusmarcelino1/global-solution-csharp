
using OzClima.Api.Enums;

namespace OzClima.Api.Models;

public class AlertaCalor
{
    public int Id { get; private set; }
    public int AreaMonitoradaId { get; private set; }
    public NivelRiscoTermico Nivel { get; private set; }
    public string Mensagem { get; private set; } = string.Empty;
    public DateTime DataHora { get; private set; }
    public bool Ativo { get; private set; }

    public AreaMonitorada AreaMonitorada { get; private set; } = null!;

    private AlertaCalor() { }

    public AlertaCalor(
        int areaMonitoradaId,
        NivelRiscoTermico nivel,
        string mensagem)
    {
        if (areaMonitoradaId <= 0)
            throw new ArgumentException("A área monitorada é inválida.");

        if (!Enum.IsDefined(nivel))
            throw new ArgumentException("Nível de risco inválido.");

        if (string.IsNullOrWhiteSpace(mensagem))
            throw new ArgumentException("A mensagem é obrigatória.");

        AreaMonitoradaId = areaMonitoradaId;
        Nivel = nivel;
        Mensagem = mensagem.Trim();
        DataHora = DateTime.UtcNow;
        Ativo = true;
    }

    public void Encerrar()
    {
        Ativo = false;
    }
}
