
using OzClima.Api.Enums;

namespace OzClima.Api.Models;

public class AreaMonitorada
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Bairro { get; private set; } = string.Empty;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public NivelArborizacao NivelArborizacao { get; private set; }
    public bool Ativa { get; private set; } = true;

    public ICollection<LeituraClimatica> Leituras { get; private set; }
        = new List<LeituraClimatica>();

    public ICollection<AlertaCalor> Alertas { get; private set; }
        = new List<AlertaCalor>();

    public ICollection<PontoApoio> PontosApoio { get; private set; }
        = new List<PontoApoio>();

    private AreaMonitorada() { }

    public AreaMonitorada(
        string nome,
        string bairro,
        double latitude,
        double longitude,
        NivelArborizacao nivelArborizacao)
    {
        Validar(nome, bairro, latitude, longitude, nivelArborizacao);

        Nome = nome.Trim();
        Bairro = bairro.Trim();
        Latitude = latitude;
        Longitude = longitude;
        NivelArborizacao = nivelArborizacao;
    }

    public void Atualizar(
        string nome,
        string bairro,
        double latitude,
        double longitude,
        NivelArborizacao nivelArborizacao)
    {
        Validar(nome, bairro, latitude, longitude, nivelArborizacao);

        Nome = nome.Trim();
        Bairro = bairro.Trim();
        Latitude = latitude;
        Longitude = longitude;
        NivelArborizacao = nivelArborizacao;
    }

    public void Desativar()
    {
        Ativa = false;
    }

    private static void Validar(
        string nome,
        string bairro,
        double latitude,
        double longitude,
        NivelArborizacao nivelArborizacao)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome da área é obrigatório.");

        if (string.IsNullOrWhiteSpace(bairro))
            throw new ArgumentException("O bairro é obrigatório.");

        if (!double.IsFinite(latitude) || latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude inválida.");

        if (!double.IsFinite(longitude) || longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude inválida.");

        if (!Enum.IsDefined(nivelArborizacao))
            throw new ArgumentException("Nível de arborização inválido.");
    }
}
