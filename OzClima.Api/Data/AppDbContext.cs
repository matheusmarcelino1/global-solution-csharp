
using Microsoft.EntityFrameworkCore;
using OzClima.Api.Models;

namespace OzClima.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AreaMonitorada> AreasMonitoradas => Set<AreaMonitorada>();

    public DbSet<LeituraClimatica> LeiturasClimaticas => Set<LeituraClimatica>();

    public DbSet<AlertaCalor> AlertasCalor => Set<AlertaCalor>();

    public DbSet<PontoApoio> PontosApoio => Set<PontoApoio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // AREA MONITORADA
        modelBuilder.Entity<AreaMonitorada>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Bairro)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.NivelArborizacao)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasMany(x => x.Leituras)
                .WithOne(x => x.AreaMonitorada)
                .HasForeignKey(x => x.AreaMonitoradaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Alertas)
                .WithOne(x => x.AreaMonitorada)
                .HasForeignKey(x => x.AreaMonitoradaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.PontosApoio)
                .WithOne(x => x.AreaMonitorada)
                .HasForeignKey(x => x.AreaMonitoradaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // LEITURA CLIMATICA
        modelBuilder.Entity<LeituraClimatica>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Temperatura)
                .IsRequired();

            entity.Property(x => x.Umidade)
                .IsRequired();

            entity.Property(x => x.DataHora)
                .IsRequired();

            entity.HasIndex(x => new
            {
                x.AreaMonitoradaId,
                x.DataHora
            });
        });

        // ALERTA DE CALOR
        modelBuilder.Entity<AlertaCalor>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nivel)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(x => x.Mensagem)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(x => x.DataHora)
                .IsRequired();
        });

        // PONTO DE APOIO
        modelBuilder.Entity<PontoApoio>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Endereco)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.Tipo)
                .HasConversion<string>()
                .HasMaxLength(30);
        });
    }
}
