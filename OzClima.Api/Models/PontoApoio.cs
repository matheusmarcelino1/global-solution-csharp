
using OzClima.Api.Enums;

namespace OzClima.Api.Models;

public class PontoApoio
{
    public int Id { get; private set; }
    public int AreaMonitoradaId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Endereco { get; private set; } = string.Empty;
    public TipoPontoApoio Tipo { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public bool Ativo { get; private set; }

    public AreaMonitorada AreaMonitorada { get; private set; } = null!;

    private PontoApoio() { }

    public PontoApoio(
        int areaMonitoradaId,
        string nome,
        string endereco,
        TipoPontoApoio tipo,
        double latitude,
        double longitude)
    {
        if (areaMonitoradaId <= 0)
            throw new ArgumentException("A área monitorada é inválida.");

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(endereco))
            throw new ArgumentException("O endereço é obrigatório.");

        if (!Enum.IsDefined(tipo))
            throw new ArgumentException("Tipo de ponto de apoio inválido.");

        if (!double.IsFinite(latitude) || latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude inválida.");

        if (!double.IsFinite(longitude) || longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude inválida.");

        AreaMonitoradaId = areaMonitoradaId;
        Nome = nome.Trim();
        Endereco = endereco.Trim();
        Tipo = tipo;
        Latitude = latitude;
        Longitude = longitude;
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
    }
}
