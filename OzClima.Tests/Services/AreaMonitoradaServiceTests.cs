
using OzClima.Api.DTOs.Areas;
using OzClima.Api.Enums;
using OzClima.Api.Services;
using OzClima.Tests.Helpers;

namespace OzClima.Tests.Services;

public class AreaMonitoradaServiceTests
{
    private static CriarAreaDto CriarDto()
    {
        return new CriarAreaDto
        {
            Nome = "Centro de Osasco",
            Bairro = "Centro",
            Latitude = -23.532,
            Longitude = -46.791,
            NivelArborizacao = NivelArborizacao.Baixo
        };
    }

    [Fact]
    public async Task DeveCadastrarAreaComSucesso()
    {
        // Arrange
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using (connection)
        using (context)
        {
            var service = new AreaMonitoradaService(context);

            // Act
            var resultado = await service.CriarAsync(CriarDto());

            // Assert
            Assert.True(resultado.Id > 0);
            Assert.Equal("Centro de Osasco", resultado.Nome);
            Assert.True(resultado.Ativa);

            var areaSalva =
                await context.AreasMonitoradas.FindAsync(resultado.Id);

            Assert.NotNull(areaSalva);
        }
    }

    [Fact]
    public async Task DeveListarSomenteAreasAtivas()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using (connection)
        using (context)
        {
            var service = new AreaMonitoradaService(context);

            var area1 = await service.CriarAsync(CriarDto());

            var area2 = await service.CriarAsync(new CriarAreaDto
            {
                Nome = "Vila Yara",
                Bairro = "Vila Yara",
                Latitude = -23.54,
                Longitude = -46.76,
                NivelArborizacao = NivelArborizacao.Medio
            });

            await service.DesativarAsync(area1.Id);

            // Act
            var resultado = (await service.ListarAsync()).ToList();

            // Assert
            Assert.Single(resultado);
            Assert.Equal(area2.Id, resultado[0].Id);
        }
    }

    [Fact]
    public async Task DeveBuscarAreaPorId()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using (connection)
        using (context)
        {
            var service = new AreaMonitoradaService(context);

            var area = await service.CriarAsync(CriarDto());

            // Act
            var resultado = await service.BuscarPorIdAsync(area.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(area.Id, resultado.Id);
            Assert.Equal("Centro de Osasco", resultado.Nome);
        }
    }

    [Fact]
    public async Task DeveRetornarNullQuandoAreaNaoExistir()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using (connection)
        using (context)
        {
            var service = new AreaMonitoradaService(context);

            // Act
            var resultado = await service.BuscarPorIdAsync(999);

            // Assert
            Assert.Null(resultado);
        }
    }

    [Fact]
    public async Task DeveAtualizarAreaComSucesso()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using (connection)
        using (context)
        {
            var service = new AreaMonitoradaService(context);

            var area = await service.CriarAsync(CriarDto());

            var atualizarDto = new AtualizarAreaDto
            {
                Nome = "Centro Atualizado",
                Bairro = "Centro",
                Latitude = -23.533,
                Longitude = -46.792,
                NivelArborizacao = NivelArborizacao.Alto
            };

            // Act
            var atualizado = await service.AtualizarAsync(
                area.Id,
                atualizarDto
            );

            var resultado = await service.BuscarPorIdAsync(area.Id);

            // Assert
            Assert.True(atualizado);
            Assert.NotNull(resultado);
            Assert.Equal("Centro Atualizado", resultado.Nome);
            Assert.Equal(
                NivelArborizacao.Alto,
                resultado.NivelArborizacao
            );
        }
    }

    [Fact]
    public async Task DeveDesativarAreaSemExcluirRegistro()
    {
        var (context, connection) =
            TestDbContextFactory.CriarContexto();

        using (connection)
        using (context)
        {
            var service = new AreaMonitoradaService(context);

            var area = await service.CriarAsync(CriarDto());

            // Act
            var desativada = await service.DesativarAsync(area.Id);

            var consulta = await service.BuscarPorIdAsync(area.Id);

            var registroBanco =
                await context.AreasMonitoradas.FindAsync(area.Id);

            // Assert
            Assert.True(desativada);
            Assert.Null(consulta);
            Assert.NotNull(registroBanco);
            Assert.False(registroBanco.Ativa);
        }
    }
}
